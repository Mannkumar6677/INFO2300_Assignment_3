using System.ComponentModel.DataAnnotations;

namespace BOMLink.Models {
    public class Manufacturer {
        #region Properties
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(50, ErrorMessage = "Supplier name cannot exceed 50 characters.")]
        public string Name { get; set; }

        // Navigation properties for SupplierManufacturers
        public List<SupplierManufacturer> SupplierManufacturers { get; set; } = new();
        #endregion
    }
}
