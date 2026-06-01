using shappo0.Models;

namespace shappo0.Services
{
    public interface IMenuService
    {
        public List<Product> GetProducts();
        public Product GetByProductId(int id);
        public List<Product> GetFilteredProducts(string? productType, string? menuType, string? onMenu);
        public void AddMenuItem(Product product);
        public void EditMenuItem(Product product);

        public void AdjustStock(int productId, int newStock);  
    }
}
