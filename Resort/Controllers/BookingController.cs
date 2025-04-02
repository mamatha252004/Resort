using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resort.DbContext;
using Resort.Models;
using Resort.Utilities;
using System.Security.Claims;

namespace Resort.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var isAdmin = User.IsInRole("Admin");

            IQueryable<Booking> bookings;
            if (isAdmin)
            {
                bookings = _context.Bookings.Include(b => b.Villa);
            }
            else
            {
                bookings = _context.Bookings.Where(b => b.UserId == userId).Include(b => b.Villa);
            }
            return View(bookings.ToList());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Details(int id)
        {
            var booking = _context.Bookings.Include(b => b.Villa).FirstOrDefault(b => b.Id == id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Approve(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking == null)
            {
                return NotFound();
            }
            booking.Status = "Approved";
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Decline(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking == null)
            {
                return NotFound();
            }
            booking.Status = "Declined";
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Finalize(int villaId, DateOnly checkInDate, int nights)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            var villa = _context.Villas.Include(v => v.VillaAmenity).FirstOrDefault(x => x.Id == villaId);
            if (villa == null)
            {
                return NotFound("Villa not found.");
            }
            Booking booking = new()
            {
                VillaId = villaId,
                Villa = villa,
                CheckInDate = checkInDate,
                Nights = nights,
                CheckOutDate = checkInDate.AddDays(nights),
                UserId = userId,
                Phone = user.PhoneNumber,
                Email = user.Email,
                Name = user.Name,
                Status = SD.StatusPending
            };
            booking.TotalCost = booking.Villa.Price * nights;
            return View(booking);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Finalize(Booking booking)
        {
            var villa = _context.Villas.Include(v => v.VillaAmenity).FirstOrDefault(x => x.Id == booking.VillaId);
            booking.TotalCost = villa.Price * booking.Nights;
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;

            _context.Bookings.Add(booking);
            _context.SaveChanges();
            return RedirectToAction(nameof(Payment), new { bookingId = booking.Id });
        }
        [Authorize]
        public IActionResult Payment(int bookingId)
        {
            var booking = _context.Bookings.Include(b => b.Villa).FirstOrDefault(b => b.Id == bookingId);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Payment(Booking booking)
        {
            var existingBooking = _context.Bookings.FirstOrDefault(b => b.Id == booking.Id);
            if(existingBooking == null)
            {
                return NotFound();
            }
            existingBooking.IsPaymentSuccessful = true;
            existingBooking.PaymentDate = DateTime.Now;
            existingBooking.Status = SD.StatusCompleted;
            _context.SaveChanges();

            return RedirectToAction(nameof(BookingConfirmation), new { bookingId = booking.Id });
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            return View(bookingId);
        }
    }
}