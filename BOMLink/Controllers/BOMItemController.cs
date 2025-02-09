//using BOMLink.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace BOMLink.Controllers {
//    public class BOMItemController : Controller {
//        public IActionResult Index() {
//            return View();
//        }

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

//    }
//}

using System.Linq;
using System.Threading.Tasks;
using BOMLink.Data;
using BOMLink.Models;
using BOMLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BOMLink.Controllers {
    [Authorize]
    public class BOMItemController : Controller {
        private readonly BOMLinkContext _context;

        public BOMItemController(BOMLinkContext context) {
            _context = context;
        }

        // GET: List BOM Items for a specific BOM
        public async Task<IActionResult> Index(int bomId) {
            var bom = await _context.BOMs
                .Include(b => b.BOMItems)
                .ThenInclude(bi => bi.Part)
                .FirstOrDefaultAsync(b => b.Id == bomId);

            if (bom == null) return NotFound();

            var viewModel = new BOMItemViewModel {
                BOMId = bom.Id,
                BOMNumber = $"BOM-{bom.Id:D6}",
                Description = bom.Description,
                Status = bom.Status.ToString(),
                BOMItems = bom.BOMItems.Select(bi => new BOMItemViewModel {
                    Id = bi.Id,
                    PartNumber = bi.Part.PartNumber,
                    PartDescription = bi.Part.Description,
                    Quantity = bi.Quantity
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: Create BOM Item
        public async Task<IActionResult> Create(int bomId) {
            var bom = await _context.BOMs
                .Include(b => b.BOMItems)
                .FirstOrDefaultAsync(b => b.Id == bomId);

            if (bom == null) return NotFound();

            var existingPartIds = bom.BOMItems.Select(bi => bi.PartId).ToList();

            var viewModel = new CreateBOMItemViewModel {
                BOMId = bom.Id,
                BOMNumber = $"BOM-{bom.Id:D6}",
                ExistingPartIds = existingPartIds,
                AvailableParts = await _context.Parts
                    .OrderBy(p => p.PartNumber)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.PartNumber} - {p.Description}" })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Create BOM Item
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBOMItemViewModel model) {
            ModelState.Remove("BOMNumber");

            // Check if the part already exists in the BOM
            bool partExists = await _context.BOMItems
                .AnyAsync(bi => bi.BOMId == model.BOMId && bi.PartId == model.PartId);

            if (partExists) {
                ModelState.AddModelError("PartId", "This part has already been added to the BOM.");
            }

            if (!ModelState.IsValid) {
                model.AvailableParts = await _context.Parts
                    .OrderBy(p => p.PartNumber)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.PartNumber} - {p.Description}" })
                    .ToListAsync();
                return View(model);
            }

            var newItem = new BOMItem {
                BOMId = model.BOMId,
                PartId = model.PartId,
                Quantity = model.Quantity,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes
            };

            _context.BOMItems.Add(newItem);
            await _context.SaveChangesAsync();

            // Update BOM updatedAt timestamp
            var bom = await _context.BOMs.FindAsync(model.BOMId);
            if (bom != null) {
                bom.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "BOM item added successfully.";
            return RedirectToAction("Index", new { bomId = model.BOMId });
        }

        // GET: Edit BOM Item
        public async Task<IActionResult> Edit(int id) {
            var bomItem = await _context.BOMItems.Include(bi => bi.BOM).FirstOrDefaultAsync(bi => bi.Id == id);
            if (bomItem == null) return NotFound();

            // Prevent editing Approved BOMs
            if (bomItem.BOM.Status == BOMStatus.Approved) {
                TempData["Error"] = "Cannot edit items in an Approved BOM.";
                return RedirectToAction("Index", new { bomId = bomItem.BOMId });
            }

            var viewModel = new EditBOMItemViewModel {
                Id = bomItem.Id,
                BOMId = bomItem.BOMId,
                PartId = bomItem.PartId,
                Quantity = bomItem.Quantity,
                AvailableParts = await _context.Parts.OrderBy(p => p.PartNumber).ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Edit BOM Item
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBOMItemViewModel model) {
            if (!ModelState.IsValid) {
                model.AvailableParts = await _context.Parts.OrderBy(p => p.PartNumber).ToListAsync();
                return View(model);
            }

            var bomItem = await _context.BOMItems.Include(bi => bi.BOM).FirstOrDefaultAsync(bi => bi.Id == model.Id);
            if (bomItem == null) return NotFound();

            // Prevent editing Approved BOMs
            if (bomItem.BOM.Status == BOMStatus.Approved) {
                TempData["Error"] = "Cannot edit items in an Approved BOM.";
                return RedirectToAction("Index", new { bomId = model.BOMId });
            }

            bomItem.PartId = model.PartId;
            bomItem.Quantity = model.Quantity;
            bomItem.UpdatedAt = DateTime.UtcNow;

            _context.Update(bomItem);
            await _context.SaveChangesAsync();

            TempData["Success"] = "BOM Item updated successfully.";
            return RedirectToAction("Index", new { bomId = model.BOMId });
        }

        // POST: Delete BOM Item
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            var bomItem = await _context.BOMItems.Include(bi => bi.BOM).FirstOrDefaultAsync(bi => bi.Id == id);
            if (bomItem == null) return NotFound();

            // Prevent deleting items in Approved BOMs
            if (bomItem.BOM.Status == BOMStatus.Approved) {
                TempData["Error"] = "Cannot delete items from an Approved BOM.";
                return RedirectToAction("Index", new { bomId = bomItem.BOMId });
            }

            _context.BOMItems.Remove(bomItem);
            await _context.SaveChangesAsync();

            TempData["Success"] = "BOM Item deleted successfully.";
            return RedirectToAction("Index", new { bomId = bomItem.BOMId });
        }

        [HttpGet]
        public async Task<IActionResult> SearchParts(string term) {
            var parts = await _context.Parts
                .Where(p => p.PartNumber.Contains(term) || p.Description.Contains(term))
                .OrderBy(p => p.PartNumber)
                .Select(p => new {
                    id = p.Id,
                    partNumber = p.PartNumber,
                    description = p.Description
                })
                .Take(10) // Limit results for better performance
                .ToListAsync();

            return Json(parts);
        }
    }
}
