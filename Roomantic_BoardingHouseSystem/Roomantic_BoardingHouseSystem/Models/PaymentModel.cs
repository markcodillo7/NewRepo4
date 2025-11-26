using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Roomantic_BoardingHouseSystem.Models
{
    public class PaymentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string TenantId { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string RoomId { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        // Used in ReportsMonitoring & PaymentProcessing
        public DateTime? PaymentDate { get; set; }

        public string PaymentMethod { get; set; } = "Cash";       // e.g. Cash, InstaPay
        public string PaymentType { get; set; } = "Monthly Rent"; // Rent, Deposit, etc.
        public string Status { get; set; } = "Paid";              // Paid, Unpaid, Overdue
    }
}
