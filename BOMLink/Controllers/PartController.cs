using BOMLink.Data;
using BOMLink.Models;
using BOMLink.ViewModels;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace BOMLink.Controllers {
    [Authorize]
    public class PartController : Controller {
        private readonly BOMLinkContext _context;

        public PartController(BOMLinkContext context) {
            _context = context;
        }

        // GET: Part
        public async Task<IActionResult> Index(string searchTerm, string selectedManufacturer, string sortBy, string sortOrder) {
            var parts = _context.Parts
                .Include(p => p.Manufacturer)
                .AsQueryable();

            // Filtering by search term (Part Number, Description, or Manufacturer)
            if (!string.IsNullOrEmpty(searchTerm)) {
                parts = parts.Where(p =>
                    p.PartNumber.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm) ||
                    p.Manufacturer.Name.Contains(searchTerm));
            }

            // Filtering by Manufacturer
            if (!string.IsNullOrEmpty(selectedManufacturer)) {
                parts = parts.Where(p => p.Manufacturer.Name == selectedManufacturer);
            }

            // Sorting logic
            parts = sortBy switch {
                "partnumber" => sortOrder == "desc" ? parts.OrderByDescending(p => p.PartNumber) : parts.OrderBy(p => p.PartNumber),
                "description" => sortOrder == "desc" ? parts.OrderByDescending(p => p.Description) : parts.OrderBy(p => p.Description),
                "manufacturer" => sortOrder == "desc" ? parts.OrderByDescending(p => p.Manufacturer.Name) : parts.OrderBy(p => p.Manufacturer.Name),
                _ => parts.OrderBy(p => p.PartNumber) // Default sorting by Part Number
            };

            var viewModel = new PartViewModel {
                Parts = await parts.ToListAsync(),
                Manufacturers = await _context.Manufacturers.ToListAsync(),
                SearchTerm = searchTerm,
                SelectedManufacturer = selectedManufacturer,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            return View(viewModel);
        }


        // GET: Part/Create
        [HttpGet]
        public IActionResult Create() {
            var viewModel = new PartFormViewModel {
                Manufacturers = _context.Manufacturers.ToList() // Load manufacturers for dropdown
            };

            return View(viewModel);
        }

        // POST: Part/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PartFormViewModel viewModel) {
            if (!ModelState.IsValid) {
                viewModel.Manufacturers = _context.Manufacturers.ToList(); // Reload dropdown
                return View(viewModel);
            }

            // Ensure part number is unique
            if (_context.Parts.Any(p => p.PartNumber == viewModel.PartNumber)) {
                TempData["Error"] = "Part number must be unique.";
                return View(viewModel);
            }

            var part = new Part {
                PartNumber = viewModel.PartNumber,
                Description = viewModel.Description,
                Labour = viewModel.Labour,
                Unit = viewModel.Unit,
                ManufacturerId = viewModel.ManufacturerId
            };

            _context.Parts.Add(part);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Part/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            var part = await _context.Parts.FindAsync(id);
            if (part == null) {
                return NotFound();
            }

            var viewModel = new PartFormViewModel {
                Id = part.Id,  // Using integer ID now
                PartNumber = part.PartNumber,
                Description = part.Description,
                ManufacturerId = part.ManufacturerId,
                Unit = part.Unit,
                Labour = part.Labour,
                Manufacturers = _context.Manufacturers.ToList() // Load manufacturers for dropdown
            };

            return View(viewModel);
        }

        // POST: Part/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PartFormViewModel viewModel) {
            if (id != viewModel.Id) return NotFound();

            // Check if the new part number is already taken by another part
            bool partNumberExists = _context.Parts.Any(p => p.PartNumber == viewModel.PartNumber && p.Id != id);
            if (partNumberExists) {
                TempData["Error"] = "Part number must be unique.";
                viewModel.Manufacturers = _context.Manufacturers.ToList(); // Reload manufacturer dropdown
                return View(viewModel);
            }

            if (!ModelState.IsValid) {
                viewModel.Manufacturers = _context.Manufacturers.ToList(); // Reload manufacturer dropdown
                return View(viewModel);
            }

            var part = await _context.Parts.FindAsync(id);
            if (part == null) return NotFound();

            // Update part properties
            part.PartNumber = viewModel.PartNumber;
            part.Description = viewModel.Description;
            part.ManufacturerId = viewModel.ManufacturerId;
            part.Unit = viewModel.Unit;
            part.Labour = viewModel.Labour;

            try {
                _context.Update(part);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Part updated successfully.";
            } catch (DbUpdateConcurrencyException) {
                if (!_context.Parts.Any(e => e.Id == id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // Part/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            var part = await _context.Parts.FindAsync(id);
            if (part == null) {
                TempData["Error"] = "Part not found.";
                return RedirectToAction(nameof(Index));
            }

            try {
                _context.Parts.Remove(part);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Part deleted successfully.";
            } catch (Exception ex) {
                TempData["Error"] = "Error deleting part: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Part/ExportToCSV
        public IActionResult ExportToCSV() {
            var parts = _context.Parts.Include(p => p.Manufacturer).ToList();

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Id,PartNumber,Description,Manufacturer,Unit,Labour");

            foreach (var part in parts) {
                csvBuilder.AppendLine($"{part.Id},{part.PartNumber},{part.Description},{part.Manufacturer?.Name},{part.Unit},{part.Labour}");
            }

            return File(Encoding.UTF8.GetBytes(csvBuilder.ToString()), "text/csv", "Parts.csv");
        }

        // GET: Part/ExportToExcel
        public IActionResult ExportToExcel() {
            var parts = _context.Parts.Include(p => p.Manufacturer).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Parts");

            // Headers
            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Part Number";
            worksheet.Cells[1, 3].Value = "Description";
            worksheet.Cells[1, 4].Value = "Manufacturer";
            worksheet.Cells[1, 5].Value = "Unit";
            worksheet.Cells[1, 6].Value = "Labour";

            int row = 2;
            foreach (var part in parts) {
                worksheet.Cells[row, 1].Value = part.Id;
                worksheet.Cells[row, 2].Value = part.PartNumber;
                worksheet.Cells[row, 3].Value = part.Description;
                worksheet.Cells[row, 4].Value = part.Manufacturer?.Name;
                worksheet.Cells[row, 5].Value = part.Unit;
                worksheet.Cells[row, 6].Value = part.Labour;
                row++;
            }

            using var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Parts.xlsx");
        }

        // GET: Part/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file) {
            if (file == null || file.Length == 0) {
                TempData["Error"] = "Please select a file to upload.";
                return RedirectToAction(nameof(Index));
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            int importedCount = 0;

            try {
                importedCount = fileExtension switch {
                    ".csv" => await ImportCsv(file),
                    ".xlsx" => await ImportExcel(file),
                    _ => throw new Exception("Invalid file format. Please upload a CSV or Excel file.")
                };

                if (TempData["Error"] == null) {
                    TempData[importedCount > 0 ? "Success" : "Info"] = importedCount > 0
                    ? $"{importedCount} part(s) imported successfully."
                    : "No new parts were imported (duplicates may have been skipped).";
                }
            } catch (Exception ex) {
                TempData["Error"] = "Error importing file: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Import CSV file
        private async Task<int> ImportCsv(IFormFile file) {
            int count = 0;

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) {
                HeaderValidated = null,
                MissingFieldFound = null
            });

            // Read header row
            csv.Read();
            csv.ReadHeader();

            // Validate required columns
            if (csv.HeaderRecord == null || !csv.HeaderRecord.Contains("PartNumber") || !csv.HeaderRecord.Contains("Description") || !csv.HeaderRecord.Contains("Manufacturer") || !csv.HeaderRecord.Contains("Unit") || !csv.HeaderRecord.Contains("Labour")) {
                TempData["Error"] = "Invalid format: Column headers must contain 'PartNumber', 'Description', 'Manufacturer', 'Unit', and 'Labour'.";
                return 0;
            }

            var manufacturers = _context.Manufacturers.ToList(); // Cache manufacturers

            // Process each row
            while (csv.Read()) {
                string partNumber = csv.GetField("PartNumber")?.Trim();
                string description = csv.GetField("Description")?.Trim();
                string manufacturerName = csv.GetField("Manufacturer")?.Trim();
                string unitText = csv.GetField("Unit")?.Trim();
                decimal labour = decimal.Parse(csv.GetField("Labour"));

                // Find Manufacturer
                var manufacturer = manufacturers.FirstOrDefault(m => m.Name == manufacturerName);
                if (manufacturer == null) continue; // Skip if manufacturer doesn't exist

                // Parse Unit
                if (!Enum.TryParse(unitText, out UnitType unit)) continue;

                // Ensure data is valid
                if (!string.IsNullOrEmpty(partNumber) && !string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(unit.ToString()) && labour >= 0 && !_context.Parts.Any(p => p.PartNumber == partNumber)) {
                    _context.Parts.Add(new Part {
                        PartNumber = partNumber,
                        Description = description,
                        ManufacturerId = manufacturer.ManufacturerId,
                        Unit = unit,
                        Labour = labour
                    });
                    count++;
                }
            }

            await _context.SaveChangesAsync();
            return count;
        }

        // Import Excel file
        private async Task<int> ImportExcel(IFormFile file) {
            int count = 0;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            if (worksheet.Dimension.Columns < 5) {
                TempData["Error"] = "Invalid format: The file must have at least five columns: 'PartNumber', 'Description', 'Manufacturer', 'Unit', 'Labour'.";
                return 0;
            }

            // Validate headers
            string partNumberHeader = worksheet.Cells[1, 2].Text.Trim().ToLower();
            string descriptionHeader = worksheet.Cells[1, 3].Text.Trim().ToLower();
            string manufacturerHeader = worksheet.Cells[1, 4].Text.Trim().ToLower();
            string unitHeader = worksheet.Cells[1, 5].Text.Trim().ToLower();
            string labourHeader = worksheet.Cells[1, 6].Text.Trim().ToLower();

            if (partNumberHeader != "part number" || descriptionHeader != "description" || manufacturerHeader != "manufacturer" || unitHeader != "unit" || labourHeader != "labour") {
                TempData["Error"] = "Invalid format: Column headers must include 'PartNumber', 'Description', 'Manufacturer', 'Unit', 'Labour'.";
                return 0;
            }

            var manufacturers = _context.Manufacturers.ToList(); // Cache manufacturers

            for (int row = 2; row <= worksheet.Dimension.Rows; row++) {
                string partNumber = worksheet.Cells[row, 2].Text.Trim();
                string description = worksheet.Cells[row, 3].Text.Trim();
                string manufacturerName = worksheet.Cells[row, 4].Text.Trim();
                string unitText = worksheet.Cells[row, 5].Text.Trim();
                decimal labour = decimal.Parse(worksheet.Cells[row, 6].Text);

                var manufacturer = manufacturers.FirstOrDefault(m => m.Name == manufacturerName);
                if (manufacturer == null) continue; // Skip if manufacturer not found

                // Parse Unit
                if (!Enum.TryParse(unitText, out UnitType unit)) continue;

                if (!string.IsNullOrEmpty(partNumber) && !string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(unit.ToString()) && labour >= 0 && !_context.Parts.Any(p => p.PartNumber == partNumber)) {
                    _context.Parts.Add(new Part {
                        PartNumber = partNumber,
                        Description = description,
                        ManufacturerId = manufacturer.ManufacturerId,
                        Unit = unit,
                        Labour = labour
                    });
                    count++;
                }
            }

            await _context.SaveChangesAsync();
            return count;
        }

    }
}
