using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage3.Data;
using Garage3.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Garage3.Controllers
{
    [Authorize(Roles = "Member,Admin")]
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IQueryable<Vehicle> applicationDbContext = _context.Vehicles
                                        .Include(v => v.Owner)
                                        .Include(v => v.Parkings)
                                        .Include(v => v.Type);
            if(!User.IsInRole("Admin"))
            {
                applicationDbContext = applicationDbContext.Where(v => v.Owner.Id == userId);
            }
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Owner)
                .Include(v => v.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            //ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();
                vehicle.OwnerId = userId;
                _context.Add(vehicle);
                 await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "UserName", vehicle.OwnerId);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name", vehicle.VehicleTypeId);
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            //ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "UserName", vehicle.OwnerId);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name", vehicle.VehicleTypeId);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }
            var existingVehicle = await _context.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            if (existingVehicle == null)
                return NotFound();

            vehicle.OwnerId = existingVehicle.OwnerId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "UserName", vehicle.OwnerId);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name", vehicle.VehicleTypeId);
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Owner)
                .Include(v => v.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Park(int? id)
        {
            if (id == null) return NotFound();

            var vehicle = await _context.Vehicles
                                .Include(v => v.Parkings)
                                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
                return NotFound();

            var activeParking = vehicle.Parkings.Any(p => p.DepartTime == null);
            if (activeParking)
                return BadRequest("Vehicle is already parked");

            var freeSpots = await _context.ParkingSpots
                                   .Where(ps => !ps.IsBlocked 
                                    && !_context.Parkings.Any(p => p.ParkingSpotId == ps.Id && p.DepartTime == null))
                                   .Select(p => new SelectListItem
                                   {
                                       Value = p.Id.ToString(),
                                       Text = p.SpotNumber
                                   })
                                   .ToListAsync();

            var vm = new UserParkVehicleViewModel
            {
                VehicleId = vehicle.Id,
                RegistrationNumber = vehicle.RegistrationNumber,
                FreeParkingSpots = freeSpots,
            };
            ViewData["ParkingSpotId"] = new SelectList(freeSpots, "Id", "Number");
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Park(UserParkVehicleViewModel model)
        {
            
            var vehicle = await _context.Vehicles
                        .Include(v => v.Parkings)
                        .FirstOrDefaultAsync(v => v.Id == model.VehicleId);

            if (vehicle == null)
                return NotFound();

            if (vehicle.Parkings.Any(p => p.DepartTime == null))
                return BadRequest("Vehicle is already parked");



            if (!ModelState.IsValid)
            {
                model.RegistrationNumber = vehicle.RegistrationNumber;
                model.FreeParkingSpots = await _context.ParkingSpots
                                        .Where(ps => !ps.IsBlocked 
                                         && !_context.Parkings.Any(p => p.ParkingSpotId == ps.Id && p.DepartTime == null)) 
                                        .Select(ps => new SelectListItem
                                        {
                                         Value = ps.Id.ToString(),
                                         Text = ps.SpotNumber
                                         })
                                        .ToListAsync();

                return View(model);
            }
            //  make sure spot exists and is free
            var spotExists = await _context.ParkingSpots
                    .AnyAsync(ps => ps.Id == model.ParkingSpotId
                    && !ps.IsBlocked
                    && !_context.Parkings.Any(p => p.ParkingSpotId == ps.Id && p.DepartTime == null));

            if (!spotExists)
            {
                //ModelState.AddModelError(nameof(model.ParkingSpotId), "Selected parking spot is no longer available.");
                model.RegistrationNumber = vehicle.RegistrationNumber;
                model.FreeParkingSpots = await _context.ParkingSpots
                    .Where(ps => !_context.Parkings.Any(p => p.ParkingSpotId == ps.Id && p.DepartTime == null) && !ps.IsBlocked)
                    .Select(ps => new SelectListItem { Value = ps.Id.ToString(), Text = ps.SpotNumber })
                    .ToListAsync();

                return View(model);
            }


            var parking = new Parking
            {
                VehicleId = model.VehicleId,
                ParkingSpotId = model.ParkingSpotId,
                ArrivalTime = DateTime.Now,
                DepartTime = null,
            };

            _context.Parkings.Add(parking);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Vehicles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(int vehicleId)
        {
            // Load vehicle and active parking
            var vehicle = await _context.Vehicles
                .Include(v => v.Parkings)
                .ThenInclude(p => p.ParkingSpot)
                .FirstOrDefaultAsync(v => v.Id == vehicleId);

            if (vehicle == null)
                return NotFound();

            // Find active parking
            var activeParking = vehicle.Parkings.FirstOrDefault(p => p.DepartTime == null);

            if (activeParking == null)
                return BadRequest("Vehicle is not currently parked");

            // Set departure time
            activeParking.DepartTime = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
