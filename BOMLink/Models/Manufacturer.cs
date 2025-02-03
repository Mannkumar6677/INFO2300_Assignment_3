using System.ComponentModel.DataAnnotations;

namespace BOMLink.Models {
    public class Manufacturer {
        #region Properties
        public int ManufacturerId { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(50, ErrorMessage = "Supplier name cannot exceed 50 characters.")]
        public string Name { get; set; }

        // Navigation properties for SupplierManufacturers
        public ICollection<SupplierManufacturer> SupplierManufacturers { get; set; } = new List<SupplierManufacturer>();
        #endregion
    }
}
