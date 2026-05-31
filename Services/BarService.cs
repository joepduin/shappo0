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

        public List<OrderLine> GetOrderLines(int orderNumber)
        {
            return _barRepository.GetOrderLines(orderNumber);
        }

        public void UpdateOrderStatus(int orderNumber, OrderStatus status)
        {
            _barRepository.UpdateOrderStatus(orderNumber, status);
        }

        public void UpdateOrderLineStatus(int orderNumber, OrderStatus status)
        {
            _barRepository.UpdateOrderLineStatus(orderNumber, status);
        }

        public List<Bar> FinishedOrders()
        {
            return _barRepository.FinishedOrders();
        }
    }
}
