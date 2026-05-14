using shappo0.Models;

namespace shappo0.Services
{
    public interface IMenuService
    {
        public List<Product> GetProducts();
        public Product GetByProductId(int id);
        public void AddMenuItem(Product product);
        public void EditMenuItem(Product product);
    }
}