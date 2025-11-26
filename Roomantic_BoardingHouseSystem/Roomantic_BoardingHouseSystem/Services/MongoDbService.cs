using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Roomantic_BoardingHouseSystem.Models;

namespace Roomantic_BoardingHouseSystem.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public IMongoCollection<RoomModel> Rooms { get; }
        public IMongoCollection<TenantModel> Tenants { get; }
        public IMongoCollection<PaymentModel> Payments { get; }
        public IMongoCollection<RequestModel> Requests { get; }
        public IMongoCollection<ReportModel> Reports { get; }
        public IMongoCollection<AdminAccountModel> AdminAccounts { get; }

        public MongoDbService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);

            Rooms = _database.GetCollection<RoomModel>("rooms");
            Tenants = _database.GetCollection<TenantModel>("tenants");
            Payments = _database.GetCollection<PaymentModel>("payments");
            Requests = _database.GetCollection<RequestModel>("requests");
            Reports = _database.GetCollection<ReportModel>("reports");
            AdminAccounts = _database.GetCollection<AdminAccountModel>("admin_accounts");
        }
    }
}
