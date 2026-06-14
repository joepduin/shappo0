using shappo0.Models;

namespace shappo0.ViewModels
{
    public class ProductsViewModel
    {
        public List<Product> AllProducts { get; set; }
        public List<Product> LowStockProducts { get; set; }

        public ProductsViewModel(List<Product> allProducts, List<Product> lowStockProducts)
        {
            AllProducts = allProducts;
            LowStockProducts = lowStockProducts;
        }
    }
}
