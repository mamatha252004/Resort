using Microsoft.AspNetCore.Mvc;
using Resort.DbContext;
using System.Collections.Generic;
using System.Linq;
using Resort.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Resort.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace Resort.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AmenityController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AmenityController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Amenity> amenities = _context.Amenities.Include(u => u.Villa).ToList();
            return View(amenities);
        }
        [HttpGet]
        public IActionResult Create()
        {
            AmenityVM amenityVM = new()
            {
                Amenity = new Amenity
                {
                    Name = string.Empty
                },

                VillaList = _context.Villas.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(amenityVM);
        }
        [HttpPost]
        public IActionResult Create(AmenityVM amenityVM)
        {
            if (ModelState.IsValid)
            {
                _context.Amenities.Add(amenityVM.Amenity);
                _context.SaveChanges();
                TempData["success"] = "The amenity has been Created successfully";
                return RedirectToAction("Index");
            }
            amenityVM.VillaList = _context.Villas.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            return View(amenityVM);
        }
        [HttpGet]
        public async Task<IActionResult> Update(int amenityId)
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _context.Villas.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                Amenity = await _context.Amenities.FirstOrDefaultAsync(x => x.Id == amenityId)
            };
            if (amenityVM.Amenity == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(amenityVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AmenityVM amenityVM)
        {
            if (ModelState.IsValid)
            {
                var amenity = await _context.Amenities.FirstOrDefaultAsync(x => x.Id == amenityVM.Amenity.Id);
                if (amenity != null)
                {
                    // Update the amenity with new values
                    amenity.Name = amenityVM.Amenity.Name;
                    amenity.Description = amenityVM.Amenity.Description;
                    amenity.VillaId = amenityVM.Amenity.VillaId;

                    _context.Amenities.Update(amenity);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "The Amenity has been updated successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Amenity not found");
                }
                
            }

            amenityVM.VillaList = _context.Villas.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(amenityVM);
        }
        [HttpGet]
        public IActionResult Delete(int amenityId)
        {
            AmenityVM amenityVM = new()
            {
                Amenity = _context.Amenities.FirstOrDefault(x => x.Id == amenityId),
                VillaList = _context.Villas.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            if (amenityVM.Amenity == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Delete(AmenityVM amenityVM)
        {
            Amenity objFromDb = _context.Amenities.FirstOrDefault(x => x.Id == amenityVM.Amenity.Id);
            if (objFromDb != null)
            {
                _context.Amenities.Remove(objFromDb);
                _context.SaveChanges();
                TempData["success"] = "The amenity has been deleted successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Amenity could not be deleted";
            return View(amenityVM);
        }

    }
}