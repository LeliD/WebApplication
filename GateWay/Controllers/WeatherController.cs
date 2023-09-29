using GateWay.Models.GateWay;
using GateWay.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace GateWay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {

        private readonly ILogger<WeatherController> _logger;

        public WeatherController(ILogger<WeatherController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeather")]
        public WeatherToReturn Get(string city)
        {

            // Deserialize the JSON response into an instance of the MyData class
            var response = new WebClient().DownloadString($"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid=412b7cc1240b95fe425658b14e486cf9");
            Root2 myDeserializedClass = JsonConvert.DeserializeObject<Root2>(response);
            // return "Feels like: "+((myDeserializedClass.main.feels_like)*(1)).ToString() + " Humidity: " + myDeserializedClass.main.humidity.ToString();
            WeatherToReturn weatherToReturn = new WeatherToReturn();
            weatherToReturn.FeelsLike = myDeserializedClass.main.feels_like;
            weatherToReturn.Humidity = myDeserializedClass.main.humidity;
            return weatherToReturn;
        }


    }
}
