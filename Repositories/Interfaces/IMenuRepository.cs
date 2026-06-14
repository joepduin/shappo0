using shappo0.Models;
using shappo0.ViewModels;
using System.Collections.Generic;

namespace shappo0.Repositories
{
    public interface IMenuRepository
    {
        List<Product> GetProducts();
        Product? GetByProductId(int? id);
        void AddMenuItem(Product product);
        void EditMenuItem(Product product);
        List<Product> GetFilteredProducts(FilterItemsViewModel? filters, bool lowStock);

        void AdjustStock(int productId, int newStock);

        public void BatchAdjustStockForOrders(List<OrderLine> orderLines);
        public void BatchAdjustStockForProducts(List<Product> adjustedProducts);
    }
}
