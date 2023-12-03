using System.ComponentModel.DataAnnotations;

namespace WebApplicationIceCreamProject.Models
{
    public class IceCreamPrediction
    {
        [Required]
        public string City { get; set; }

        [Required]
        public string Season { get; set; }

        [Required]
        [Range(0, 100)]
        public float FeelsLike { get; set; }

        [Required]
        [Range(0, 100)]
        public float Humidity { get; set; }

        [Required]
        public string Weekday { get; set; }
        
        public string? PredictedFlavor { get; set; }
    }
}
