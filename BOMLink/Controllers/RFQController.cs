using BOMLink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BOMLink.Controllers {
    public class RFQController : Controller {
        public IActionResult Index() {
            return View();
        }

        //// RFQController - GET: Create RFQ
        //public async Task<IActionResult> Create() {
        //    var viewModel = new CreateRFQViewModel {
        //        AvailableBOMs = await _context.BOMs
        //            .Where(b => b.Status != BOMStatus.Draft) // Exclude Draft BOMs
        //        .OrderBy(b => b.Id)
        //            .ToListAsync(),
        //        AvailableSuppliers = await _context.Suppliers.OrderBy(s => s.Name).ToListAsync()
        //    };
        //    return View(viewModel);
        //}

    }
}
