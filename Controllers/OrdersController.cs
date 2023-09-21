using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationIceCreamProject.Data;
using WebApplicationIceCreamProject.Models;

namespace WebApplicationIceCreamProject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IceCreamContext _db;

        public OrdersController(IceCreamContext context)
        {
            _db = context;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder([Bind("Id,FirstName,LastName,PhoneNumber,Email,Street,City,HouseNumber,Products,Date,FeelsLike,Humidity,IsItHoliday,Day,Total")] Order order)
        {
            if (ModelState.IsValid)
            {
                _db.Add(order);
                await _db.SaveChangesAsync();
                // After the order is successfully placed, set the TempData flag.
                TempData["OrderCompleted"] = true;

                return RedirectToAction("ThankYou");
            }
            return View(order);
        }
        //public IActionResult Checkout(Order order)
        //{
        //    return View(order);
        //}
        public IActionResult Checkout(string order)
        {
            // Deserialize the order object
            Order orderObject = JsonSerializer.Deserialize<Order>(order);

            // Ensure the order has the necessary data, e.g., products
            if (orderObject != null && orderObject.Products != null && orderObject.Products.Any())
            {
                return View(orderObject);
            }

            // Handle the case where the order is not found or doesn't have products
            return RedirectToAction("Shop","Home"); // Redirect to an error or fallback action
        }


        public IActionResult ThankYou()
        {
            return View();
        }

        // GET: Orders
        //public async Task<IActionResult> Index()
        //{
        //      return _context.Order != null ? 
        //                  View(await _context.Order.ToListAsync()) :
        //                  Problem("Entity set 'IceCreamContext.Order'  is null.");
        //}

        //// GET: Orders/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Order == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Order
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(order);
        //}

        //// GET: Orders/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Orders/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,PhoneNumber,Email,Street,City,HouseNumber,IceCream,Date,FeelsLike,Humidity,Pressure")] Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(order);
        //}

        //// GET: Orders/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Order == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Order.FindAsync(id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(order);
        //}

        //// POST: Orders/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name,PhoneNumber,Email,Street,City,HouseNumber,IceCream,Date,FeelsLike,Humidity,Pressure")] Order order)
        //{
        //    if (id != order.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(order);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OrderExists(order.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(order);
        //}

        //// GET: Orders/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Order == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Order
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(order);
        //}

        //// POST: Orders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Order == null)
        //    {
        //        return Problem("Entity set 'IceCreamContext.Order'  is null.");
        //    }
        //    var order = await _context.Order.FindAsync(id);
        //    if (order != null)
        //    {
        //        _context.Order.Remove(order);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool OrderExists(int id)
        //{
        //  return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
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

                foreach (var item in _db.Order)
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
