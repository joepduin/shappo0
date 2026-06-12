using shappo0.Models;
using shappo0.Repositories;
using shappo0.ViewModels;

namespace shappo0.Services
{
    public class MenuService : IMenuService
    {
        private IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public List<Product> GetProducts()
        {
            return _menuRepository.GetProducts();
        }

        public Product? GetByProductId(int? id)
        {
            return _menuRepository.GetByProductId(id);
        }

        public List<Product> GetFilteredProducts(FilterItemsViewModel? filters)
        {
            return _menuRepository.GetFilteredProducts(filters);
        }

        public void AddMenuItem(Product product)
        {
            _menuRepository.AddMenuItem(product);
        }
        public void EditMenuItem(Product product)
        {
            _menuRepository.EditMenuItem(product);
        }
        public void AdjustStock(int productId, int newStock)
        {
            _menuRepository.AdjustStock(productId, newStock);
        }
        public void BatchAdjustStock(List<OrderLine> orderLines)
        {
            _menuRepository.BatchAdjustStock(orderLines);
        }
    }
}
