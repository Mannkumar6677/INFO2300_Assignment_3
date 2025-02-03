using BOMLink.Models;

namespace BOMLink.ViewModels {
    public class ManufacturerViewModel {
        #region Properties
        public List<Manufacturer> Manufacturers { get; set; }
        public string SearchTerm { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        #endregion
    }
}
