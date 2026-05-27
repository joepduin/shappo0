using shappo0.Models;
using shappo0.ViewModels;

namespace shappo0.Extensions
{
    public static class StatusExtensions
    {
        public static TableOrderStatus ToTableStatus(this OrderLine? orderLine)
        {
            if (orderLine != null)
            {
                if (orderLine.Status == OrderStatus.Running) return TableOrderStatus.Ordered;
                else if (orderLine.Status == OrderStatus.Preparing) return TableOrderStatus.BeingPrepared;
                else if (orderLine.Status == OrderStatus.Completed) return TableOrderStatus.ReadyToBeServed;
            }
            return TableOrderStatus.Unknown;
        }

        public static TableOrderStatus ToTableStatus(this TableViewModel? tableViewModel)
        {
            if (tableViewModel != null)
            {
                if (tableViewModel.Status == OrderStatus.Running) return TableOrderStatus.Ordered;
                else if (tableViewModel.Status == OrderStatus.Preparing) return TableOrderStatus.BeingPrepared;
                else if (tableViewModel.Status == OrderStatus.Completed) return TableOrderStatus.ReadyToBeServed;
            }
            return TableOrderStatus.Unknown;
        }

        public static TableOrderStatus ToTableStatus(this OrderStatus orderStatus)
        {
            if (orderStatus == OrderStatus.Running) return TableOrderStatus.Ordered;
            else if (orderStatus == OrderStatus.Preparing) return TableOrderStatus.BeingPrepared;
            else if (orderStatus == OrderStatus.Completed) return TableOrderStatus.ReadyToBeServed;
            return TableOrderStatus.Unknown;
        }
    }
}
