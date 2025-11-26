using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Roomantic_BoardingHouseSystem.Models
{
    public class RequestModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string TenantId { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string RoomId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = "Maintenance";

        public string Status { get; set; } = "Pending";

        // Used by RequestHandling sorting:
        public DateTime DateFiled { get; set; } = DateTime.UtcNow;

        // Used in UpdateRequest:
        public string? Notes { get; set; }
    }
}
