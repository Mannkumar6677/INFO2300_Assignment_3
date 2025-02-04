﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOMLink.Models {
    public class Part {
        #region Properties
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a part number.")]
        [StringLength(50, ErrorMessage = "Part number must be less than 30 characters.")]
        public string PartNumber { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter a labour quantity.")]
        public decimal Labour { get; set; }

        [Required(ErrorMessage = "Please enter a unit.")]
        public UnitType Unit { get; set; }

        [Required(ErrorMessage = "Please enter a manufacturer.")]
        [ForeignKey("Manufacturer")]
        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        #endregion
    }

    #region Enum
    public enum UnitType {
        each,
        ten,
        twenty,
        fifty,
        hundred,
        meter,
        foot,
    }
    #endregion
}
