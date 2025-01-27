using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BOMLink.Models {
    public class Part {
        #region Properties
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a part number.")]
        [StringLength(50, ErrorMessage = "Part number must be less than 30 characters.")]
        [Key]
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public decimal Labour { get; set; }
        public string Per { get; set; }

        [Required(ErrorMessage = "Please enter a manufacturer.")]
        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        #endregion
    }
}
