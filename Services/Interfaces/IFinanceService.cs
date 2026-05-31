using Microsoft.Data.SqlClient;
using shappo0.Models;

namespace shappo0.Services.Interfaces
{
    public interface IFinanceService
    {
        List<Finance> GetIncomeByDate(DateTime startDate, DateTime endDate);
        
        decimal GetTipsByDate(DateTime startDate, DateTime endDate);

    }
}
