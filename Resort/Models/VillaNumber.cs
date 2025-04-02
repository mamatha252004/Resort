using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resort.Models
{
    public class VillaNumber
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        
        public int Villa_Number { get; set; }

        [ForeignKey("VillaModel")]
        public int VillaId { get; set; }
        public VillaModel? Villa { get; set; }
        public string? SpecialDetails { get; set; }
    }
}
