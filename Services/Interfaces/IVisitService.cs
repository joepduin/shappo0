using shappo0.Models;

namespace shappo0.Services
{
    public interface IVisitService
    {
        public Visit? GetById(int id);
        public Visit? GetActiveVisitForTable(int tableNumber);
        public int StartNewVisit(Visit visit);
        public void CloseVisit(int visitId);
    }
}
