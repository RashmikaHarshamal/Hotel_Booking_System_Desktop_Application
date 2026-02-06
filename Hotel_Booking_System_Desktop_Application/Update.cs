using System;
using System.ComponentModel.DataAnnotations;

namespace Project
{
    public class Update
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public byte[]? ProfileImagePath { get; set; }

        [Required]
        public int? NumberOfBeds { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [Required]
        public string? Type { get; set; }

        public bool IsBooked { get; set; } // Add this property
        public bool IsConfirmed { get; set; } // Add this property
    }
}