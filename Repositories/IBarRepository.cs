using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IBarRepository
    {
        List<Bar> GetAll();
        Bar? GetById(int OrderNumber);


    }
}
