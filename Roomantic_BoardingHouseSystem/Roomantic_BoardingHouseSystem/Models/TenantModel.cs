using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Roomantic_BoardingHouseSystem.Models
{
    public class TenantModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public int Age { get; set; }

        public string? Address { get; set; }
        public string? ContactNumber { get; set; }

        public string Status { get; set; } = "Active";

        public DateTime? MoveInDate { get; set; }

        // RELATION: to Room
        [BsonRepresentation(BsonType.ObjectId)]
        public string? RoomId { get; set; }

        // For convenience / display (used in your views and controller)
        public string? RoomNumber { get; set; }

        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Tenant";
    }
}
