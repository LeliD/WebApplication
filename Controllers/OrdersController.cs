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
using WebApplicationIceCreamProject.Services;

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
            //Check the order has products (the cart isn't empty)
            var orderItems = await _db.ShoppingCartItems.Where(item => item.CartId == CartController.ShoppingCartId).ToListAsync();
            if (!orderItems.Any())
                return RedirectToAction("Checkout", new { order = JsonSerializer.Serialize(order) });
            //Bind order's products to the entity order
            order.Products=orderItems;
            //For Checkout function,in case not valid
            string orderJson = JsonSerializer.Serialize(order);
         
            if (ModelState.IsValid)
            {
                
                // Call the address verification service
                var isAddressValid = await VerifyAddressAsync(order.City, order.Street);
                if (!isAddressValid)
                {
                    ModelState.AddModelError(string.Empty, "The provided street or city does not exist.");
                    return RedirectToAction("Checkout", new { order = orderJson });
                }

                //Call the weather service
                var weatherDataResponse = await GetWeatherDataAsync(order.City);
                if (weatherDataResponse != null)
                {
                    // Update the FeelsLike and Humidity properties of the order based on weather data
                    order.FeelsLike = weatherDataResponse.FeelsLike;
                    order.Humidity = weatherDataResponse.Humidity;
                }
                else
                {
                    // Handle the case where weather data could not be retrieved
                    ModelState.AddModelError(string.Empty, "Failed to retrieve weather data for the specified city.");
                    return RedirectToAction("Checkout", new { order = orderJson });
                }

                //Update the IsItHoliday property of the order by calling the holiday check service
                order.IsItHoliday = CheckHoliday(order.Date).Result;
                // Set the Date property to the current date and time.
                order.Date = DateTime.Now;
                // Set the Day property to the current day of week.
                order.Day = (Models.DayOfWeek)DateTime.Now.DayOfWeek;

                _db.Add(order);
                await _db.SaveChangesAsync();
               
                foreach (var item in orderItems)
                {
                    item.OrderId = order.Id;
                }
                await _db.SaveChangesAsync();
             
                // After the order is successfully placed, set the TempData flag.
                TempData["OrderCompleted"] = true;
                
                //go to ThankYou
                return RedirectToAction("ThankYou");
            }

            //Order isn't valid
            return RedirectToAction("Checkout", new { order = orderJson });

        }
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
            return RedirectToAction("Index", "Cart"); // Redirect to an error or fallback action
        }


        public IActionResult ThankYou()
        {
            return View();
        }
        // This method verifies if the provided address exists
        private async Task<bool> VerifyAddressAsync(string city, string street)
        {
            try
            {
                var apiUrl = $"https://localhost:7099/Address?city={Uri.EscapeDataString(city)}&street={Uri.EscapeDataString(street)}";

                var apiService = new ApiService("bf185c7f-1a4e-4662-88c5-fa118a244bda"); // API key
                var addressVerificationResponse = await apiService.GetApiResponseAsync<bool>(apiUrl);

                return addressVerificationResponse;
            }
            catch (Exception ex)
            {
                // Handle the exception, log, or return false in case of an error
                return false;
            }
        }

        // This method retrieves weather data for the specified city
        private async Task<WeatherDataResponse> GetWeatherDataAsync(string city)
        {
            try
            {
                var weatherApiUrl = $"https://localhost:7099/Weather?city={Uri.EscapeDataString(city)}"; // Update the API URL

                var weatherService = new ApiService("412b7cc1240b95fe425658b14e486cf9"); // API key
                var weatherDataResponse = await weatherService.GetApiResponseAsync<WeatherDataResponse>(weatherApiUrl);

                return weatherDataResponse;
            }
            catch (Exception ex)
            {
                // Handle the exception, log, or return null in case of an error
                return null;
            }
        }

        // This method checks if the date falls on a holiday
        private async Task<bool> CheckHoliday(DateTime date)
        {
            try
            {
                var dateApiUrl = $"https://localhost:7099/DateCheck?y={date.Year}&m={date.Month}&d={date.Day}";

                var dateService = new ApiService("111DFWDV"); // API key
                var isItHolidayResponse = dateService.GetApiResponseAsync<bool>(dateApiUrl).Result;

                // Assuming isItHolidayResponse is a boolean indicating if it's a holiday
                return isItHolidayResponse;
                
            }
            catch (Exception ex)
            {
                // Handle the exception or log it
                return false;
            }
        }

  

        //GET: Orders

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
      
    }
}
