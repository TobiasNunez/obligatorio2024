using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using obligatorio2024.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace obligatorio2024.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Obligatorio2024Context _context;

        public HomeController(ILogger<HomeController> logger, Obligatorio2024Context context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var menuItems = _context.Menus.ToList();
            return View(menuItems);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                var menu = new Menu
                {
                    ImagenUrl = $"/images/{fileName}",
                    NombrePlato = "Ejemplo de Plato", // Modifica según necesites
                    Precio = 100.00M // Modifica según necesites
                };

                _context.Menus.Add(menu);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View();
        }
    }
}