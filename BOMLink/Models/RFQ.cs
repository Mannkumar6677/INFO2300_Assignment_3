namespace BOMLink.Models {
    public class RFQ {
        #region Properties
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        // Navigation properties for RFQItems
        public ICollection<RFQItem> RFQItems { get; set; } = new List<RFQItem>();
        #endregion
    }
}
