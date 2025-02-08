using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BOMLink.Models {
    public class RFQ {
        #region Properties
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        public int BOMId { get; set; }
        public BOM BOM { get; set; }

        // Assigned User (Automatically Captured from Logged-In User)
        [Required]
        public string UserId { get; set; } // IdentityUser uses string for ID

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        // Navigation properties for RFQItems
        public ICollection<RFQItem> RFQItems { get; set; } = new List<RFQItem>();
        #endregion
    }
}
