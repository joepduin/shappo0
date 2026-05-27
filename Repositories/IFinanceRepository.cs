using Microsoft.Data.SqlClient;
using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IFinanceRepository
    {
        List<Bill> GetFinances();
        List<Bill> GetFinancesByDate(DateTime date);
    }
}
