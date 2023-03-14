using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Data.Entities
{
    public abstract class BaseEntity 
    {
        public BaseEntity()
        {
            CreatedDate = DateTime.Now;
        }
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }


    }



    public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {

            builder.HasQueryFilter(x => x.IsDeleted == false); // Bu veritabanı üzerinde yapılan bütün sorgularda bu satır kullanılacak. Özetle veri çekimlerinde artık yalnızca silinmemiş verileri getir gibi bir .where koşulu yazmaya gerek yok.

            // Örneğin programın herhangi bir yerinde _db.Products.Where(x => x.isDeleted == false) yazmak yerine yalnızca _db.Products yazacağız.



           

            builder.Property(x => x.ModifiedDate).IsRequired(false); // null olabilir.

        }

        // Bu metodun özelliklerinin miras alınan kısımlarda (diğer entityler için) çalışmasını istiyorum. Aynı zamanda da ilave kodlar yazıp , metodun işleyişini değiştireceğim (override). Bu neden metot Virtual tanımlanmalı.

    }



}
