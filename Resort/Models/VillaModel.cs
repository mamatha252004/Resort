using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resort.Models
{
    public class VillaModel
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Price per Night")]
        public double Price { get; set; }
        public int sqft { get; set; }
        public int Occupancy { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }

        [Display(Name="Image Url")]
        public string? ImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public static List<VillaModel> GetVillas()
        {
            // Return an empty list of villas
            return new List<VillaModel>();
        }
        [ValidateNever]
        public IEnumerable<Amenity>? VillaAmenity { get; set; }
        [NotMapped]
        public bool IsAvailable { get; set; } = true;

    }
}
