using System.Collections.Generic;

namespace Roomantic_BoardingHouseSystem.Models
{
    public class ReportsMonitoringViewModel
    {
        public decimal MonthlyIncome { get; set; }
        public int UnpaidTenantsCount { get; set; }
        public int OccupiedRooms { get; set; }
        public int TotalRooms { get; set; }
        public int MaintenanceRequests { get; set; }

        public List<ReportModel> GeneratedReports { get; set; } = new();
    }
}
