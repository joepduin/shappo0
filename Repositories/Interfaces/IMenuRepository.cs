using shappo0.Models;
using shappo0.ViewModels;
using System.Collections.Generic;

namespace shappo0.Repositories
{
    public interface IMenuRepository
    {
        List<Product> GetProducts();
        Product GetByProductId(int id);
        void AddMenuItem(Product product);
        void EditMenuItem(Product product);
        List<Product> GetFilteredProducts(FilterItemsViewModel? filters);

        void AdjustStock(int productId, int newStock);

        public void BatchAdjustStock(List<OrderLine> orderLines);
    }
}
