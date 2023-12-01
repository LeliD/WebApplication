using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApplicationIceCreamProject.Data;
using Microsoft.Extensions.Configuration;
namespace WebApplicationIceCreamProject.Controllers
{

	//public class KafkaController : Controller
	//{
	//	private readonly KafkaConsumer kafkaConsumer;
	//	private CancellationTokenSource cancellationTokenSource;

	//	private readonly string connectionString = "AdminContext";

	//	public KafkaController()
	//	{
	//		kafkaConsumer = new KafkaConsumer(connectionString);
	//		cancellationTokenSource = new CancellationTokenSource();
	//	}

	// Your action methods
	public class KafkaController : Controller
	{
		private readonly KafkaConsumer kafkaConsumer;
		private CancellationTokenSource cancellationTokenSource;
		private readonly string connectionString;

		public KafkaController(IConfiguration configuration)
		{
			connectionString = configuration["ConnectionStrings:AdminContext"]; // Retrieve the connection string
			kafkaConsumer = new KafkaConsumer(connectionString); // Pass the connection string to the KafkaConsumer constructor
			cancellationTokenSource = new CancellationTokenSource();
		}





		//public class KafkaController : Controller

		//{
		//	private readonly KafkaConsumer kafkaConsumer;
		//private CancellationTokenSource cancellationTokenSource;
		//private readonly IceCreamContext dbContext;


		//public KafkaController(IceCreamContext dbContext)
		//{
		//	this.dbContext = dbContext;
		//	this.kafkaConsumer = new KafkaConsumer(dbContext);
		//	this.cancellationTokenSource = new CancellationTokenSource();
		//}

		//	private readonly KafkaConsumer kafkaConsumer;
		//	private readonly Func<IceCreamContext> _contextFactory;
		//	private CancellationTokenSource cancellationTokenSource;

		//	public KafkaController(Func<IceCreamContext> contextFactory)
		//	{
		//		_contextFactory = contextFactory;
		//		kafkaConsumer = new KafkaConsumer(_contextFactory);
		//		cancellationTokenSource = new CancellationTokenSource();
		//	}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult StartConsumer()
		{
			cancellationTokenSource = new CancellationTokenSource();
			Task.Run(() => kafkaConsumer.StartConsumerAsync(cancellationTokenSource.Token));
			return RedirectToAction("Index");
		}

		public IActionResult StopConsumer()
		{
			cancellationTokenSource.Cancel();
			return RedirectToAction("Index");
		}
		[HttpPost]
		
		public IActionResult StartSimulator()
		{
			try
			{
				cancellationTokenSource = new CancellationTokenSource();
				// Start Kafka consumer in the background
				Task.Run(() =>
				{
					try
					{
						kafkaConsumer.StartConsumerAsync(cancellationTokenSource.Token).GetAwaiter().GetResult();
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Kafka Consumer Error: {ex.Message}");
					}
					finally
					{
						cancellationTokenSource.Dispose(); // Dispose the CancellationTokenSource
					}
				});
				return Ok(); // Return a success response
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to start Kafka Simulator: {ex.Message}");
			}
		}

		//public IActionResult StartSimulator()
		//{
		//	try
		//	{
		//		cancellationTokenSource = new CancellationTokenSource();
		//		// Start Kafka consumer in the background
		//		Task.Run(() => kafkaConsumer.StartConsumerAsync(cancellationTokenSource.Token));
		//		return Ok(); // Return a success response
		//	}
		//	catch (Exception ex)
		//	{
		//		return BadRequest($"Failed to start Kafka Simulator: {ex.Message}");
		//	}
		//}
	}


}
//using Microsoft.AspNetCore.Mvc;
//using WebApplicationIceCreamProject.Services;

//public class KafkaController : ControllerBase
//{
//	private readonly KafkaConsumerService kafkaConsumerService;

//	public KafkaController(KafkaConsumerService kafkaConsumerService)
//	{
//		this.kafkaConsumerService = kafkaConsumerService;
//	}

//	[HttpPost("/start-consumer")]
//	public IActionResult StartConsumer()
//	{
//		kafkaConsumerService.StartConsumer();
//		return Ok("Consumer started.");
//	}

//	[HttpPost("/stop-consumer")]
//	public IActionResult StopConsumer()
//	{
//		kafkaConsumerService.StopConsumer();
//		return Ok("Consumer stopped.");
//	}
//}

