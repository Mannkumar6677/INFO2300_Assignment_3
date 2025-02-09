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
using BOMLink.ViewModels.BOMItemViewModels;
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

        // GET: Create or Edit BOM Item
        [HttpGet]
        public async Task<IActionResult> CreateOrEdit(int? id, int bomId) {
            var bom = await _context.BOMs
                .Include(b => b.BOMItems)
                .FirstOrDefaultAsync(b => b.Id == bomId);

            if (bom == null) return NotFound();

            var existingPartIds = bom.BOMItems.Select(bi => bi.PartId).ToList();

            var viewModel = new CreateOrEditBOMItemViewModel {
                BOMId = bom.Id,
                BOMNumber = $"BOM-{bom.Id:D6}",
                ExistingPartIds = existingPartIds,
                AvailableParts = await _context.Parts
                    .OrderBy(p => p.PartNumber)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.PartNumber} - {p.Description}" })
                    .ToListAsync()
            };

            if (id.HasValue) {
                var bomItem = await _context.BOMItems.FirstOrDefaultAsync(bi => bi.Id == id);
                if (bomItem == null) return NotFound();

                viewModel.Id = bomItem.Id;
                viewModel.PartId = bomItem.PartId;
                viewModel.Quantity = bomItem.Quantity;
                viewModel.Notes = bomItem.Notes;
            }

            return View(viewModel);
        }

        // POST: Create or Edit BOM Item
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(CreateOrEditBOMItemViewModel model) {
            ModelState.Remove("BOMNumber");

            bool partExists = await _context.BOMItems
                .AnyAsync(bi => bi.BOMId == model.BOMId && bi.PartId == model.PartId && bi.Id != model.Id);

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

            if (model.Id == 0) // Create new BOMItem
            {
                var newItem = new BOMItem {
                    BOMId = model.BOMId,
                    PartId = model.PartId,
                    Quantity = model.Quantity,
                    Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes
                };

                _context.BOMItems.Add(newItem);
            } else // Edit existing BOMItem
              {
                var existingBOMItem = await _context.BOMItems.FindAsync(model.Id);
                if (existingBOMItem == null) return NotFound();

                existingBOMItem.PartId = model.PartId;
                existingBOMItem.Quantity = model.Quantity;
                existingBOMItem.Notes = model.Notes;
            }

            await _context.SaveChangesAsync();

            var bom = await _context.BOMs.FindAsync(model.BOMId);
            if (bom != null) {
                bom.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "BOM item saved successfully.";
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
