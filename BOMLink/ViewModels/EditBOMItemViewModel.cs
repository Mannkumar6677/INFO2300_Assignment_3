using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BOMLink.Models;

namespace BOMLink.ViewModels {
    public class EditBOMItemViewModel {
        #region Properties
        [Required]
        public int Id { get; set; }

        [Required]
        public int BOMId { get; set; }

        [Required]
        public int PartId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        public List<Part> AvailableParts { get; set; } = new List<Part>();
        #endregion
    }
}
