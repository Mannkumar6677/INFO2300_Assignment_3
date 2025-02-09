using System.Collections.Generic;

namespace BOMLink.ViewModels {
    public class BOMItemViewModel {
        #region Properties
        public int Id { get; set; }
        public int BOMId { get; set; }
        public string BOMNumber { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        // Part Details
        public string PartNumber { get; set; }
        public string PartDescription { get; set; }
        public int Quantity { get; set; }

        // List of BOM Items for display
        public List<BOMItemViewModel> BOMItems { get; set; } = new List<BOMItemViewModel>();
        #endregion
    }
}
