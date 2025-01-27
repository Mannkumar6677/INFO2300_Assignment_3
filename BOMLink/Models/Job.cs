using System.ComponentModel.DataAnnotations;

namespace BOMLink.Models {
    public class Job {
        #region Properties
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a job number.")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter a customer.")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime StartDate { get; set; }
        public string ContactName { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        #endregion
    }
}
