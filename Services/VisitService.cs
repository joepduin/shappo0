using shappo0.Models;
using shappo0.Repositories;

namespace shappo0.Services
{
    public class VisitService : IVisitService
    {

        private IVisitRepository _visitRepository;

        public VisitService(IVisitRepository visitRepository)
        {  
            _visitRepository = visitRepository;
        }

        public Visit? GetById(int id)
        {
            return _visitRepository.GetById(id);
        }


        public Visit GetActiveVisitForTable(int tableNumber)
        {
            return _visitRepository.GetActiveVisitForTable(tableNumber);
        }


        public int StartNewVisit(Visit visit)
        {
            return _visitRepository.StartNewVisit(visit);
        }

        public void CloseVisit(int visitId)
        {
            _visitRepository.CloseVisit(visitId);
        }
    }
}
