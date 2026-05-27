using Microsoft.AspNetCore.Components.Endpoints;
using shappo0.Models;
using Microsoft.Data.SqlClient;

namespace shappo0.Repositories
{
    public class DbMenuRepository : IMenuRepository
    {
        private readonly string? _connectionString;

        public DbMenuRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        // Regular GetProducts method, but with an ORDER BY clause to sort the products by onMenu (descending) and then by name (ascending)
        public List<Product> GetProducts()
        {
            List<Product> products = new List<Product>();
            string query = "SELECT productId, name, price, vatPercentage, stock, productType, menuType, onMenu " +
                "FROM product " +
                "ORDER BY onMenu DESC, menuType, productType, name";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            products.Add(ReadProduct(reader));
                    }
                }
            }
            return products;
        }

        public Product GetByProductId(int productId)
        {
            Product product = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT productId, name, price, vatPercentage, stock, productType, menuType, onMenu FROM product WHERE productId = @productId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@productId", productId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            product = ReadProduct(reader);
                    }
                }
            }

            return product;
        }

        public List<Product> GetFilteredProducts(string? productType, string? menuType, string? onMenu)
        {
            string query = "SELECT productId, name, price, vatPercentage, stock, productType, menuType, onMenu " +
                "FROM product";

            List<string> conditions = new List<string>();


            if (!string.IsNullOrEmpty(productType)) 
            {
                conditions.Add("productType = @productType");
            }

            if (!string.IsNullOrEmpty(menuType))
            {
                conditions.Add("menuType = @menuType");
            }

            if (!string.IsNullOrEmpty(onMenu))
            {
                conditions.Add("onMenu = @onMenu");
            }

            if (conditions.Count > 0)
            {
                query += " WHERE " + string.Join(" AND ", conditions);
            }

            query += " ORDER BY onMenu DESC, menuType, productType, name";

                List<Product> products = new List<Product>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Makes sure to introduce the parameters within the written query AS strings, not as enums!
                    if (!string.IsNullOrEmpty(productType))
                        command.Parameters.AddWithValue("@productType", Enum.Parse<ProductType>(productType.ToString(), true));
                    if (!string.IsNullOrEmpty(menuType))
                        command.Parameters.AddWithValue("@menuType", Enum.Parse<MenuType>(menuType.ToString(), true));
                    // Since onMenu is a boolean, we need to convert the string value to a boolean before adding it as a parameter
                    if (!string.IsNullOrEmpty(onMenu))
                    {
                        bool isOnMenu = onMenu.ToLower() == "true";
                        command.Parameters.AddWithValue("@onMenu", isOnMenu);
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            products.Add(ReadProduct(reader));
                    }
                }
            }
            return products;
        }

        public void AddMenuItem(Product product)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"INSERT INTO product (name, price, vatPercentage, stock, productType, menuType, onMenu) " +
                    $"VALUES (@name, @price, @vatPercentage, @stock, @productType, @menuType, @onMenu); " +
                    $"SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@name", product.Name);
                command.Parameters.AddWithValue("@price", product.Price);
                command.Parameters.AddWithValue("@vatPercentage", product.VATPercentage);
                command.Parameters.AddWithValue("@stock", product.Stock);
                command.Parameters.AddWithValue("@productType", product.ProductType.ToString());
                command.Parameters.AddWithValue("@menuType", product.MenuType.ToString());
                command.Parameters.AddWithValue("@onMenu", product.OnMenu);

                command.Connection.Open();

                product.ProductId = Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void EditMenuItem(Product product)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE product " +
                    $"SET name = @name, price = @price, vatPercentage = @vatPercentage, stock = @stock, productType = @productType, menuType = @menuType, onMenu = @onMenu " +
                    $"WHERE productId = @productId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@productId", product.ProductId);
                command.Parameters.AddWithValue("@name", product.Name);
                command.Parameters.AddWithValue("@price", product.Price);
                command.Parameters.AddWithValue("@vatPercentage", product.VATPercentage);
                command.Parameters.AddWithValue("@stock", product.Stock);
                command.Parameters.AddWithValue("@productType", product.ProductType);
                command.Parameters.AddWithValue("@menuType", product.MenuType);
                command.Parameters.AddWithValue("@onMenu", product.OnMenu);

                command.Connection.Open();

                int nrOfRowsAffected = command.ExecuteNonQuery();
                if (nrOfRowsAffected == 0)
                    throw new Exception("No records updated..");
            }
        }

        public void AdjustStock(int productId, int newStock)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE product " +
                    $"SET stock = @stock " +
                    $"WHERE productId = @productId";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@productId", productId);
                command.Parameters.AddWithValue("@stock", newStock);

                command.Connection.Open();

                int nrOfRowsAffected = command.ExecuteNonQuery();
                if (nrOfRowsAffected == 0)
                    throw new Exception("No records updated..");
            }
        }
        private Product ReadProduct(SqlDataReader reader)
        {
            int productId = (int)reader["productId"];
            string name = (string)reader["name"];
            decimal price = (decimal)reader["price"];
            decimal vatPercentage = (decimal)reader["vatPercentage"];
            int stock = (int)reader["stock"];
            string productTypeString = (string)reader["productType"];
            string menuTypeString = (string)reader["menuType"];

            ProductType productType = Enum.Parse<ProductType>(productTypeString, true);
            MenuType menuType = Enum.Parse<MenuType>(menuTypeString, true);

            bool onMenu = (bool)reader["onMenu"];

            return new Product(productId, name, price, vatPercentage, stock, productType, menuType, onMenu);
        }
    }
}
