using System.ComponentModel.DataAnnotations;

namespace BOMLink.Models {
    public class RFQItem {
        #region Properties
        public int RFQItemId { get; set; }
        public int RFQId { get; set; }
        public RFQ RFQ { get; set; }
        [Required(ErrorMessage = "Please select a part number.")]
        public int PartId { get; set; }
        public Part Part { get; set; }
        [Required(ErrorMessage = "Please enter a quantity.")]
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int LeadTime { get; set; }
        #endregion
    }
}
