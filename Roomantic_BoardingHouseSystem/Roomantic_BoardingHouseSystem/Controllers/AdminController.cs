using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Roomantic_BoardingHouseSystem.Models;
using Roomantic_BoardingHouseSystem.Services;

namespace Roomantic_BoardingHouseSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IMongoCollection<RoomModel> _rooms;
        private readonly IMongoCollection<TenantModel> _tenants;
        private readonly IMongoCollection<PaymentModel> _payments;
        private readonly IMongoCollection<RequestModel> _requests;
        private readonly IMongoCollection<ReportModel> _reports;
        private readonly IMongoCollection<AdminAccountModel> _admins;

        public AdminController(MongoDbService db)
        {
            _rooms = db.Rooms;
            _tenants = db.Tenants;
            _payments = db.Payments;
            _requests = db.Requests;
            _reports = db.Reports;
            _admins = db.AdminAccounts;
        }
        // =======================
        // ADMIN DASHBOARD
        // =======================
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Dashboard";

            // Dashboard Counts
            var totalTenants = await _tenants.CountDocumentsAsync(_ => true);
            var occupiedRooms = await _rooms.CountDocumentsAsync(r => r.Status == "Occupied");
            var pendingRequests = await _requests.CountDocumentsAsync(r => r.Status == "Pending");
            var overduePayments = await _payments.CountDocumentsAsync(p => p.Status == "Overdue");

            // Recent activities (optional future use)
            var recentActivities = await _payments.Find(_ => true)
                                                  .SortByDescending(p => p.PaymentDate)
                                                  .Limit(10)
                                                  .ToListAsync();

            // Prepare data for the view
            var dashboardData = new
            {
                TotalTenants = totalTenants,
                OccupiedRooms = occupiedRooms,
                PendingRequests = pendingRequests,
                OverduePayments = overduePayments,
                RecentActivities = recentActivities
            };

            return View(dashboardData);
        }


        // =======================
        // ROOM MANAGEMENT (LIST)
        // =======================
        public async Task<IActionResult> RoomManagement()
        {
            ViewData["Title"] = "Room Module";

            var rooms = await _rooms.Find(_ => true)
                                    .SortBy(r => r.RoomNumber)
                                    .ToListAsync();

            return View(new RoomManagementViewModel
            {
                Rooms = rooms
            });
        }

        // =======================
        // ADD ROOM
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoom(RoomModel room)
        {
            if (!ModelState.IsValid)
            {
                var rooms = await _rooms.Find(_ => true).ToListAsync();
                return View("RoomManagement", new RoomManagementViewModel { Rooms = rooms });
            }

            if (string.IsNullOrWhiteSpace(room.Status))
                room.Status = "Vacant";

            room.Status = char.ToUpper(room.Status[0]) + room.Status.Substring(1).ToLower();

            room.Id = null;

            await _rooms.InsertOneAsync(room);

            return RedirectToAction("RoomManagement");
        }

        // =======================
        // UPDATE ROOM
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoom(RoomModel room)
        {
            if (string.IsNullOrEmpty(room.Id))
                return RedirectToAction("RoomManagement");

            var filter = Builders<RoomModel>.Filter.Eq(r => r.Id, room.Id);

            var update = Builders<RoomModel>.Update
                .Set(r => r.RoomNumber, room.RoomNumber)
                .Set(r => r.RoomType, room.RoomType)
                .Set(r => r.Capacity, room.Capacity)
                .Set(r => r.CurrentOccupants, room.CurrentOccupants)
                .Set(r => r.RentPrice, room.RentPrice)
                .Set(r => r.Deposit, room.Deposit)
                .Set(r => r.Status, room.Status)
                .Set(r => r.Description, room.Description);

            await _rooms.UpdateOneAsync(filter, update);

            return RedirectToAction("RoomManagement");
        }

        // =======================
        // ARCHIVE ROOM
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveRoom(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("RoomManagement");

            await _rooms.UpdateOneAsync(
                r => r.Id == id,
                Builders<RoomModel>.Update.Set(r => r.Status, "Maintenance"));

            return RedirectToAction("RoomManagement");
        }

        // =======================
        // DELETE ROOM
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("RoomManagement");

            var room = await _rooms.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (room == null)
                return RedirectToAction("RoomManagement");

            // Check tenants by RoomId relation
            var hasTenants = await _tenants.CountDocumentsAsync(t => t.RoomId == room.Id) > 0;

            if (hasTenants)
            {
                TempData["ErrorMessage"] = "Cannot delete this room because it has assigned tenants.";
                return RedirectToAction("RoomManagement");
            }

            await _rooms.DeleteOneAsync(r => r.Id == id);

            return RedirectToAction("RoomManagement");
        }

        // =======================
        // TENANT REGISTRATION
        // =======================
        public async Task<IActionResult> TenantRegistration()
        {
            ViewData["Title"] = "Tenant Registration";

            var availableRooms = await _rooms.Find(r => r.Status == "Vacant").ToListAsync();
            var tenants = await _tenants.Find(_ => true).ToListAsync();

            return View(new TenantRegistrationViewModel
            {
                AvailableRooms = availableRooms,
                Tenants = tenants
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTenant(TenantModel tenant)
        {
            if (!ModelState.IsValid)
            {
                var availableRooms = await _rooms.Find(r => r.Status == "Vacant").ToListAsync();
                var tenants = await _tenants.Find(_ => true).ToListAsync();

                return View("TenantRegistration", new TenantRegistrationViewModel
                {
                    AvailableRooms = availableRooms,
                    Tenants = tenants
                });
            }

            tenant.Status ??= "Active";
            tenant.MoveInDate ??= DateTime.Now;

            // If RoomNumber is chosen, map to RoomId and set room status
            if (!string.IsNullOrEmpty(tenant.RoomNumber))
            {
                var room = await _rooms.Find(r => r.RoomNumber == tenant.RoomNumber).FirstOrDefaultAsync();
                if (room != null)
                {
                    tenant.RoomId = room.Id;

                    await _rooms.UpdateOneAsync(
                        r => r.Id == room.Id,
                        Builders<RoomModel>.Update.Set(r => r.Status, "Occupied"));
                }
            }

            await _tenants.InsertOneAsync(tenant);

            return RedirectToAction("TenantRegistration");
        }

        // =======================
        // PAYMENT PROCESSING
        // =======================
        public async Task<IActionResult> PaymentProcessing()
        {
            ViewData["Title"] = "Payment Processing";

            var payments = await _payments.Find(_ => true)
                                          .SortByDescending(p => p.PaymentDate)
                                          .ToListAsync();

            return View(new PaymentProcessingViewModel { PaymentRecords = payments });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(PaymentModel payment)
        {
            if (!ModelState.IsValid)
            {
                var payments = await _payments.Find(_ => true).ToListAsync();
                return View("PaymentProcessing", new PaymentProcessingViewModel { PaymentRecords = payments });
            }

            payment.PaymentDate ??= DateTime.Now;
            payment.Status ??= "Paid";

            await _payments.InsertOneAsync(payment);

            return RedirectToAction("PaymentProcessing");
        }

        // =======================
        // REQUEST HANDLING
        // =======================
        public async Task<IActionResult> RequestHandling()
        {
            ViewData["Title"] = "Request Handling";

            var requests = await _requests.Find(_ => true)
                                          .SortByDescending(r => r.DateFiled)
                                          .ToListAsync();

            return View(new RequestHandlingViewModel { Requests = requests });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRequest(RequestModel request)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(request.Id))
            {
                var reqs = await _requests.Find(_ => true).ToListAsync();
                return View("RequestHandling", new RequestHandlingViewModel { Requests = reqs });
            }

            await _requests.UpdateOneAsync(
                r => r.Id == request.Id,
                Builders<RequestModel>.Update
                    .Set(r => r.Status, request.Status)
                    .Set(r => r.Notes, request.Notes));

            return RedirectToAction("RequestHandling");
        }

        // =======================
        // REPORTS & MONITORING
        // =======================
        public async Task<IActionResult> ReportsMonitoring()
        {
            ViewData["Title"] = "Reports & Monitoring";

            var reports = await _reports.Find(_ => true)
                                        .SortByDescending(r => r.GeneratedDate)
                                        .ToListAsync();

            var totalRooms = await _rooms.CountDocumentsAsync(_ => true);
            var occupiedRooms = await _rooms.CountDocumentsAsync(r => r.Status == "Occupied");
            var maintenanceRequests = await _requests.CountDocumentsAsync(r => r.Category == "Maintenance");
            var unpaidTenants = await _payments.CountDocumentsAsync(p => p.Status == "Unpaid" || p.Status == "Overdue");

            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);

            var monthlyPayments = await _payments.Find(
                p => p.Status == "Paid" &&
                     p.PaymentDate >= startOfMonth &&
                     p.PaymentDate < endOfMonth).ToListAsync();

            decimal monthlyIncome = monthlyPayments.Sum(p => p.Amount);

            return View(new ReportsMonitoringViewModel
            {
                MonthlyIncome = monthlyIncome,
                UnpaidTenantsCount = (int)unpaidTenants,
                OccupiedRooms = (int)occupiedRooms,
                TotalRooms = (int)totalRooms,
                MaintenanceRequests = (int)maintenanceRequests,
                GeneratedReports = reports
            });
        }

        // =======================
        // ADMIN ACCOUNT MANAGEMENT
        // =======================
        public async Task<IActionResult> AdminManagement()
        {
            ViewData["Title"] = "Admin Management";

            var username = User.Identity?.Name;
            var admin = await _admins.Find(a => a.Username == username).FirstOrDefaultAsync();

            return View(new AdminAccountModel
            {
                FullName = admin?.FullName ?? "Admin User",
                Email = admin?.Email ?? "",
                Phone = admin?.Phone ?? "",
                LoginHistory = new List<LoginHistoryModel>()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUsername(string newUsername)
        {
            if (string.IsNullOrWhiteSpace(newUsername))
                return RedirectToAction("AdminManagement");

            var username = User.Identity?.Name;

            await _admins.UpdateOneAsync(
                a => a.Username == username,
                Builders<AdminAccountModel>.Update.Set(a => a.Username, newUsername));

            return RedirectToAction("AdminManagement");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
                return RedirectToAction("AdminManagement");

            var username = User.Identity?.Name;
            var admin = await _admins.Find(a => a.Username == username).FirstOrDefaultAsync();

            if (admin == null)
                return RedirectToAction("AdminManagement");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, admin.PasswordHash))
                return RedirectToAction("AdminManagement");

            var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _admins.UpdateOneAsync(
                a => a.Id == admin.Id,
                Builders<AdminAccountModel>.Update.Set(a => a.PasswordHash, newHash));

            return RedirectToAction("AdminManagement");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(AdminProfileModel profile)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("AdminManagement");

            var username = User.Identity?.Name;

            await _admins.UpdateOneAsync(
                a => a.Username == username,
                Builders<AdminAccountModel>.Update
                    .Set(a => a.FullName, profile.FullName)
                    .Set(a => a.Email, profile.Email)
                    .Set(a => a.Phone, profile.Phone));

            return RedirectToAction("AdminManagement");
        }
    }
}
