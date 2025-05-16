using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WheelDeal.Models
{
    [Table("cars")]
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        [MaxLength(50)]
        public string Brand { get; set; }

        [Required]
        [MaxLength(50)]
        public string Model { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [Column("condition")]
        [MaxLength(10)]
        public string Condition { get; set; } // Use "new" or "used"

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }

        public int? Mileage { get; set; }

        [MaxLength(50)]
        public string FuelType { get; set; }

        [MaxLength(50)]
        public string Transmission { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        public string Description { get; set; }

        [MaxLength(255)]
        public string ImagePath { get; set; }

        [MaxLength(10)]
        public string Status { get; set; } = "available";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
