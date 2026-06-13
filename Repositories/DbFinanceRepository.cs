using shappo0.Models;
using Microsoft.Data.SqlClient;
using shappo0.Repositories.Interfaces;

namespace shappo0.Repositories
{
    public class DbFinanceRepository : IFinanceRepository
    {
        private readonly string? _connectionString;
        public DbFinanceRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        public List<Finance> GetIncomeByDate(DateRange? range)
        {
            List<Finance> finances = new List<Finance>();

            DateTime startDate = range?.Start ?? DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;
            if (range?.End != null)
            {
                endDate = range.End.Value.AddDays(1);
            }

            // Query selects by group based on menutype, sums the amount of items ordered within that group and calculates the revenue per group
            // Also includes the filter option if date is selected
            string query = @"SELECT p.menuType, SUM(ol.quantity) AS total_items_ordered, ROUND((SUM(ol.quantity * (p.price * (p.VATPercentage + 100) / 100))), 2) AS total_revenue " +
                "FROM ORDER_LINE AS ol " +
                "JOIN PRODUCT AS p ON ol.productId = p.productId " +
                "JOIN [ORDER] AS o ON ol.orderNumber = o.orderNumber " +
                "WHERE o.timeCreated >= @StartDate AND o.timeCreated <= @EndDate " +
                "GROUP BY p.menuType;";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Finance finance = ReadFinance(reader);
                            finances.Add(finance);
                        }
                    }
                }
            }

            return finances;
        }


        public decimal GetTipsByDate(DateRange? range)
        {
            decimal totalTips = 0;

            DateTime startDate = range?.Start ?? DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;
            if (range?.End != null)
            {
                endDate = range.End.Value.AddDays(1);
            }

            // ORDER is joined from VISIT to BILL, since the timeCreated column (ORDER) is required for the date filter and tips are located in BILL
            string query = @"SELECT ISNULL(SUM(b.totalTip), 0) AS total_tips " +
                "FROM BILL AS b " +
                "JOIN VISIT AS v ON b.visitId = v.visitId " +
                "JOIN [ORDER] AS o ON v.visitId = o.visitId " +
                "WHERE o.timeCreated >= @StartDate AND o.timeCreated <= @EndDate;";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    connection.Open();
                    totalTips = (decimal)command.ExecuteScalar();
                }
            }
            return totalTips;
        }

        private Finance ReadFinance(SqlDataReader reader)
        {
            string menuType = (string)reader["menuType"];
            int totalItemsOrdered = (int)reader["total_items_ordered"];
            decimal totalRevenue = (decimal)reader["total_revenue"];
            return new Finance(menuType, totalItemsOrdered, totalRevenue);
        }
    }
}
