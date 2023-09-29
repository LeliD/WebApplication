using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System;
using RestSharp;
using static GateWay.Models.Image;
using Newtonsoft.Json;
using System.Net;

namespace GateWay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<ImageController> _logger;

        public ImageController(ILogger<ImageController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetImage")]
        public bool Get(string imageUrl)
        {
            string apiKey = "acc_e0d8ec2b70f224f";
            string apiSecret = "fac9d53c7e3257537b61fcc44aab9c3e";
           
            string basicAuthValue = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", apiKey, apiSecret)));

            var client = new RestClient("https://api.imagga.com/v2/tags");
            var request = new RestRequest();
            request.AddParameter("image_url", imageUrl);
            request.AddHeader("Authorization", String.Format("Basic {0}", basicAuthValue));

            RestResponse response = client.Execute(request);
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(response.Content);
            if (myDeserializedClass?.result.tags != null)
            {
                foreach (var element in myDeserializedClass.result.tags)
                {
                    if (string.Equals(element.tag.en, "ice cream", StringComparison.OrdinalIgnoreCase))
                        return true; //Ice cream
                }
            }
            return false; //Not ice cream
        }
    }
    
   
}
