namespace Roomantic_BoardingHouseSystem.Models
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;

        public string AdminAccountsCollectionName { get; set; } = string.Empty;
        public string TenantsCollectionName { get; set; } = string.Empty;
        public string RoomsCollectionName { get; set; } = string.Empty;
        public string PaymentsCollectionName { get; set; } = string.Empty;
        public string RequestsCollectionName { get; set; } = string.Empty;
        public string ReportsCollectionName { get; set; } = string.Empty;
        public string NotificationsCollectionName { get; set; } = string.Empty;
    }
}
