using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplicationIceCreamProject.Models;

namespace WebApplicationIceCreamProject.Data
{
    public class IceCreamContext : DbContext
    {
        public IceCreamContext(DbContextOptions<IceCreamContext> options)
            : base(options)
        {
        }
        public IceCreamContext()
        {
        }
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				var loggerFactory = LoggerFactory.Create(builder =>
				{
					builder
						.AddFilter((category, level) =>
							category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
						.AddConsole();
				});

				optionsBuilder
					.UseLoggerFactory(loggerFactory)
					.EnableSensitiveDataLogging()
					.UseSqlServer("AdminContext");
			}
		}
		public DbSet<WebApplicationIceCreamProject.Models.IceCream> IceCream { get; set; } = default!;
        public DbSet<CartItem> ShoppingCartItems { get; set; }
        public DbSet<WebApplicationIceCreamProject.Models.Order>? Order { get; set; }
        public DbSet<WebApplicationIceCreamProject.Models.OrdersDate>? OrdersDate { get; set; }
    }
}
