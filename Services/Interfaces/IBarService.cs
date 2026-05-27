using shappo0.Models;

namespace shappo0.Services
{
    public interface IBarService
    {
        List<Bar> GetAll();
        Bar? GetById(int OrderNumber);
    }
}
