using System.Collections.Generic;

namespace Roomantic_BoardingHouseSystem.Models
{
    public class TenantRegistrationViewModel
    {
        public List<RoomModel> AvailableRooms { get; set; } = new();
        public List<TenantModel> Tenants { get; set; } = new();
    }
}
