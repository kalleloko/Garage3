using Garage3.Data;
using Garage3.Helpers;
using Garage3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Garage3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            const double hourlyPrice = 20.0;
            var users = await _context.Users
                        .Include(u => u.Vehicles)
                        .ThenInclude(v => v.Parkings)
                        .ToListAsync();

            var model = users.Select(u => new MemberListViewModel
            {
                MemberId = u.Id,
                MemberName = u.FirstName + " " + u.LastName,
                VehicleCount = u.Vehicles.Count,
                TotalParkingCost = u.Vehicles
                                  .SelectMany(v => v.Parkings)
                                  .Where(p => p.DepartTime != null)
                                  .Sum(p => p.CalculatePrice(hourlyPrice)),
            }).ToList();
            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            const double hourlyPrice = 20.0;

            var member = await _context.Users
                .Include(u => u.Vehicles)
                    .ThenInclude(v => v.Parkings)
                    .ThenInclude(v => v.ParkingSpot)
                .Include(u => u.Vehicles)
                    .ThenInclude(v => v.Type)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (member == null)
                return NotFound();

            var vm = new MemberDetailsViewModel
            {
                MemberName = member.UserName,

                Vehicles = member.Vehicles.Select(v => new MemberVehicleViewModel
                {
                    RegistrationNumber = v.RegistrationNumber,
                    VehicleType = v.Type.Name,
                    Brand = v.Brand,
                    ParkingStatus = v.ActiveParking != null ? v.ActiveParking.ParkingSpot.SpotNumber : "",
                    ArrivalTime = v.ActiveParking?.ArrivalTime,

                    AccumulatedCost = v.Parkings
                        .Where(p => p.DepartTime != null)
                        .Sum(p => p.CalculatePrice(hourlyPrice))
                }).ToList()
            };

            return View(vm);
        }



    }
}
