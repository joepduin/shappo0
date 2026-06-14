using shappo0.Models;
using shappo0.ViewModels;

namespace shappo0.Services
{
    public interface IMenuService
    {
        public List<Product> GetProducts();
        public Product? GetByProductId(int? id);
        public List<Product> GetFilteredProducts(FilterItemsViewModel? filters, bool lowStock);
        public void AddMenuItem(Product product);
        public void EditMenuItem(Product product);
        public void AdjustStock(int productId, int newStock);
        public void BatchAdjustStockForOrders(List<OrderLine> orderLines);
        public void BatchAdjustStockForProducts(List<Product> adjustedProducts);
        public bool CompareStock(List<Product> incomingProducts);
    }
}
