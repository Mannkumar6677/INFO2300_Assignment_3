using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BOMLink.Models;
using BOMLink.Validations;

namespace BOMLink.ViewModels {
    [EnsureEitherJobOrCustomerValidation]
    public class CreateBOMViewModel {
        #region Properties
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public int? JobId { get; set; } // Optional Job Selection

        public int? CustomerId { get; set; } // Optional Customer Selection

        [Required]
        public BOMStatus Status { get; set; } = BOMStatus.Draft; // Default to Draft

        // Dropdown Options
        public List<Job> AvailableJobs { get; set; } = new List<Job>();
        public List<Customer> AvailableCustomers { get; set; } = new List<Customer>();
        #endregion
    }
}
