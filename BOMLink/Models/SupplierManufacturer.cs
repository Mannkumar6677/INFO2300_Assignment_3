using System.ComponentModel.DataAnnotations;

namespace BOMLink.Models {
    public class SupplierManufacturer {
        #region Properties
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select at list one supplier.")]
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        [Required(ErrorMessage = "Please select at list one manufacturer.")]
        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        #endregion
    }
}
