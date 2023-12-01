using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using WebApplicationIceCreamProject.Models;
using WebApplicationIceCreamProject.Data;
using Confluent.Kafka;

namespace WebApplicationIceCreamProject
{

	public class KafkaConsumer
	{
		private readonly IceCreamContext _context;

		public KafkaConsumer(string connectionString)
		{
			var options = new DbContextOptionsBuilder<IceCreamContext>()
				.UseSqlServer(connectionString)
				.Options;
			_context = new IceCreamContext(options);
		}

		// Rest of the class...


		//public KafkaConsumer(string connectionString)
		//{
		//	var options = new DbContextOptionsBuilder<IceCreamContext>()
		//		.UseSqlServer(connectionString)
		//		.Options;
		//	_context = new IceCreamContext(options);
		//}
		public async Task StartConsumerAsync(CancellationToken cancellationToken)
		{
			var config = new ConsumerConfig
			{

				BootstrapServers = "dory.srvs.cloudkafka.com:9094",
				SecurityProtocol = SecurityProtocol.SaslSsl,
				SaslMechanism = SaslMechanism.ScramSha512,
				SaslUsername = "hamvnwgx", // Replace with your SASL username
				SaslPassword = "IjJCR7O5tdftLwExKHMMF3ZBMPKBK4Mh",
				GroupId = "hamvnwgx-group-id",
				AutoOffsetReset = AutoOffsetReset.Earliest,
				EnableAutoCommit = false,
			};

			using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
			{
				consumer.Subscribe("hamvnwgx-orders");

				while (!cancellationToken.IsCancellationRequested)
				{
					try
					{
						var consumeResult = consumer.Consume(cancellationToken);
						var orderJson = consumeResult.Message.Value;

						var order = JsonSerializer.Deserialize<Order>(orderJson);

						Console.WriteLine($"Received order: {order.Id}");
						using (var transaction = await _context.Database.BeginTransactionAsync())
						{
							try
							{
								// Add and save order
								await _context.AddAsync(order);
								await _context.SaveChangesAsync();

								// Commit the transaction
								await transaction.CommitAsync();
							}
							catch (Exception ex)
							{
								// Handle exception
								await transaction.RollbackAsync();
								Console.WriteLine($"Error saving changes to the database: {ex.Message}");
							}
						}

						//await _context.AddAsync(order);
						//var x = await _context.SaveChangesAsync();
						
					}
					catch (OperationCanceledException)
					{
						// Handle cancellation request
					}
					catch (ConsumeException e)
					{
						// Handle the consume exception
						Console.WriteLine($"Error consuming message: {e.Error.Reason}");
					}
				}
			}
		}
	}
}

		//public async Task StartConsumerAsync(CancellationToken cancellationToken)
		//{
		//	var config = new ConsumerConfig
		//	{
		//		BootstrapServers = "dory.srvs.cloudkafka.com:9094",
		//		SecurityProtocol = SecurityProtocol.SaslSsl,
		//		SaslMechanism = SaslMechanism.ScramSha512,
		//		SaslUsername = "hamvnwgx", // Replace with your SASL username
		//		SaslPassword = "IjJCR7O5tdftLwExKHMMF3ZBMPKBK4Mh",
		//		GroupId = "hamvnwgx-group-id",
		//		AutoOffsetReset = AutoOffsetReset.Earliest,
		//		EnableAutoCommit = false,
		//	};

		//	using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
		//	{
		//		consumer.Subscribe("hamvnwgx-orders");

		//		while (!cancellationToken.IsCancellationRequested)
		//		{
		//			try
		//			{
		//				var consumeResult = consumer.Consume(cancellationToken);
		//				var orderJson = consumeResult.Message.Value;

		//				var order = JsonSerializer.Deserialize<Order>(orderJson);

		//				Console.WriteLine($"Received order: {order.Id}");

		//				// Add the received order to the context
		//				await _context.AddAsync(order);
		//				await _context.SaveChangesAsync();
		//			}
		//			catch (OperationCanceledException)
		//			{
		//				// Handle cancellation request
		//			}
		//			catch (ConsumeException e)
		//			{
		//				// Handle the consume exception
		//				Console.WriteLine($"Error consuming message: {e.Error.Reason}");
		//			}
		//		}
		//	}
		//}
	


//using System;
//using Confluent.Kafka;
//using System.Threading.Tasks;
//using System.Threading;
//using System.Text.Json;
//using WebApplicationIceCreamProject.Models;
//using WebApplicationIceCreamProject.Migrations;
//using WebApplicationIceCreamProject.Data;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;

//namespace WebApplicationIceCreamProject
//{
//	public class KafkaConsumer
//	{
//		private readonly string kafkaBroker;
//		private readonly string orderTopicName;
//		private readonly IceCreamContext dbContext;


//		public KafkaConsumer(IceCreamContext dbContext)
//		{
//			this.kafkaBroker = "dory.srvs.cloudkafka.com:9094";
//			this.orderTopicName = "hamvnwgx-orders";
//			this.dbContext = dbContext;
//		}


//		public async Task StartConsumerAsync(CancellationToken cancellationToken)
//		{
//			var config = new ConsumerConfig
//			{
//				BootstrapServers = kafkaBroker,
//				SecurityProtocol = SecurityProtocol.SaslSsl,
//				SaslMechanism = SaslMechanism.ScramSha512,
//				SaslUsername = "hamvnwgx", // Replace with your SASL username
//				SaslPassword = "IjJCR7O5tdftLwExKHMMF3ZBMPKBK4Mh",
//				GroupId = "hamvnwgx-group-id",
//				AutoOffsetReset = AutoOffsetReset.Earliest,
//				EnableAutoCommit = false,
//				// Add any other consumer configuration settings as needed
//			};




//			using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
//			{
//				consumer.Subscribe(orderTopicName);
//				while (!cancellationToken.IsCancellationRequested)
//				{
//					try
//					{
//						using (var context = new IceCreamContext())
//						{

//							var consumeResult = consumer.Consume(cancellationToken);
//							var orderJson = consumeResult.Message.Value;

//							// Deserialize the received order JSON
//							var order = JsonSerializer.Deserialize<Order>(orderJson);

//							// Process the received order
//							Console.WriteLine($"Received order: {order.Id}");

//							await dbContext.AddAsync(order);
//							await dbContext.SaveChangesAsync(); // Commit the changes to the database
//						}
//					}
//					catch (OperationCanceledException)
//					{
//						// Handle cancellation request
//					}
//					catch (ConsumeException e)
//					{
//						// Handle the consume exception
//						Console.WriteLine($"Error consuming message: {e.Error.Reason}");
//					}
//				}
//			}

//			//return Task.CompletedTask;
//		}
//	}

//}
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Text.Json;
//using WebApplicationIceCreamProject.Models;
//using Confluent.Kafka;
//using WebApplicationIceCreamProject.Data;

//namespace WebApplicationIceCreamProject
//{
//	public class KafkaConsumer
//	{
//		private readonly Func<IceCreamContext> _contextFactory;

//		public KafkaConsumer(Func<IceCreamContext> contextFactory)
//		{
//			_contextFactory = contextFactory;
//		}

//		public async Task StartConsumerAsync(CancellationToken cancellationToken)
//		{
//			using var context = _contextFactory.Invoke();

//			var config = new ConsumerConfig
//			{
//				BootstrapServers = "dory.srvs.cloudkafka.com:9094",
//				SecurityProtocol = SecurityProtocol.SaslSsl,
//				SaslMechanism = SaslMechanism.ScramSha512,
//				SaslUsername = "hamvnwgx", // Replace with your SASL username
//				SaslPassword = "IjJCR7O5tdftLwExKHMMF3ZBMPKBK4Mh",
//				GroupId = "hamvnwgx-group-id",
//				AutoOffsetReset = AutoOffsetReset.Earliest,
//				EnableAutoCommit = false,
//			};

//			using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
//			{
//				consumer.Subscribe("hamvnwgx-orders");

//				while (!cancellationToken.IsCancellationRequested)
//				{
//					try
//					{
//						var consumeResult = consumer.Consume(cancellationToken);
//						var orderJson = consumeResult.Message.Value;

//						var order = JsonSerializer.Deserialize<Order>(orderJson);

//						Console.WriteLine($"Received order: {order.Id}");

//						// Add the received order to the context
//						await context.AddAsync(order);
//						await context.SaveChangesAsync();
//					}
//					catch (OperationCanceledException)
//					{
//						// Handle cancellation request
//					}
//					catch (ConsumeException e)
//					{
//						// Handle the consume exception
//						Console.WriteLine($"Error consuming message: {e.Error.Reason}");
//					}
//				}
//			}
//		}
//	}





