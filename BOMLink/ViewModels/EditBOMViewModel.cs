using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BOMLink.Models;
using BOMLink.Validations;

namespace BOMLink.ViewModels {
    [EnsureEitherJobOrCustomerValidation]
    public class EditBOMViewModel {
        #region Properties
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        public string Description { get; set; }

        // Job Selection (Nullable)
        public int? JobId { get; set; }
        public List<Job> AvailableJobs { get; set; } = new List<Job>();

        // Customer Selection (Nullable)
        public int? CustomerId { get; set; }
        public List<Customer> AvailableCustomers { get; set; } = new List<Customer>();

        // Status Dropdown
        [Required]
        public BOMStatus Status { get; set; }

        // Audit Fields
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        #endregion
    }
}
