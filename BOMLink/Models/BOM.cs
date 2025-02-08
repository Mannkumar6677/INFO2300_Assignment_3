using BOMLink.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BOMLink.Models {
    public class BOM {
        #region Properties
        [Key]
        public int Id { get; set; }

        [ForeignKey("Job")]
        public int? JobId { get; set; }
        public Job? Job { get; set; }

        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "You must select either a Job or a Customer.")]
        [EnsureEitherJobOrCustomerValidation]
        public string ValidationRule => "Ensuring either Job or Customer is selected";

        // Assigned User (Automatically Captured from Logged-In User)
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser CreatedBy { get; set; }

        // Status Tracking
        [Required]
        [EnumDataType(typeof(BOMStatus))]
        public BOMStatus Status { get; set; } = BOMStatus.Draft; // Default to Draft

        // Internal Notes
        [MaxLength(500)]
        public string? Notes { get; set; } // Optional internal comments

        // Version Control
        [Required]
        [Column(TypeName = "decimal(4,1)")]
        public decimal Version { get; set; } = 1.0m; // Start at 1.0

        // Timestamps
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Auto-update when modified

        [NotMapped]
        public string BOMNumber => $"BOM-{Id:D6}";

        // Navigation properties for BOMItems
        public ICollection<BOMItem> BOMItems { get; set; } = new List<BOMItem>();
        public ICollection<RFQ> RFQs { get; set; } = new List<RFQ>();
        public ICollection<PO> POs { get; set; } = new List<PO>();
        #endregion

        #region Methods
        // Increment Version on Modification
        public void IncrementVersion() {
            Version += 0.1m;
        }
        #endregion
    }

    #region Enum
    // Enum for BOM Status
    public enum BOMStatus {
        Draft,
        PendingApproval,
        Approved,
        Rejected
    }
    #endregion
}
