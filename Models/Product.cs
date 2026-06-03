using shappo0.Enums;

namespace shappo0.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public decimal VATPercentage { get; set; }
        public int Stock {  get; set; }
        public ProductType ProductType { get; set; }
        public MenuType MenuType { get; set; }
        public bool OnMenu { get; set; }
        public decimal PriceIncludingTax
        {
            get
            {
                return Price + (Price * (VATPercentage / 100));
            }
            set { }
        }

        public Product()
        {

        }

        // Regular constructor
        public Product(int productId, string name, decimal price, decimal vatPercentage, int stock, ProductType productType, MenuType menuType, bool onMenu)
        {
            ProductId = productId;
            Name = name;
            Price = price;
            VATPercentage = vatPercentage;
            Stock = stock;
            ProductType = productType;
            MenuType = menuType;
            OnMenu = onMenu;
        }
    }
}
