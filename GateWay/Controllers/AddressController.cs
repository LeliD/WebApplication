using static GateWay.Models.Address;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace GateWay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressController : ControllerBase
    {

        private readonly ILogger<AddressController> _logger;

        public AddressController(ILogger<AddressController> logger)
        {
            _logger = logger;
        }

        
        [HttpGet(Name = "GetAddress")]
        public bool Get(string city, string street)
        {

            // Deserialize the JSON response into an instance of the MyData class
            var response = new WebClient().DownloadString("https://data.gov.il/api/3/action/datastore_search?resource_id=bf185c7f-1a4e-4662-88c5-fa118a244bda&limit=130000");
            Root root = JsonConvert.DeserializeObject<Root>(response);

            // Check if the given city and street exist in the Addresses list
            if (root?.result.records != null)
            {
                return root.result.records.Any(record => record.city_name.TrimStart().TrimEnd() == city && record.street_name.TrimStart().TrimEnd() == street);
            }

            return false; // Address not found

        }

    }
}
