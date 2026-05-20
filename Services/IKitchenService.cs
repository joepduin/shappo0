using shappo0.Models;

namespace shappo0.Services
{
    public interface IKitchenService 
    {
        List<Kitchen> GetAll();
        Kitchen? GetById(int OrderNumber);
    }
}
