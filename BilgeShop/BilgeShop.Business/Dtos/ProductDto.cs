using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Business.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; } // string zaten nullable olduğu için, ? yazsak da yazmasak da olur. UnitPrice (decimal?) gibi değil.

        public decimal? UnitPrice { get; set; }
        public int UnitInStock { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImagePath { get; set; }


    }
}
