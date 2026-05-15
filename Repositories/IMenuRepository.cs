using shappo0.Models;
using System.Collections.Generic;

namespace shappo0.Repositories
{
    public interface IMenuRepository
    {
        List<Product> GetProducts();
        Product GetByProductId(int id);
        void AddMenuItem(Product product);
        void EditMenuItem(Product product);
        List<Product> GetFilteredProducts(string? productType, string? menuType, string? onMenu);
    }
}
