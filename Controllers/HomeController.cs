using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplicationIceCreamProject.Data;
using WebApplicationIceCreamProject.Models;

namespace WebApplicationIceCreamProject.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly AdminContext _context;
        public HomeController(/*ILogger<HomeController> logger*/AdminContext context)
        {
            //_logger = logger;
            _context=context;   
        }

        public IActionResult Index()
        {
            return View();
        }
        // GET: Home/Shop
        public async Task<IActionResult> Shop()
        {
            return _context.IceCream != null ?
                        View(await _context.IceCream.ToListAsync()) :
                        Problem("Entity set 'AdminContext.IceCream'  is null.");
        }
        // GET: Home/About
        public IActionResult About()
        {
            return View();
        }

        // GET: Home/Contact
        public IActionResult Contact()
        {
            return View();
        }

        // GET:Home/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.IceCream == null)
            {
                return NotFound();
            }

            var iceCream = await _context.IceCream
                .FirstOrDefaultAsync(m => m.Id == id);
            if (iceCream == null)
            {
                return NotFound();
            }

            return View(iceCream);
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
    }
}