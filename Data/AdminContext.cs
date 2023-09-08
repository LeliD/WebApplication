using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplicationIceCreamProject.Models;

namespace WebApplicationIceCreamProject.Data
{
    public class AdminContext : DbContext
    {
        public AdminContext (DbContextOptions<AdminContext> options)
            : base(options)
        {
        }

        public DbSet<WebApplicationIceCreamProject.Models.IceCream> IceCream { get; set; } = default!;
    }
}
