using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Repositories;

namespace shappo0.Services
{
    public class BarService : IBarService
    {
        private readonly IBarRepository _barRepository;

        public BarService(IBarRepository barRepository)
        {
            _barRepository = barRepository;
        }

        public List<Bar> GetAll()
        {
            return _barRepository.GetAll();
        }

        public Bar? GetById(int OrderNumber)
        {
            return _barRepository.GetById(OrderNumber);
           
        }
    }
}
