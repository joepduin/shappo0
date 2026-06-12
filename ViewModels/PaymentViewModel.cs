using shappo0.Enums;
using shappo0.Models;

namespace shappo0.ViewModels
{

    public class PaymentViewModel
    {
        public int BillId { get; set; }
        public int TableNumber { get; set; }
        public int NumberOfPeoplePayed { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal AmountToPay { get; set; }
        public decimal OutstandingAmount { get; set; }
        public SplitChoice SplitChoice { get; set; }

        public PaymentViewModel()
        {

        }

        public PaymentViewModel(int billId, int tableNumber, int numberOfPeoplePayed, int numberOfPeople, decimal amountToPay, decimal outstandingAmount, SplitChoice splitChoice)
        {
            BillId = billId;
            TableNumber = tableNumber;
            NumberOfPeoplePayed = numberOfPeoplePayed;
            NumberOfPeople = numberOfPeople;
            AmountToPay = amountToPay;
            OutstandingAmount = outstandingAmount;
            SplitChoice = splitChoice;
        }
    }
}