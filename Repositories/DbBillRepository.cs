using shappo0.Models;
using Microsoft.Data.SqlClient;
using shappo0.Repositories.Interfaces;

namespace shappo0.Repositories
{

        public class DbBillRepository : IBillRepository
        {
            private readonly string? _connectionString;

            public DbBillRepository(IConfiguration configuration)
            {
                _connectionString = configuration.GetConnectionString("shappo0");
            }

            public int SaveBill(Bill bill)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "INSERT INTO BILL (subTotal, total, VATLow, VATHigh, isPaid, visitId) VALUES (@subTotal, @total, @vatLow, @vatHigh, @isPaid, @visitId); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@subTotal", bill.SubTotal);
                        command.Parameters.AddWithValue("@total", bill.Total);
                        command.Parameters.AddWithValue("@vatLow", bill.VatLow);
                        command.Parameters.AddWithValue("@vatHigh", bill.VatHigh);
                        command.Parameters.AddWithValue("@isPaid", bill.IsPaid);
                        command.Parameters.AddWithValue("@visitId", bill.VisitId);

                        connection.Open();
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }

            public Bill GetBillByVisitId(int visitId)
            {
                Bill bill = null;
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT billId, subTotal, total, VATLow, VATHigh, isPaid, visitId FROM BILL WHERE visitId = @visitId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@visitId", visitId);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bill = ReadBill(reader);
                            }
                        }
                    }
                }
                return bill;
            }

            public void UpdateBillPaid(int billId, bool isPaid)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE BILL SET isPaid = @isPaid WHERE billId = @billId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@isPaid", isPaid);
                        command.Parameters.AddWithValue("@billId", billId);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }

            private Bill ReadBill(SqlDataReader reader)
            {
                return new Bill
                {
                    BillId = (int)reader["billId"],
                    SubTotal = (decimal)reader["subTotal"],
                    Total = (decimal)reader["total"],
                    VatLow = (decimal)reader["VATLow"],
                    VatHigh = (decimal)reader["VATHigh"],
                    IsPaid = (bool)reader["isPaid"],
                    VisitId = (int)reader["visitId"]
                };
            }
        }
    }