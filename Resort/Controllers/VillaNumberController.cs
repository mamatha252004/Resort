using Microsoft.AspNetCore.Mvc;
using Resort.DbContext;
using System.Collections.Generic;
using System.Linq;
using Resort.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Resort.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace Resort.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VillaNumberController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VillaNumberController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<VillaNumber> villaNumbers = _context.VillaNumbers.Include(u => u.Villa).ToList();
            return View(villaNumbers);
        }
        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaNumber= new VillaNumber(),
                VillaList = _context.Villas.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(villaNumberVM);
        }
        [HttpPost]
        public IActionResult Create(VillaNumberVM villa)
        {
            bool roomNumberExists = _context.VillaNumbers.Any(x => x.Villa_Number == villa.VillaNumber.Villa_Number);

            if (ModelState.IsValid && !roomNumberExists)
            {
                _context.VillaNumbers.Add(villa.VillaNumber);
                _context.SaveChanges();
                TempData["success"] = "The villa Number has been Created successfully";
                return RedirectToAction("Index");
            }
            if (roomNumberExists)
            {
                TempData["error"] = "The villa number already exists";
            }
            villa.VillaList = _context.Villas.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(villa);
        }
        public IActionResult Update(int villaNumber)
        {
            VillaNumberVM villaNumberVM = new()
            {
                
                VillaList = _context.Villas.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = _context.VillaNumbers.FirstOrDefault(x => x.Villa_Number == villaNumber)

            };
            if (villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error","Home");
            }
            return View(villaNumberVM);
        }
        [HttpPost]
        public IActionResult Update(VillaNumberVM villaNumberVM)
        {
            
            if (ModelState.IsValid)
            {
               
                {
                    _context.VillaNumbers.Update(villaNumberVM.VillaNumber);
                    _context.SaveChanges();
                    TempData["success"] = "The villa Number has been Updated successfully";
                    return RedirectToAction("Index");
                }
                
                
            }
            villaNumberVM.VillaList = _context.Villas.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(villaNumberVM);
        }
        public IActionResult Delete(int villaNumber)
        {
            VillaNumberVM villaNumberVM = new()
            {

                VillaList = _context.Villas.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = _context.VillaNumbers.FirstOrDefault(x => x.Villa_Number == villaNumber)

            };
            if (villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }
        [HttpPost]
        public IActionResult Delete(VillaNumberVM villaNumberVM)
        {
            VillaNumber? objFromDb = _context.VillaNumbers.FirstOrDefault(x => x.Villa_Number == villaNumberVM.VillaNumber.Villa_Number);
            if (objFromDb is not null)
            {
                _context.VillaNumbers.Remove(objFromDb);
                _context.SaveChanges();
                TempData["success"] = "The villa Number has been deleted successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Villa Number could not be deleted";
            return View();
        }
    }
}
