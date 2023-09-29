using GateWay.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using static GateWay.Models.Dates;
namespace GateWay.Models
{
    [ApiController]
    [Route("[controller]")]
    public class DateCheckController : ControllerBase
    {
        private readonly ILogger<DateCheckController> _logger;

        public DateCheckController(ILogger<DateCheckController> logger)
        {
            _logger = logger;
        }


        [HttpGet(Name = "GetDateCheck")]
        public bool GetDates(string y, string m, string d)
        {
            DateTime date = new DateTime(int.Parse(y), int.Parse(m), int.Parse(d));
            List<string> nonParashatEvents = new List<string>();

            // Calculate the start and end dates of the week
            DateTime startOfWeek = date.AddDays(-(int)date.DayOfWeek);
            DateTime endOfWeek = startOfWeek.AddDays(6);

            for (DateTime currentDate = startOfWeek; currentDate <= endOfWeek; currentDate = currentDate.AddDays(1))
            {
                // Construct the Hebcal API URL for the current date
                string currentDateString = currentDate.ToString("yyyy-MM-dd");
                string apiUrl = $"https://www.hebcal.com/converter?cfg=json&date={currentDateString}&g2h=1&strict=1";

                try
                {
                    // Send a GET request to Hebcal API
                    string response = new WebClient().DownloadString(apiUrl);
                    Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(response);
                    if (myDeserializedClass != null && myDeserializedClass.events != null)
                    {
                        // Filter non-"Parashat" events for the current date and add them to the list
                        List<string> nonParashatEventsForDate = myDeserializedClass.events
                        .Where(e => e != null && !e.StartsWith("Parashat"))
                        .ToList();

                        nonParashatEvents.AddRange(nonParashatEventsForDate);
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions or errors
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            // Return true if there are non-Parashat events, otherwise return false
            return nonParashatEvents.Any();
        }




    }
}

