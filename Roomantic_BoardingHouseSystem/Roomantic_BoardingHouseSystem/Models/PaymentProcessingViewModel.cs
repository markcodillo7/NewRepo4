using System.Collections.Generic;

namespace Roomantic_BoardingHouseSystem.Models
{
    public class PaymentProcessingViewModel
    {
        public List<PaymentModel> PaymentRecords { get; set; } = new();
    }
}
