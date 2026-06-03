using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Repositories
{
    public class DbOrderRepository : IOrderRepository
    {
        private readonly string? _connectionString;

        public DbOrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        O.orderNumber, O.timeCreated, O.[status] AS OrderStatus, 
                        V.visitId, V.tableNumber, V.isActive AS VisitIsActive,
                        OS.staffId AS OrderStaffId, OS.name AS OrderStaffName, OS.[role] AS OrderStaffRole,
                        VS.staffId AS VisitStaffId, VS.name AS VisitStaffName, VS.[role] AS VisitStaffRole
                    FROM [Order] AS O
                    JOIN [Visit] AS V ON O.visitId = V.visitId
                    JOIN [Staff] AS OS ON O.staffId = OS.staffId
                    JOIN [Staff] AS VS ON V.staffId = VS.staffId ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Order order = ReadOrder(reader);
                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }

        public List<Order> GetRunningOrders(OrderStatus status)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT DISTINCT 
                        O.orderNumber, O.timeCreated, O.[status] AS OrderStatus, 
                        V.visitId, V.tableNumber, V.isActive AS VisitIsActive,
                        OS.staffId AS OrderStaffId, OS.name AS OrderStaffName, OS.[role] AS OrderStaffRole,
                        VS.staffId AS VisitStaffId, VS.name AS VisitStaffName, VS.[role] AS VisitStaffRole
                    FROM [Order] AS O
                    JOIN [Visit] AS V ON O.visitId = V.visitId
                    JOIN [Staff] AS OS ON O.staffId = OS.staffId
                    JOIN [Staff] AS VS ON V.staffId = VS.staffId
                    WHERE O.[status] = @status";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@status", status.ToString());

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Order order = ReadOrder(reader);
                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }

        public List<Order> GetRunningOrdersByType(ProductType type)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT DISTINCT 
                        O.orderNumber, O.timeCreated, O.[status] AS OrderStatus, 
                        V.visitId, V.tableNumber, V.isActive AS VisitIsActive,
                        OS.staffId AS OrderStaffId, OS.name AS OrderStaffName, OS.[role] AS OrderStaffRole,
                        VS.staffId AS VisitStaffId, VS.name AS VisitStaffName, VS.[role] AS VisitStaffRole
                    FROM [Order] AS O
                    JOIN [Visit] AS V ON O.visitId = V.visitId
                    JOIN [Staff] AS OS ON O.staffId = OS.staffId
                    JOIN [Staff] AS VS ON V.staffId = VS.staffId " +
                    "INNER JOIN [Order_Line] AS OL ON O.orderNumber = OL.orderNumber " +
                    "INNER JOIN [Product] AS P ON OL.productId = P.productId " +
                    "WHERE O.[status] = @status AND ";

                if (type == ProductType.Drink)
                {
                    query += "ProductType = 'Drink'";
                }
                else
                {
                    query += "ProductType != 'Drink'";
                }

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@status", OrderStatus.Ordered.ToString());

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Order order = ReadOrder(reader);
                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }

        public List<Order> GetOrdersByVisitId(int visitId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        O.orderNumber, O.timeCreated, O.[status] AS OrderStatus, 
                        V.visitId, V.tableNumber, V.isActive AS VisitIsActive,
                        OS.staffId AS OrderStaffId, OS.name AS OrderStaffName, OS.[role] AS OrderStaffRole,
                        VS.staffId AS VisitStaffId, VS.name AS VisitStaffName, VS.[role] AS VisitStaffRole
                    FROM [Order] AS O
                    JOIN [Visit] AS V ON O.visitId = V.visitId
                    JOIN [Staff] AS OS ON O.staffId = OS.staffId
                    JOIN [Staff] AS VS ON V.staffId = VS.staffId
                    WHERE V.visitId = @visitId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@visitId", visitId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Order order = ReadOrder(reader);
                    orders.Add(order);
                }

                reader.Close();
            }
            return orders;
        }



        private Order ReadOrder(SqlDataReader reader)
        {
            Staff orderStaff = new Staff
            {
                StaffId = (int)reader["OrderStaffId"],
                Name = (string)reader["OrderStaffName"],
                Role = Enum.Parse<RoleType>((string)reader["OrderStaffRole"], true)
            };

            Staff visitStaff = new Staff
            {
                StaffId = (int)reader["VisitStaffId"],
                Name = (string)reader["VisitStaffName"],
                Role = Enum.Parse<RoleType>((string)reader["VisitStaffRole"], true)
            };

            Table attachedTable = new Table
            {
                TableNumber = (int)reader["tableNumber"]
            };

            Visit attachedVisit = new Visit
            {
                VisitId = (int)reader["visitId"],
                Table = attachedTable,
                Staff = visitStaff,
                IsActive = (bool)reader["VisitIsActive"]
            };

            int orderNumber = (int)reader["orderNumber"];
            DateTime timeCreated = (DateTime)reader["timeCreated"];
            OrderStatus status = Enum.Parse<OrderStatus>((string)reader["OrderStatus"], true);

            return new Order(orderNumber, timeCreated, status, attachedVisit, orderStaff);
        }

        public int CreateOrder(Order order)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"INSERT INTO [Order](timeCreated, status, visitId, staffId) " +
                                $"VALUES(@timeCreated, @status, @visitId, @staffId); " +
                                "SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@timeCreated", order.TimeCreated);
                command.Parameters.AddWithValue("@status", order.Status.ToString());

                command.Parameters.AddWithValue("@visitId", order.Visit.VisitId);
                command.Parameters.AddWithValue("@staffId", order.Staff.StaffId);

                command.Connection.Open();

                order.OrderNumber = Convert.ToInt32(command.ExecuteScalar());

                return order.OrderNumber;
            }
        }

        public void UpdateOrderStatus(int orderNumber, OrderStatus status)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            string query =
                "UPDATE [ORDER] " +
                "SET status = @status " +
                "WHERE orderNumber = @orderNumber";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@status", status.ToString());
            command.Parameters.AddWithValue("@orderNumber", orderNumber);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}