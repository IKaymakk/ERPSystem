using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERPSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERPSystem.Infrastructure.DBContext;

public class ErpDbContext : DbContext
{
    public ErpDbContext(DbContextOptions<ErpDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<Unit> Units { get; set; }

}