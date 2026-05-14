using shappo0.Models;
using shappo0.Repositories;

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

        public Product GetByProductId(int id)
        {
            return _menuRepository.GetByProductId(id);
        }
        public void AddMenuItem(Product product)
        {
            _menuRepository.AddMenuItem(product);
        }
        public void EditMenuItem(Product product)
        {
            _menuRepository.EditMenuItem(product);
        }
    }
}
