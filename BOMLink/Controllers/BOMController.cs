using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOMLink.Data;
using BOMLink.Models;
using BOMLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BOMLink.Controllers {
    [Authorize]
    public class BOMController : Controller {
        private readonly BOMLinkContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BOMController(BOMLinkContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        // GET: List all BOMs with search and sorting
        public async Task<IActionResult> Index(string searchTerm, string sortBy, string sortOrder, string customerCodeFilter, string createdByFilter) {
            var query = _context.BOMs
                .Include(b => b.Job)
                .Include(b => b.Customer)
                .Include(b => b.CreatedBy)
                .AsQueryable();

            // Collect Unique Customers & Users for Filtering
            var allCustomerCodes = await _context.Customers.Select(c => c.CustomerCode).Distinct().ToListAsync();
            var allUsers = await _context.Users.Select(u => u.UserName).Distinct().ToListAsync();

            // Apply Search Filter
            if (!string.IsNullOrEmpty(searchTerm)) {
                searchTerm = searchTerm.ToLower();
                query = query.Where(b =>
                    b.Description.ToLower().Contains(searchTerm) ||
                    (b.Customer != null && b.Customer.CustomerCode.ToLower().Contains(searchTerm)) ||
                    (b.Job != null && b.Job.Number.ToLower().Contains(searchTerm) ||
                    b.Id.ToString().Contains(searchTerm))
                );
            }

            // Apply Customer Filter
            if (!string.IsNullOrEmpty(customerCodeFilter)) {
                query = query.Where(b => b.Customer != null && b.Customer.CustomerCode == customerCodeFilter);
            }

            // Apply Created By Filter
            if (!string.IsNullOrEmpty(createdByFilter)) {
                query = query.Where(b => b.CreatedBy.UserName == createdByFilter);
            }

            // Sorting Logic
            query = sortBy switch {
                "description" => sortOrder == "desc" ? query.OrderByDescending(b => b.Description) : query.OrderBy(b => b.Description),
                "job" => sortOrder == "desc" ? query.OrderByDescending(b => b.Job.Number) : query.OrderBy(b => b.Job.Number),
                "status" => sortOrder == "desc" ? query.OrderByDescending(b => b.Status) : query.OrderBy(b => b.Status),
                "version" => sortOrder == "desc" ? query.OrderByDescending(b => b.Version) : query.OrderBy(b => b.Version),
                "createdBy" => sortOrder == "desc" ? query.OrderByDescending(b => b.CreatedBy.UserName) : query.OrderBy(b => b.CreatedBy.UserName),
                "updatedAt" => sortOrder == "desc" ? query.OrderByDescending(b => b.UpdatedAt) : query.OrderBy(b => b.UpdatedAt),
                "bomNumber" => sortOrder == "desc" ? query.OrderByDescending(b => b.Id) : query.OrderBy(b => b.Id),
                _ => query.OrderByDescending(b => b.CreatedAt) // Default: Latest first
            };

            var bomList = await query.Select(b => new BOMViewModel {
                Id = b.Id,
                Description = b.Description,
                JobNumber = b.Job != null ? b.Job.Number : "N/A", // Handle no job
                CustomerCode = b.Customer != null ? b.Customer.CustomerCode : "N/A", // Display customer if no job
                CreatedBy = b.CreatedBy.UserName,
                Status = b.Status.ToString(),
                Version = b.Version,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToListAsync();

            var viewModel = new BOMViewModel {
                BOMs = bomList,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortOrder = sortOrder,
                CustomerCodeFilter = customerCodeFilter,
                CreatedByFilter = createdByFilter,
                AvailableCustomers = allCustomerCodes,
                AvailableUsers = allUsers
            };

            return View(viewModel);
        }

        // GET: Create a new BOM
        [Authorize]
        public async Task<IActionResult> Create() {
            var viewModel = new CreateBOMViewModel {
                AvailableJobs = await _context.Jobs.OrderBy(j => j.Number).ToListAsync(),
                AvailableCustomers = await _context.Customers.OrderBy(c => c.CustomerCode).ToListAsync()
            };
            return View(viewModel);
        }

        // POST: Create a new BOM
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBOMViewModel model) {
            if (!ModelState.IsValid) {
                model.AvailableJobs = await _context.Jobs.OrderBy(j => j.Number).ToListAsync();
                model.AvailableCustomers = await _context.Customers.OrderBy(c => c.CustomerCode).ToListAsync();
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            var newBOM = new BOM {
                Description = model.Description,
                JobId = model.JobId,
                CustomerId = model.CustomerId,
                Status = model.Status,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Version = 1.0m
            };

            _context.BOMs.Add(newBOM);
            await _context.SaveChangesAsync();

            TempData["Success"] = "BOM created successfully.";
            return RedirectToAction("Index");
        }

        // GET: Edit BOM
        public async Task<IActionResult> Edit(int id) {
            var bom = await _context.BOMs.FindAsync(id);
            if (bom == null) {
                TempData["Error"] = "BOM not found.";
                return RedirectToAction(nameof(Index));
            }

            // Prevent editing if BOM is Approved
            if (bom.Status == BOMStatus.Approved) {
                TempData["Error"] = "Approved BOMs cannot be edited.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new EditBOMViewModel {
                Id = bom.Id,
                Description = bom.Description,
                JobId = bom.JobId,
                CustomerId = bom.CustomerId,
                Status = bom.Status,
                AvailableJobs = await _context.Jobs.OrderBy(j => j.Number).ToListAsync(),
                AvailableCustomers = await _context.Customers.OrderBy(c => c.CustomerCode).ToListAsync()
            };

            return View(viewModel);
        }


        // POST: Edit BOM
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditBOMViewModel model) {
            if (!ModelState.IsValid) {
                model.AvailableJobs = await _context.Jobs.OrderBy(j => j.Number).ToListAsync();
                model.AvailableCustomers = await _context.Customers.OrderBy(c => c.CustomerCode).ToListAsync();
                return View(model);
            }

            var existingBOM = await _context.BOMs.FindAsync(id);
            if (existingBOM == null) return NotFound();

            existingBOM.Description = model.Description;
            existingBOM.JobId = model.JobId;
            existingBOM.CustomerId = model.CustomerId;
            existingBOM.Status = model.Status;
            existingBOM.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "BOM updated successfully.";
            return RedirectToAction("Index");
        }

        // POST: Delete BOM
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            var bom = await _context.BOMs
                .Include(b => b.BOMItems)
                .Include(b => b.RFQs)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bom == null) {
                TempData["Error"] = "BOM not found.";
                return RedirectToAction(nameof(Index));
            }

            if (bom.RFQs.Any()) {
                TempData["Error"] = "This BOM is used in RFQs and cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            try {
                // Delete associated BOM Items first (cascade delete)
                _context.BOMItems.RemoveRange(bom.BOMItems);
                _context.BOMs.Remove(bom);
                await _context.SaveChangesAsync();

                TempData["Success"] = "BOM deleted successfully.";
            } catch (Exception ex) {
                TempData["Error"] = "Error deleting BOM: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Clone BOM
        [HttpPost]
        public async Task<IActionResult> Clone(int id) {
            var existingBOM = await _context.BOMs
                .Include(b => b.BOMItems)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBOM == null) {
                TempData["Error"] = "BOM not found.";
                return RedirectToAction(nameof(Index));
            }

            var clonedBOM = new BOM {
                Description = existingBOM.Description + " (Copy)",
                CustomerId = existingBOM.CustomerId,
                JobId = existingBOM.JobId,
                UserId = existingBOM.UserId,
                Status = BOMStatus.Draft, // Reset to Draft
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.BOMs.Add(clonedBOM);
            await _context.SaveChangesAsync();

            // Clone Items
            foreach (var item in existingBOM.BOMItems) {
                var newItem = new BOMItem {
                    BOMId = clonedBOM.Id,
                    PartId = item.PartId,
                    Quantity = item.Quantity
                };
                _context.BOMItems.Add(newItem);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "BOM cloned successfully.";
            return RedirectToAction(nameof(Index));
        }

    }
}


