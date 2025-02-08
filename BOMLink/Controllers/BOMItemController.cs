using BOMLink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BOMLink.Controllers {
    public class BOMItemController : Controller {
        public IActionResult Index() {
            return View();
        }

        //public async Task<IActionResult> ValidateItem(int id) {
        //    var item = await _context.BOMItems.FindAsync(id);
        //    if (item == null) return NotFound();

        //    item.IsChecked = true;
        //    await _context.SaveChangesAsync();

        //    // Check if all items in BOM are validated
        //    var bom = await _context.BOMs
        //        .Include(b => b.BOMItems)
        //        .FirstOrDefaultAsync(b => b.Id == item.BOMId);

        //    if (bom.BOMItems.All(i => i.IsChecked)) {
        //        bom.Status = BOMStatus.Approved;
        //    } else {
        //        bom.Status = BOMStatus.PendingApproval;
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("ViewItems", new { id = bom.Id });
        //}

        //public async Task<IActionResult> DeleteItem(int id) {
        //    var item = await _context.BOMItems
        //        .Include(i => i.BOM)
        //        .FirstOrDefaultAsync(i => i.Id == id);

        //    if (item == null) return NotFound();

        //    if (item.BOM.Status == BOMStatus.Approved) {
        //        TempData["Error"] = "Cannot delete items from an approved BOM.";
        //        return RedirectToAction("ViewItems", new { id = item.BOMId });
        //    }

        //    _context.BOMItems.Remove(item);
        //    await _context.SaveChangesAsync();
        //    TempData["Success"] = "Item deleted successfully.";
        //    return RedirectToAction("ViewItems", new { id = item.BOMId });
        //}

    }
}
