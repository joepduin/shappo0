using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IMenuRepository
    {
        public List<Product> GetProducts();
        public Product GetByProductId(int id);
        public void AddMenuItem(Product product);
        public void EditMenuItem(Product product);
    }
}
