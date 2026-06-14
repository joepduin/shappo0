using Microsoft.Data.SqlClient;
using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Repositories
{
    public class DbPaymentRepository : IPaymentRepository
    {
        private readonly string? _connectionString;

        public DbPaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        public int SavePayment(Payment payment)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO PAYMENT (paymentMethod, paymentAmount, billId, paymentTip) 
                                 VALUES (@paymentMethod, @paymentAmount, @billId, @paymentTip);
                                 SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@paymentMethod", payment.PaymentMethod.ToString());
                    command.Parameters.AddWithValue("@paymentAmount", payment.PaymentAmount);
                    command.Parameters.AddWithValue("@billId", payment.Bill.BillId);
                    command.Parameters.AddWithValue("@paymentTip", payment.PaymentTip);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public decimal GetTotalPaidByBillId(int billId)
        {
            decimal totalPaid = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {                           //Als er geen payments zijn in een bill zou het anders crachen, daarom de ISNULL.
                string query = @"SELECT ISNULL(SUM(paymentAmount), 0)
                                 FROM PAYMENT 
                                 WHERE billId = @billId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@billId", billId);

                    connection.Open();
                    totalPaid = Convert.ToDecimal(command.ExecuteScalar());
                }
            }
            return totalPaid;
        }

        public List<Payment> GetPaymentsByBillId(int billId)
        {
            List<Payment> payments = new List<Payment>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT paymentId, paymentMethod, paymentAmount, billId, paymentTip 
                                 FROM PAYMENT 
                                 WHERE billId = @billId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@billId", billId);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(ReadPayment(reader));
                        }
                    }
                }
            }
            return payments;
        }

        public int SaveBill(Bill bill)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO BILL (subTotal, total, VATLow, VATHigh, totalTip, isPaid, visitId, splitChoice, numberOfPeople)
                                 VALUES (@subTotal, @total, @vatLow, @vatHigh, @totalTip, @isPaid, @visitId, @splitChoice, @numberOfPeople);
                                 SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@subTotal", bill.SubTotal);
                    command.Parameters.AddWithValue("@total", bill.Total);
                    command.Parameters.AddWithValue("@vatLow", bill.VatLow);
                    command.Parameters.AddWithValue("@vatHigh", bill.VatHigh);
                    command.Parameters.AddWithValue("@totalTip", bill.TotalTip);
                    command.Parameters.AddWithValue("@isPaid", bill.IsPaid);
                    command.Parameters.AddWithValue("@visitId", bill.Visit.VisitId);
                    command.Parameters.AddWithValue("@splitChoice", bill.SplitChoice.ToString());

                    if (bill.NumberOfPeople == null) command.Parameters.AddWithValue("@numberOfPeople", DBNull.Value);
                    else command.Parameters.AddWithValue("@numberOfPeople", bill.NumberOfPeople);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public Bill? GetBillByVisitId(int? visitId)
        {
            Bill? bill = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT billId, subTotal, total, VATLow, VATHigh, totalTip, isPaid, visitId, splitChoice, numberOfPeople
                                 FROM BILL 
                                 WHERE visitId = @visitId";

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

        public Bill? GetBillByVisitId(int visitId)
        {
            Bill? bill = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT billId, subTotal, total, VATLow, VATHigh, totalTip, isPaid, visitId, splitChoice, numberOfPeople 
                                 FROM BILL 
                                 WHERE visitId = @visitId;";

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

        public Bill? GetBillById(int? billId)
        {
            Bill? bill = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT billId, subTotal, total, VATLow, VATHigh, totalTip, isPaid, visitId, splitChoice, numberOfPeople
                                 FROM BILL 
                                 WHERE billId = @billId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@billId", billId);

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

        public void UpdateBillPaid(Bill bill)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE BILL 
                                 SET isPaid = @isPaid, totalTip = @totalTip
                                 WHERE billId = @billId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@isPaid", true);
                    command.Parameters.AddWithValue("@totalTip", bill.TotalTip);
                    command.Parameters.AddWithValue("@billId", bill.BillId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveFeedback(Payment payment)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO FEEDBACK ([type], description, paymentId) 
                                 VALUES (@type, @description, @paymentId);";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@type", payment.Feedback?.Type.ToString());
                    command.Parameters.AddWithValue("@description", payment.Feedback?.Description);
                    command.Parameters.AddWithValue("@paymentId", payment.PaymentId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private Bill ReadBill(SqlDataReader reader)
        {
            int? numberOfPeople = null; //dit kan ook leeg zijn
            if (reader["numberOfPeople"] != DBNull.Value) numberOfPeople = (int)reader["numberOfPeople"];

            return new Bill
            {
                BillId = (int)reader["billId"],
                SubTotal = (decimal)reader["subTotal"],
                Total = (decimal)reader["total"],
                VatLow = (decimal)reader["VATLow"],
                VatHigh = (decimal)reader["VATHigh"],
                TotalTip = (decimal)reader["totalTip"],
                IsPaid = (bool)reader["isPaid"],
                Visit = new Visit { VisitId = (int)reader["visitId"] },
                SplitChoice = Enum.Parse<SplitChoice>((string)reader["splitChoice"], true),
                NumberOfPeople = numberOfPeople
            };
        }

        private Payment ReadPayment(SqlDataReader reader)
        {
            return new Payment
            {
                PaymentId = (int)reader["paymentId"],
                PaymentMethod = Enum.Parse<PaymentMethodType>((string)reader["paymentMethod"], true),
                PaymentAmount = (decimal)reader["paymentAmount"],
                PaymentTip = (decimal)reader["paymentTip"],
                Bill = new Bill { BillId = (int)reader["billId"] }
            };
        }
    }
}