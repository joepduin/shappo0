using shappo0.Models;

namespace shappo0.Repositories.Interfaces
{
    public interface IBillRepository
    {
        int SaveBill(Bill bill);
        Bill GetBillByVisitId(int visitId);
        void UpdateBillPaid(int billId, bool isPaid);
    }
}
