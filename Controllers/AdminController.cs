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
using WebApplicationIceCreamProject.Services;
using Microsoft.AspNetCore.Hosting;

namespace WebApplicationIceCreamProject.Controllers
{
    public class AdminController : Controller
    {

        private readonly IceCreamContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(IceCreamContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        

        // GET: Admin
        public async Task<IActionResult> Index()
        {
              return _context.IceCream != null ? 
                          View(await _context.IceCream.ToListAsync()) :
                          Problem("Entity set 'AdminContext.IceCream'  is null.");
        }
        // GET: Orders/Edit/5
        public async Task<IActionResult> EditOrder(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(int id, [Bind("Id,FirstName,LastName,PhoneNumber,Email,Street,City,HouseNumber,Products,Date,FeelsLike,Humidity,IsItHoliday,Day,Total")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(OrdersList));
            }
            return View(order);
        }
        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> OrderDetails(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            var orderItems = await _context.ShoppingCartItems.Where(item => item.OrderId == id).ToListAsync();
            //Bind order's products to the entity order
            order.Products = orderItems;

            return View(order);
        }
        // GET: Orders/Delete/5
        public async Task<IActionResult> DeleteOrder(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("DeleteOrder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'IceCreamContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                var orderItems = await _context.ShoppingCartItems.Where(item => item.OrderId == order.Id).ToListAsync();
                foreach(var item in orderItems)
                {
                    _context.ShoppingCartItems.Remove(item);
                    await _context.SaveChangesAsync();
                }
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(OrdersList));
        }

        public IActionResult OpenPage()
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
        public async Task<IActionResult> Create([Bind("Id,Name,Image_URL,Description,Price")] IceCream iceCream, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Handle the case where the user uploads an image from the file explorer
                // Generate a unique filename for the image
                string uniqueFileName = $"{Guid.NewGuid().ToString()}_{DateTime.Now.Ticks}{Path.GetExtension(ImageFile.FileName)}.jpg";

                // Define the path where you want to save the image inside the wwwroot / images folder
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", uniqueFileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                // Update the Image_URL property with the new path (relative to wwwroot)
                iceCream.Image_URL = $"{uniqueFileName}";

                _context.Add(iceCream);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else if (!string.IsNullOrEmpty(iceCream.Image_URL))
            {
                // Handle the case where the user provides an image URL
                var imageUrl = iceCream.Image_URL;
                var isImageValid = await CheckIfImageIsValidAsync(imageUrl);

                if (!isImageValid)
                {
                    ModelState.AddModelError(string.Empty, "The provided image URL is not valid or does not contain a recognized ice cream image.");
                    return View(iceCream);
                }

                // Generate a unique filename for the image
                string uniqueFileName = $"{Guid.NewGuid().ToString()}_{DateTime.Now.Ticks}.jpg";

                // Define the path where you want to save the image inside the wwwroot / images folder
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", uniqueFileName);

                // Download and save the image to the specified path
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(imageUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var contentStream = await response.Content.ReadAsStreamAsync();

                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            await contentStream.CopyToAsync(fileStream);
                        }

                        // Update the Image_URL property with the new path (relative to wwwroot)
                        iceCream.Image_URL = $"{uniqueFileName}";

                        _context.Add(iceCream);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please provide either an image URL or upload an image.");
                return View(iceCream);
            }

            // If ModelState is not valid, return the view with validation errors
            return View(iceCream);
        }


        private async Task<bool> CheckIfImageIsValidAsync(string imageUrl)
        {
            //Implement your logic to check if the image is valid(e.g., using your ApiService).
            //Return true if the image is valid, and false if it's not.
            var apiService = new ApiService("acc_e0d8ec2b70f224f");
            var isIceCream = await apiService.GetApiResponseAsync<bool>($"http://gatewayapi.somee.com/Image?imageUrl={Uri.EscapeDataString(imageUrl)}");
            return isIceCream;
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
            // Check if date1 is greater than date2, and swap them if necessary
            if (date1 > date2)
            {
                DateTime temp = date1;
                date1 = date2;
                date2 = temp;
            }
            int counter = 1;
            List<OrdersDate> temps = new List<OrdersDate>();
            for (DateTime i = date1; i <= date2; i = i.AddDays(1))
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
