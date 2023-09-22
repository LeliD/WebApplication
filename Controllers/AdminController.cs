using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationIceCreamProject.Data;
using WebApplicationIceCreamProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;

namespace WebApplicationIceCreamProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly IceCreamContext _context;

        public AdminController(IceCreamContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
              return _context.IceCream != null ? 
                          View(await _context.IceCream.ToListAsync()) :
                          Problem("Entity set 'AdminContext.IceCream'  is null.");
        }

        public async Task<IActionResult> OpenPage()
        {
            return View();
        }

        // GET: Admin/Details/5
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

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Image_URL,Description,Price")] IceCream iceCream)
        {
            if (ModelState.IsValid)
            {
                _context.Add(iceCream);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(iceCream);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.IceCream == null)
            {
                return NotFound();
            }

            var iceCream = await _context.IceCream.FindAsync(id);
            if (iceCream == null)
            {
                return NotFound();
            }
            return View(iceCream);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Image_URL,Description,Price")] IceCream iceCream)
        {
            if (id != iceCream.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(iceCream);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IceCreamExists(iceCream.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(iceCream);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.IceCream == null)
            {
                return Problem("Entity set 'AdminContext.IceCream'  is null.");
            }
            var iceCream = await _context.IceCream.FindAsync(id);
            if (iceCream != null)
            {
                _context.IceCream.Remove(iceCream);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IceCreamExists(int id)
        {
          return (_context.IceCream?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> OrdersList()
        {
            return _context.Order != null ?
                        View(await _context.Order.ToListAsync()) :
                        Problem("Entity set 'IceCreamContext.Order'  is null.");
        }

        public IActionResult Graph()
        {
            return View();
        }
        public IActionResult ShowGraph(DateTime date1, DateTime date2)
        {
            int counter = 1;
            List<OrdersDate> temps = new List<OrdersDate>();
            for (DateTime i = date1; i < date2; i = i.AddDays(1))//לבדוק שהדייט הראשון קטן מהשני!!!
            {
                OrdersDate t = new OrdersDate { Id = counter++, Day = i.Day, Month = i.Month, Counter = 0 };

                foreach (var item in _context.Order)
                {
                    if (item.Date.Day == i.Day && item.Date.Month == i.Month)
                        t.Counter++;//the number of orders in this date
                }
                temps.Add(t);

            }
            return View(temps);
        }
    }
}
