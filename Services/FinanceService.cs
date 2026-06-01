using shappo0.Models;
using shappo0.Repositories.Interfaces;
using shappo0.Services.Interfaces;

namespace shappo0.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly IFinanceRepository _financeRepository;

        public FinanceService(IFinanceRepository financeRepository)
        {
            _financeRepository = financeRepository;
        }


        public List<Finance> GetIncomeByDate(DateTime startDate, DateTime endDate)
        {
            return _financeRepository.GetIncomeByDate(startDate, endDate);
        }

        public decimal GetTipsByDate(DateTime startDate, DateTime endDate)
        { 
            return _financeRepository.GetTipsByDate(startDate, endDate);
        }

    }
}
