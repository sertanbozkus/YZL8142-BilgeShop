using BilgeShop.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Data.Context
{
    public class BilgeShopContext : DbContext
    {

        public BilgeShopContext(DbContextOptions<BilgeShopContext> options) : base(options)
        {

        }
         

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // FLUENT API -> C# tarafındaki classlarım , Sql tablolarına dönüştürülürken özelliklerinde yaptığım biçimlendirmeler.

            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());

            base.OnModelCreating(modelBuilder);

        }

        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<ProductEntity> Products => Set<ProductEntity>();
        public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();




    }
}

