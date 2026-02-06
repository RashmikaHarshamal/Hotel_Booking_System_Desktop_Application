using System.ComponentModel.DataAnnotations;

namespace Project
{
    public class Choice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NumberOfBeds { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Type { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Cost { get; set; }

        [Required]
        [MaxLength(255)]
        public byte[]? ImagePath { get; set; }
    }
}