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
        private readonly IceCreamContext _context;
        public HomeController(/*ILogger<HomeController> logger*/IceCreamContext context)
        {
            //_logger = logger;
            _context=context;   
        }

        public async Task<IActionResult> Index()
        {
            var iceCreamList = new List<IceCream>();
            var iceCream1 = await _context.IceCream
              .FirstOrDefaultAsync(m => m.Id == 2);
            var iceCream2 = await _context.IceCream
                .FirstOrDefaultAsync(m => m.Id == 3);
            var iceCream3 = await _context.IceCream
              .FirstOrDefaultAsync(m => m.Id == 5);
            var iceCream4 = await _context.IceCream
              .FirstOrDefaultAsync(m => m.Id == 7);
            if (iceCream1 != null)
                iceCreamList.Add(iceCream1);
            if (iceCream2 != null)
                iceCreamList.Add(iceCream2);
            if (iceCream3 != null)
                iceCreamList.Add(iceCream3);
            if (iceCream4 != null)
                iceCreamList.Add(iceCream4);
            return View(iceCreamList);
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