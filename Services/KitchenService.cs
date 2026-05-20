using shappo0.Models;
using shappo0.Repositories;

namespace shappo0.Services
{
    public class KitchenService : IKitchenService
    {
        private readonly IKitchenRepository _kitchenRepository;

        public KitchenService(IKitchenRepository kitchenRepository)
        {
            _kitchenRepository = kitchenRepository;
        }

        public List<Kitchen> GetAll()
        {
            return _kitchenRepository.GetAll();
        }

        public Kitchen? GetById(int OrderNumber)
        {
            return _kitchenRepository.GetById(OrderNumber);

        }
    }
}
