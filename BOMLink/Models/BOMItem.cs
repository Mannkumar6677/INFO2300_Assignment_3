namespace BOMLink.Models {
    public class BOMItem {
        #region Properties
        public int Id { get; set; }
        public int BOMId { get; set; }
        public BOM BOM { get; set; }
        public int PartId { get; set; }
        public Part Part { get; set; }
        public int Quantity { get; set; }
        #endregion
    }
}
