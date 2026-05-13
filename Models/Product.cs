namespace shappo0.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal VATPercentage { get; set; }
        public int Stock {  get; set; }
        public string ProductType { get; set; }
        public string MenuType { get; set; }
        public bool OnMenu { get; set; }

        public Product()
        {

        }

        public Product(string name, decimal price, decimal vatPercentage, int stock, string productType, string menuType, bool onMenu)
        {
            Name = name;
            Price = price;
            VATPercentage = vatPercentage;
            Stock = stock;
            ProductType = productType;
            MenuType = menuType;
            OnMenu = onMenu;
        }

        public Product(int productId, string name, decimal price, decimal vatPercentage, int stock, string productType, string menuType, bool onMenu)
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
