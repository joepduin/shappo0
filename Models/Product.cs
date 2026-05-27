using shappo0.Enums;

namespace shappo0.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal VATPercentage { get; set; }
        public int Stock {  get; set; }
        public ProductType ProductType { get; set; }
        public MenuType MenuType { get; set; }
        public bool OnMenu { get; set; }

        public Product()
        {

        }

        // Constructor for creating a new product without specifying the ProductId, which is used in the Create methods, since the ProductId is later added
        public Product(string name, decimal price, decimal vatPercentage, int stock, ProductType productType, MenuType menuType, bool onMenu)
        {
            Name = name;
            Price = price;
            VATPercentage = vatPercentage;
            Stock = stock;
            ProductType = productType;
            MenuType = menuType;
            OnMenu = onMenu;
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

        // Constructor for updating the stock of a product, which only requires the ProductId and the new stock value
        public Product(int productId, int stock)
        {
            ProductId = productId;
            Stock = stock;
        }
    }
}
