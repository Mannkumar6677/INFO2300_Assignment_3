using System.ComponentModel.DataAnnotations;

namespace BOMLink.Models {
    public class Supplier {
        #region Properties
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a supplier name.")]
        public string Name { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }

        [EmailAddress(ErrorMessage = "Please a valid supplier email.")]
        [Required(ErrorMessage = "Please enter a supplier email.")]
        public string ContactEmail { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }

        // Navigation properties for SupplierManufacturers
        public ICollection<SupplierManufacturer> SupplierManufacturers { get; set; }
        #endregion
    }
}
