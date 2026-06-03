using Microsoft.Data.SqlClient;
using shappo0.Models;

namespace shappo0.Services.Interfaces
{
    public interface IFinanceService
    {
        List<Finance> GetIncomeByDate(DateRange? range, string? quickFilter);


        decimal GetTipsByDate(DateRange? range, string? quickFilter);
        DateRange? CalculateRange(DateRange? range, string? quickFilter);
    }
}