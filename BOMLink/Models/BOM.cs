using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOMLink.Models {
    public class BOM {
        #region Properties
        public int Id { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        public string Description { get; set; }

        // Assigned User (Automatically Captured from Logged-In User)
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        // Navigation properties for BOMItems
        public ICollection<BOMItem> BOMItems { get; set; } = new List<BOMItem>();
        #endregion
    }
}
