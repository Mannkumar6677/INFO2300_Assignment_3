using BOMLink.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BOMLink.ViewModels {
    public class JobViewModel {
        // Used for listing (Index)
        public List<Job> Jobs { get; set; } = new();

        public string SearchTerm { get; set; }
        public string SelectedCustomer { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string Action { get; set; }

        //// Used for Create/Edit
        //public int Id { get; set; }

        //[Required(ErrorMessage = "Please enter a job number.")]
        //public string Number { get; set; }

        //[Required(ErrorMessage = "Please enter a description.")]
        //public string Description { get; set; }

        //[Required(ErrorMessage = "Please select a customer.")]
        //public int CustomerId { get; set; }

        //[Required(ErrorMessage = "Please enter a start date.")]
        //public DateTime StartDate { get; set; } = DateTime.Now;

        //public string? ContactName { get; set; }

        //[Required(ErrorMessage = "Please select a status.")]
        //public JobStatus Status { get; set; } = JobStatus.Pending;

        //public int UserId { get; set; } // Auto-assigned from logged-in user

        // Dropdown options
        public List<Customer> Customers { get; set; } = new();
    }
}
