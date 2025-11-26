using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Roomantic_BoardingHouseSystem.Models
{
    public class ReportModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string ReportType { get; set; } = string.Empty; // e.g., MonthlyIncome, RoomStatus
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Used in ReportsMonitoring
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
    }
}
