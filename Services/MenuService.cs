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

        public Product? GetByProductId(int? id)
        {
            return _menuRepository.GetByProductId(id);
        }

        public List<Product> GetFilteredProducts(FilterItems? filters, bool lowStock)
        {
            return _menuRepository.GetFilteredProducts(filters, lowStock);
        }

        public void AddMenuItem(Product product)
        {
            if (CheckValidStock(product))
            {
                // Price is calculated: the VAT % is calculated into a 1.09 or 1.21 value, which the Price is then divided by
                decimal priceExcludingVAT = product.Price / (1 + (product.VATPercentage / 100));
                product.Price = priceExcludingVAT;

                _menuRepository.AddMenuItem(product);
            }
            else
            {
                throw new ArgumentException("Stock cannot be negative.");
            }
        }
        public void EditMenuItem(Product product)
        {
            if (CheckValidStock(product))
            {
                // Re-applies the logic from the Add method in order to save the correct price, while still displaying the price including VAT in the edit View
                decimal priceExcludingVAT = product.Price / (1 + (product.VATPercentage / 100));
                product.Price = priceExcludingVAT;

                _menuRepository.EditMenuItem(product);
            }
            else
                throw new ArgumentException("Stock cannot be negative.");
        }
        public void AdjustStock(int productId, int newStock)
        {
            if (newStock < 0)
                throw new ArgumentException("Stock cannot be negative.");
            else 
                _menuRepository.AdjustStock(productId, newStock);
        }

        // Laurens
        public void BatchAdjustStockForOrders(List<OrderLine> orderLines)
        {
            foreach (OrderLine orderLine in orderLines)
            {
                if (orderLine.Product.Stock - orderLine.Quantity < 0)
                {
                    throw new InvalidOperationException($"Insufficient stock for product {orderLine.Product.Name}. Cannot adjust stock.");
                }
            }
            _menuRepository.BatchAdjustStockForOrders(orderLines);
        }

        // Ylvi
        public void BatchAdjustStockForProducts(List<Product> adjustedProducts)
        {
            // Checks if stock change is possible
            foreach (Product product in adjustedProducts)
            {
                if (product.Stock < 0)
                {
                    throw new ArgumentException($"Stock for product {product.Name} cannot be negative.");
                }
            }
            _menuRepository.BatchAdjustStockForProducts(adjustedProducts);
        }

        // Compares a List with incoming products to the current database products, checking for ANY stock mismatch
        public bool CompareStock(List<Product> incomingProducts)
        {
            List<Product> currentDbProducts = _menuRepository.GetProducts();

            // Loops through each Product in incoming list
            foreach (Product incomingProduct in incomingProducts)
            {
                // Looks through each Product in the database list, looking for a matching ProductId
                foreach (var dbProduct in currentDbProducts)
                {
                    if (dbProduct.ProductId == incomingProduct.ProductId)
                    {
                        // Checks if the stock values are different
                        if (incomingProduct.Stock != dbProduct.Stock)
                        {
                            return false;
                        }
                        break;
                    }
                }
            }

            // Returns true, meaning no Stock was changed
            return true;
        }

        private bool CheckValidStock(Product product)
        {
            if (product.Stock < 0)
            {
                return false;
            }
            return true;
        }
    }
}
