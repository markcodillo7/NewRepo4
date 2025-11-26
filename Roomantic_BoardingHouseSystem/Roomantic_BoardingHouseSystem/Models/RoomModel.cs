using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Roomantic_BoardingHouseSystem.Models
{
    public class RoomModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;

        // Used in UpdateRoom
        public int Capacity { get; set; } = 1;
        public int CurrentOccupants { get; set; } = 0;

        public decimal RentPrice { get; set; }
        public decimal Deposit { get; set; }

        public string Status { get; set; } = "Vacant"; // Vacant / Occupied / Maintenance

        public string Description { get; set; } = string.Empty;
    }
}
