using Microsoft.AspNetCore.Mvc;
using Resort.DbContext;
using System.Collections.Generic;
using System.Linq;
using Resort.Models;
using Microsoft.AspNetCore.Authorization;


namespace Resort.Controllers
{
    [Authorize(Roles ="Admin")]
    public class VillaController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ApplicationDbContext _context;
        public VillaController(ApplicationDbContext context ,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<VillaModel> villas = _context.Villas.ToList();
            return View(villas);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(VillaModel villa)
        {
            if (villa.Name == villa.Description)
            {
                ModelState.AddModelError("Name", "Description and Name cannot be same.");
            }

            if (ModelState.IsValid)
            {
                if(villa.Image != null)
                {
                    string filename = Guid.NewGuid().ToString() + "_" + Path.GetExtension(villa.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath,@"Images\VillaImage");

                    using var fileStream = new FileStream(Path.Combine(imagePath, filename), FileMode.Create);
                    villa.Image.CopyTo(fileStream);

                    villa.ImageUrl= @"\Images\VillaImage\" + filename;
                }
                else
                {
                    villa.ImageUrl = "https://dummyimage.com/600x400/fff";
                }
                _context.Villas.Add(villa);
                _context.SaveChanges();
                TempData["success"] = "The villa has been Created successfully";
                return RedirectToAction("Index");
            }
            return View(villa);

        }
        public IActionResult Update(int villaId)
        {
            VillaModel? villa = _context.Villas.FirstOrDefault(x=> x.Id == villaId);
           
            if (villa == null)
            {
                return RedirectToAction("Error","Home");
            }
            return View(villa);
        }
        [HttpPost]
        public IActionResult Update(VillaModel villa)
        {
            if (ModelState.IsValid && villa.Id>0)
            {
                if (villa.Image != null)
                {
                    string filename = Guid.NewGuid().ToString() + "_" + Path.GetExtension(villa.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"Images\VillaImage");

                    if(!string.IsNullOrEmpty(villa.ImageUrl))
                    {
                       var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, villa.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using var fileStream = new FileStream(Path.Combine(imagePath, filename), FileMode.Create);
                    villa.Image.CopyTo(fileStream);

                    villa.ImageUrl = @"\Images\VillaImage\" + filename;
                }               
                _context.Villas.Update(villa);
                _context.SaveChanges();
                TempData["success"] = "The villa has been updated successfully";
                return RedirectToAction("Index");
            }
            return View();

        }
        public IActionResult Delete(int villaId)
        {
            VillaModel? villa = _context.Villas.FirstOrDefault(x => x.Id == villaId);

            if (villa == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villa);
        }
        [HttpPost]
        public IActionResult Delete(VillaModel villa)
        {
            VillaModel? objFromDb = _context.Villas.FirstOrDefault(x => x.Id==villa.Id);
            if (objFromDb is not null)
            {
                if (!string.IsNullOrEmpty(objFromDb.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, objFromDb.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _context.Villas.Remove(objFromDb);
                _context.SaveChanges();
                TempData["success"] = "The villa has been deleted successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Villa could not be deleted";
            return View();

        }
    }
}
