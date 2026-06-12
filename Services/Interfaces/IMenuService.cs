using shappo0.Models;
using shappo0.ViewModels;

namespace shappo0.Services
{
    public interface IMenuService
    {
        public List<Product> GetProducts();
        public Product? GetByProductId(int? id);
        public List<Product> GetFilteredProducts(FilterItemsViewModel? filters);
        public void AddMenuItem(Product product);
        public void EditMenuItem(Product product);
        public void AdjustStock(int productId, int newStock);
        public void BatchAdjustStock(List<OrderLine> orderLines);
    }
}
