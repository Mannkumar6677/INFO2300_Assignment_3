using BOMLink.ViewModels.BOMItemViewModels;

namespace BOMLink.ViewModels.BOMViewModels {
    public class BOMDetailsViewModel {
        #region Properties
        public int BOMId { get; set; }
        public string BOMNumber { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string JobNumber { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<BOMItemViewModel> BOMItems { get; set; } = new List<BOMItemViewModel>();
        #endregion
    }
}