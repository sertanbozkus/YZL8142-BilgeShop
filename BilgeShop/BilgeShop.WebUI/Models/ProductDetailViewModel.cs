namespace BilgeShop.WebUI.Models
{
    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? UnitPrice { get; set; }
        public string ImagePath { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }


        public bool IsSoldOut { get; set; }
        public string SoldingOutMessage { get; set; }

    }
}
