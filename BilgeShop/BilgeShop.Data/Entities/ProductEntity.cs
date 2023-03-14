using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Data.Entities
{
    public class ProductEntity : BaseEntity
    {
        
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? UnitPrice { get; set; } // ? -> nullable
        public int UnitInStock { get; set; }
        public string ImagePath { get; set; }

        public int CategoryId { get; set; }

        // Relational Property
        public CategoryEntity Category { get; set; }
    }



    public class ProductEntityConfiguration : BaseConfiguration<ProductEntity>
    {
        public override void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            // ürün adı zorunlu ve max 50 karakter

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            // ürün açıklaması zorunlu olmayabilir.

            builder.Property(x => x.Description)
                .IsRequired(false);


            // ürün fiyatı zorunlu olmayabilir.

            builder.Property(x => x.UnitPrice)
                .IsRequired(false);

            // ürün görseli zorunlu olmayabilir.

            builder.Property(x => x.ImagePath)
                .IsRequired(false);

            // kategori id zorunlu

            builder.Property(x => x.CategoryId)
                .IsRequired();

            // unitstock zorunlu.

            builder.Property(x => x.UnitInStock)
                .IsRequired();

            // zorunlu olma kısımları default olarak atanır, yazılmak zorunda değil fakat ben tüm bilgiler bir arada gözüksün, inceleme/hata ayıklama daha kolay olsun diye, önemli olanları yazıyorum.

            base.Configure(builder);
        }
    }

}



