using BOMLink.Data;
using BOMLink.Models;
using BOMLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BOMLink.Controllers {
    [Authorize]
    public class JobController : Controller {
        private readonly BOMLinkContext _context;

        public JobController(BOMLinkContext context) {
            _context = context;
        }

        // GET: Job
        public async Task<IActionResult> Index(string searchTerm, string sortBy, string sortOrder) {
            var jobs = _context.Jobs.Include(j => j.Customer).AsQueryable();

            // Filtering by customer name
            if (!string.IsNullOrEmpty(searchTerm)) {
                jobs = jobs.Where(j => j.Customer.Name.Contains(searchTerm));
            }

            // Sorting logic
            jobs = sortBy switch {
                "customer" => sortOrder == "desc" ? jobs.OrderByDescending(j => j.Customer.Name) : jobs.OrderBy(j => j.Customer.Name),
                "status" => sortOrder == "desc" ? jobs.OrderByDescending(j => j.Status) : jobs.OrderBy(j => j.Status),
                "date" => sortOrder == "desc" ? jobs.OrderByDescending(j => j.StartDate) : jobs.OrderBy(j => j.StartDate),
                _ => jobs.OrderBy(j => j.Number)
            };

            var viewModel = new JobViewModel {
                Jobs = await jobs.ToListAsync(),
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            return View(viewModel);
        }

        // GET: Job/Create
        [HttpGet]
        public IActionResult Create() {
            var viewModel = new JobFormViewModel {
                Customers = _context.Customers.ToList() // Load customers for dropdown
            };

            return View(viewModel);
        }

        // POST: Job/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobFormViewModel viewModel) {
            if (!ModelState.IsValid) {
                viewModel.Customers = _context.Customers.ToList(); // Reload dropdown
                return View(viewModel);
            }

            var job = new Job {
                Number = viewModel.Number,
                Description = viewModel.Description,
                CustomerId = viewModel.CustomerId,
                StartDate = viewModel.StartDate,
                ContactName = viewModel.ContactName,
                Status = viewModel.Status,
                UserId = 1 // Replace with logged-in user later
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Job/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) {
                return NotFound();
            }

            var viewModel = new JobFormViewModel {
                Id = job.Id,
                Number = job.Number,
                Description = job.Description,
                CustomerId = job.CustomerId,
                StartDate = job.StartDate,
                ContactName = job.ContactName,
                Status = job.Status,
                Customers = _context.Customers.ToList()
            };

            return View(viewModel);
        }

        // POST: Job/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobFormViewModel viewModel) {
            if (!ModelState.IsValid) {
                viewModel.Customers = _context.Customers.ToList(); // Reload dropdown
                return View(viewModel);
            }

            var job = await _context.Jobs.FindAsync(viewModel.Id);
            if (job == null) return NotFound();

            job.Number = viewModel.Number;
            job.Description = viewModel.Description;
            job.CustomerId = viewModel.CustomerId;
            job.StartDate = viewModel.StartDate;
            job.ContactName = viewModel.ContactName;
            job.Status = viewModel.Status;

            _context.Update(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Job/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) {
                TempData["Error"] = "Job not found.";
                return RedirectToAction(nameof(Index));
            }

            try {
                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Job deleted successfully.";
            } catch (Exception ex) {
                TempData["Error"] = "Error deleting job: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
