using Microsoft.Data.SqlClient;
using shappo0.Enums;
using shappo0.Models;

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

        public Product? GetByProductId(int? productId)
        {
            Product? product = null;

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

        // Applies the filters to the query. Includes 6 important steps
        public List<Product> GetFilteredProducts(FilterItems? filters, bool lowStock)
        {
            // 1. Base query
            string query = "SELECT productId, name, price, vatPercentage, stock, productType, menuType, onMenu " +
                "FROM product";

            // 2. Creates a list to hold conditions which will later be added
            List<string> conditions = new List<string>();

            // 3. Goes over each of the filters and adds them to the conditions List
            if (!string.IsNullOrEmpty(filters?.ProductType))
            {
                conditions.Add("productType = @productType");
            }
            if (!string.IsNullOrEmpty(filters?.MenuType))
            {
                conditions.Add("menuType = @menuType");
            }
            if (filters?.OnMenu.HasValue == true)
            {
                conditions.Add("onMenu = @onMenu");
            }
            // Uses the lowStock boolean to add a condition to the query if it is true (used for the "Low Stock" overview)
            if (lowStock)
            {
                conditions.Add("stock < 10");
            }

            // 4. If there are any conditions, adds them to the query with a WHERE clause
            if (conditions.Count > 0)
            {
                // Uses Join method: first parameter is used as a separator between elements, the second parameter is the List which contain the actual elements
                // Uses following syntax: x AND y AND z
                query += " WHERE " + string.Join(" AND ", conditions);
            }

            // 5. Adds an ORDER BY clause, since that has to be after the WHERE clause
            query += " ORDER BY onMenu DESC, menuType, productType, name";

            // 6. Does the "regular" database logic
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // CHANGE: make sure to introduce the parameters within the written query as STRINGS, not as ENUMS (changed in database)!
                    if (!string.IsNullOrEmpty(filters?.ProductType))
                        command.Parameters.AddWithValue("@productType", filters.ProductType);
                    if (!string.IsNullOrEmpty(filters?.MenuType))
                        command.Parameters.AddWithValue("@menuType", filters.MenuType);
                    if (filters?.OnMenu.HasValue == true)
                        command.Parameters.AddWithValue("@onMenu", filters.OnMenu.Value);

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
                command.Parameters.AddWithValue("@productType", product.ProductType.ToString());
                command.Parameters.AddWithValue("@menuType", product.MenuType.ToString());
                command.Parameters.AddWithValue("@onMenu", product.OnMenu);

                command.Connection.Open();

                int nrOfRowsAffected = command.ExecuteNonQuery();
                if (nrOfRowsAffected == 0)
                    throw new Exception("No records updated..");
            }
        }

        // Method for singular stock adjustment
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

        // Laurens
        //Method to batch adjust the stock so the connection doesn't need to reopen for as many times as there are orderlines
        // Uses OrderLine objects
        public void BatchAdjustStockForOrders(List<OrderLine> orderLines)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE [PRODUCT] SET stock = stock - @quantity WHERE productId = @productId AND stock >= @quantity";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Connection.Open();

                    foreach (OrderLine line in orderLines)
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@quantity", line.Quantity);
                        command.Parameters.AddWithValue("@productId", line.Product.ProductId);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        // Uses Product objects
        public void BatchAdjustStockForProducts(List<Product> adjustedProducts)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE [PRODUCT] SET stock = @stock WHERE productId = @productId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Connection.Open();
                    foreach (Product product in adjustedProducts)
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@stock", product.Stock);
                        command.Parameters.AddWithValue("@productId", product.ProductId);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static Product ReadProduct(SqlDataReader reader)
        {
            int productId = (int)reader["productId"];
            string name = (string)reader["name"];
            decimal price = (decimal)reader["price"];
            decimal vatPercentage = (decimal)reader["vatPercentage"];
            int stock = (int)reader["stock"];
            bool onMenu = (bool)reader["onMenu"];
            ProductType productType = Enum.Parse<ProductType>((string)reader["productType"], true);
            MenuType menuType = Enum.Parse<MenuType>((string)reader["menuType"], true);

            return new Product(productId, name, price, vatPercentage, stock, productType, menuType, onMenu);
        }
    }
}
