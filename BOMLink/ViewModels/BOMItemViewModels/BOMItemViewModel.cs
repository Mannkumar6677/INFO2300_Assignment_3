using System.Collections.Generic;

namespace BOMLink.ViewModels.BOMItemViewModels {
    public class BOMItemViewModel {
        #region Properties
        public int Id { get; set; }
        public int BOMId { get; set; }
        public string BOMNumber { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        // Part Details
        public int PartId { get; set; }
        public string PartNumber { get; set; }
        public string PartDescription { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }

        // List of BOM Items for display
        public List<BOMItemViewModel> BOMItems { get; set; } = new List<BOMItemViewModel>();
        #endregion
    }
}
