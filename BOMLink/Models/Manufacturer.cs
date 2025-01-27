using System.ComponentModel.DataAnnotations;

namespace BOMLink.Models {
    public class Manufacturer {
        #region Properties
        public int ManufacturerId { get; set; }

        [Required(ErrorMessage = "Please enter a manufacturer name.")]
        public string Name { get; set; }


        // Navigation properties for SupplierManufacturers
        public ICollection<SupplierManufacturer> SupplierManufacturers { get; set; }
        #endregion
    }
}
