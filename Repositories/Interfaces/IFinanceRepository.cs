using Microsoft.Data.SqlClient;
using shappo0.Models;

namespace shappo0.Repositories.Interfaces
{
    public interface IFinanceRepository
    {
        List<Finance> GetIncomeByDate(DateTime startDate, DateTime endDate);
        decimal GetTipsByDate(DateTime startDate, DateTime endDate);
    }
}
