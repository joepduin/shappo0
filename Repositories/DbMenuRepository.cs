using Microsoft.AspNetCore.Components.Endpoints;
using shappo0.Models;
using System.Data.SqlClient;

namespace shappo0.Repositories
{
    public class DbMenuRepository : IMenuRepository
    {
        private readonly string? _connectionString;

        public DbMenuRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        public List<Product> GetProducts()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT name, price, vatPercentage, stock, productType, menuType, onMenu FROM product";
                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Product product = ReadProduct(reader);
                    products.Add(product);
                }
                reader.Close();
            }
            return products;
        }

        public Product GetByProductId(int productId)
        {
            Product product = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT productId, name, price, vatPercentage, stock, productType, menuType, onMenu " +
                    "FROM product " +
                    "WHERE productId = @productId";
                SqlCommand command = new SqlCommand(query);

                command.Parameters.AddWithValue("@productId", productId);
                
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if(reader.Read())
                    product = ReadProduct(reader);

                reader.Close();
            }

            return product;
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
                command.Parameters.AddWithValue("@productType", product.ProductType);
                command.Parameters.AddWithValue("@menuType", product.MenuType);
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

        private Product ReadProduct(SqlDataReader reader)
        {
            int productId = (int)reader["productId"];
            string name = (string)reader["name"];
            int price = (int)reader["price"];
            int vatPercentage = (int)reader["vatPercentage"];
            int stock = (int)reader["stock"];
            string productType = (string)reader["productType"];
            string menuType = (string)reader["menuType"];
            bool onMenu = (bool)reader["onMenu"];

            return new Product(productId, name, price, vatPercentage, stock, productType, menuType, onMenu);
        }
    }
}
