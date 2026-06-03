using Microsoft.Data.SqlClient;
using shappo0.Models;

namespace shappo0.Repositories.Interfaces
{
    public interface IFinanceRepository
    {
        List<Finance> GetIncomeByDate(DateRange? range);
        decimal GetTipsByDate(DateRange? range);
    }
}
