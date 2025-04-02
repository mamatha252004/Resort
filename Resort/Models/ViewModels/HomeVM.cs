using Resort.Models.ViewModels;

namespace Resort.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<VillaModel>? VillaList { get; set; }
        public DateOnly? CheckInDate { get; set; }
        public DateOnly? CheckOutDate { get; set; }
        public int? Nights { get; set; }
    }
}
