using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IKitchenRepository
    {
        List<Kitchen> GetAll();
        Kitchen? GetById(int OrderNumber);

       
    }
}
