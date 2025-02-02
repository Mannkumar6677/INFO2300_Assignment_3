using BOMLink.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BOMLink.Controllers {
    [Authorize]
    public class DashboardController : Controller {
        private readonly BOMLinkContext _context;

        public DashboardController(BOMLinkContext context) {
            _context = context;
        }

        public IActionResult Index() {
            var dashboardData = new {
                TotalBOMs = _context.BOMs.Count(),
                OpenRFQs = _context.RFQs.Count(r => r.StatusId == 1),
                PendingPOs = _context.POs.Count(p => p.StatusId == 1),
                PackingSlipMismatches = _context.POItems.Count(pi => pi.QuantityReceived < pi.Quantity)
            };

            return View(dashboardData);
        }
    }
}
