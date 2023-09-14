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

        public DbSet<WebApplicationIceCreamProject.Models.IceCream> IceCream { get; set; } = default!;
        public DbSet<CartItem> ShoppingCartItems { get; set; }
    }
}
