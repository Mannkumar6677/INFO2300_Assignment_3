using System.ComponentModel.DataAnnotations;

namespace BOMLink.Models {
    public class Supplier {
        #region Properties
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter an unique supplier name.")]
        public string Name { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }

        [EmailAddress(ErrorMessage = "Please a valid supplier email.")]
        [Required(ErrorMessage = "Please enter a supplier email.")]
        public string ContactEmail { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }

        [Required(ErrorMessage = "Please enter an unique supplier code.")]
        public string SupplierCode { get; set; }

        // Navigation properties for SupplierManufacturers
        public List<SupplierManufacturer> SupplierManufacturers { get; set; } = new();
        #endregion
    }
}
