using shappo0.Models;

namespace shappo0.ViewModels
{
    public class TableViewModel
    {
        public int OrderNumber { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderLine> OrderLines { get; set; }

        public TableViewModel()
        {
        }

        public TableViewModel(int orderNumber, OrderStatus status, List<OrderLine> orderLines)
        {
            OrderNumber = orderNumber;
            Status = status;
            OrderLines = orderLines;
        }
    }
}
