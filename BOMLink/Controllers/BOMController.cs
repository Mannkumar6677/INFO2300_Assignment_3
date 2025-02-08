using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOMLink.Data;
using BOMLink.Models;
using BOMLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BOMLink.Controllers {
    [Authorize]
    public class BOMController : Controller {
        private readonly BOMLinkContext _context;

        public BOMController(BOMLinkContext context) {
            _context = context;
        }

        // GET: List all BOMs with search and sorting
        public async Task<IActionResult> Index(string searchTerm, string sortBy, string sortOrder, string customerCodeFilter, string createdByFilter) {
            var query = _context.BOMs
                .Include(b => b.Job)
                .Include(b => b.CreatedBy)
                .AsQueryable();

            // Collect Unique Customers & Users for Filtering
            var allCustomerCodes = await _context.Customers.Select(c => c.CustomerCode).Distinct().ToListAsync();
            var allUsers = await _context.Users.Select(u => u.UserName).Distinct().ToListAsync();

            // Apply Search Filter
            if (!string.IsNullOrEmpty(searchTerm)) {
                searchTerm = searchTerm.ToLower();
                query = query.Where(b => b.Description.ToLower().Contains(searchTerm) ||
                                         b.Job.Number.ToLower().Contains(searchTerm) ||
                                 (b.Customer != null && b.Customer.CustomerCode.ToLower().Contains(searchTerm)));
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
    }
}
