    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MongoDB.Driver;
    using Roomantic_BoardingHouseSystem.Models;
    using Roomantic_BoardingHouseSystem.Services;

    namespace Roomantic_BoardingHouseSystem.Controllers
    {
        [Authorize(Roles = "Tenant")]
        public class TenantController : Controller
        {
            private readonly IMongoCollection<TenantModel> _tenants;
            private readonly IMongoCollection<RoomModel> _rooms;
            private readonly IMongoCollection<PaymentModel> _payments;
            private readonly IMongoCollection<RequestModel> _requests;

            public TenantController(MongoDbService db)
            {
                _tenants = db.Tenants;
                _rooms = db.Rooms;
                _payments = db.Payments;
                _requests = db.Requests;
            }

            // ==========================
            // DASHBOARD
            // ==========================
            public async Task<IActionResult> Dashboard()
            {
                var username = User.Identity?.Name;

                var tenant = await _tenants.Find(t => t.Username == username).FirstOrDefaultAsync();
                var room = tenant.RoomId != null
                    ? await _rooms.Find(r => r.Id == tenant.RoomId).FirstOrDefaultAsync()
                    : null;

                return View(new
                {
                    Tenant = tenant,
                    Room = room
                });
            }

            // ==========================
            // PROFILE PAGE
            // ==========================
            public async Task<IActionResult> Profile()
            {
                var username = User.Identity?.Name;
                var tenant = await _tenants.Find(t => t.Username == username).FirstOrDefaultAsync();

                return View(tenant);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> UpdateProfile(TenantModel profile)
            {
                if (!ModelState.IsValid)
                    return RedirectToAction("Profile");

                var username = User.Identity!.Name;

                await _tenants.UpdateOneAsync(
                    t => t.Username == username,
                    Builders<TenantModel>.Update
                        .Set(t => t.FullName, profile.FullName)
                        .Set(t => t.Gender, profile.Gender)
                        .Set(t => t.Age, profile.Age)
                        .Set(t => t.Address, profile.Address)
                        .Set(t => t.ContactNumber, profile.ContactNumber)
                );

                return RedirectToAction("Profile");
            }

            // ==========================
            // PAYMENTS
            // ==========================
            public async Task<IActionResult> MyPayments()
            {
                var username = User.Identity?.Name;

                var tenant = await _tenants.Find(t => t.Username == username).FirstOrDefaultAsync();

                var payments = await _payments.Find(p => p.TenantId == tenant.Id)
                                              .SortByDescending(p => p.PaymentDate)
                                              .ToListAsync();

                return View(payments);
            }

            // ==========================
            // REQUESTS
            // ==========================
            public async Task<IActionResult> Requests()
            {
                var username = User.Identity?.Name;

                var tenant = await _tenants.Find(t => t.Username == username).FirstOrDefaultAsync();

                var requests = await _requests.Find(r => r.TenantId == tenant.Id)
                                              .SortByDescending(r => r.DateFiled)
                                              .ToListAsync();

                return View(requests);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> SubmitRequest(RequestModel request)
            {
                var username = User.Identity!.Name;
                var tenant = await _tenants.Find(t => t.Username == username).FirstOrDefaultAsync();

                request.Id = null;
                request.TenantId = tenant.Id!;
                request.RoomId = tenant.RoomId!;
                request.DateFiled = DateTime.Now;
                request.Status = "Pending";

                await _requests.InsertOneAsync(request);

                return RedirectToAction("Requests");
            }
        }
    }
