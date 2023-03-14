using BilgeShop.Data.Context;
using BilgeShop.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Data.Repositories
{
    public class SqlRepository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        private readonly BilgeShopContext _db;
        private readonly DbSet<TEntity> _dbSet;
        public SqlRepository(BilgeShopContext db)
        {
            _db = db;
            _dbSet = _db.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
           //  entity.CreatedDate = DateTime.Now eğer constructor'da tanımlamıyorsanız (BaseEntity içerisi) burada da kullanılabilir.
            _dbSet.Add(entity);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            Delete(entity);
        }

        public void Delete(TEntity entity)
        {
            entity.IsDeleted = true;
            entity.ModifiedDate = DateTime.Now;
            _dbSet.Update(entity);
            _db.SaveChanges();

            // Artık projemde DbContext metotlarına direkt erişim ve kullanım vermeyeceğim, aracı olarak Repository kullancağım için, istense de Hard Delete yapılamaz. Çünkü Delete motlarının hepsi Soft Delete olarak özelleştirilmiş.

            // HardDelete diye ayrı bir hizmet açılabilir fakat genellikle böyle bir kullanım olmaz, Hard Delete işlemi yapılmak isterse , Veritabancı tarafından Sql Server üzerinden yapılır.

        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);

            // Bu veri varsa, veriyi dön, yoksa boş dön.


        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate is not null ? _dbSet.Where(predicate) : _dbSet;
        }

        // Tolist() -> Nesneleri döner.
        // IQueryable -> Sorgu döner.
 

        public TEntity GetById(int id)
        {
            return _dbSet.Find(id);
        }
       
        public void Update(TEntity entity)
        {
            entity.ModifiedDate = DateTime.Now;
            _dbSet.Update(entity);
            _db.SaveChanges();
        }
    }
}


// Generic repository sayesinde, tek tek her tablonuz için repository classı açıp, metotlar tanımlamanıza gerek yok.

// Oldu ki bir tablo için DbContext işlemlerinde kullanacağınız metotların farklı çalışma gerekiyor, O zaman yalnızca o tabloya özel bir interface-repository ikilisi açılır.
