using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IVisitRepository
    {
        public Visit? GetById(int id);

        public Visit? GetActiveVisitForTable(int tableNumber);
        public int StartNewVisit(Visit newVisit);
        public void CloseVisit(int visitId);

    }
}
