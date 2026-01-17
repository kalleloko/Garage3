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

namespace Garage3.Controllers
{
    public class ParkingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParkingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Parkings
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Index(string searchType = null, string searchRegNo = null)
        {
            //var query = _context.Parkings
            //   .Include(p => p.Vehicle)
            //       .ThenInclude(v => v.Type)
            //   .Include(p => p.Vehicle)
            //       .ThenInclude(v => v.Owner)
            //   .Include(p => p.ParkingSpot)
            //   .Where(p => p.DepartTime == null) // only active
            //   .AsQueryable();

            var query = _context.Parkings
                        .Include(p => p.Vehicle)
                        .ThenInclude(v => v.Type)
                        .Include(p => p.Vehicle)
                        .ThenInclude(v => v.Owner)
                        .Include(p => p.ParkingSpot)
                        .Where(p => p.DepartTime == null)
                        .AsQueryable();
            if (!string.IsNullOrEmpty(searchType))
            {
                query = query.Where(p => p.Vehicle.Type.Name.Contains(searchType));
            }

            if (!string.IsNullOrEmpty(searchRegNo))
            {
                query = query.Where(p => p.Vehicle.RegistrationNumber.Contains(searchRegNo));
            }

            var model = await query
                       .Select(p => new AdminParkedVehicleViewModel
                       {
                         OwnerName = p.Vehicle.Owner.UserName,
                         VehicleType = p.Vehicle.Type.Name,
                         RegistrationNumber = p.Vehicle.RegistrationNumber,
                         ParkingSpotNumber = p.ParkingSpot.SpotNumber,
                         ArrivalTime = p.ArrivalTime,
                         ParkedSince = DateTime.Now - p.ArrivalTime,
                       }).ToListAsync();


            var applicationDbContext = _context.Parkings.Include(p => p.ParkingSpot).Include(p => p.Vehicle);
            return View(model);
        }

        // GET: Parkings/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var parking = await _context.Parkings
        //        .Include(p => p.ParkingSpot)
        //        .Include(p => p.Vehicle)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (parking == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(parking);
        //}

        //// GET: Parkings/Create
        //public IActionResult Create()
        //{
        //    ViewData["ParkingSpotId"] = new SelectList(_context.ParkingSpots, "Id", "Id");
        //    ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "RegistrationNumber");
        //    return View();
        //}

        //// POST: Parkings/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,VehicleId,ParkingSpotId,ArrivalTime,DepartTime")] Parking parking)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(parking);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ParkingSpotId"] = new SelectList(_context.ParkingSpots, "Id", "Id", parking.ParkingSpotId);
        //    ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "RegistrationNumber", parking.VehicleId);
        //    return View(parking);
        //}

        //// GET: Parkings/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var parking = await _context.Parkings.FindAsync(id);
        //    if (parking == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ParkingSpotId"] = new SelectList(_context.ParkingSpots, "Id", "Id", parking.ParkingSpotId);
        //    ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "RegistrationNumber", parking.VehicleId);
        //    return View(parking);
        //}

        //// POST: Parkings/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,VehicleId,ParkingSpotId,ArrivalTime,DepartTime")] Parking parking)
        //{
        //    if (id != parking.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(parking);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ParkingExists(parking.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ParkingSpotId"] = new SelectList(_context.ParkingSpots, "Id", "Id", parking.ParkingSpotId);
        //    ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "RegistrationNumber", parking.VehicleId);
        //    return View(parking);
        //}

        //// GET: Parkings/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var parking = await _context.Parkings
        //        .Include(p => p.ParkingSpot)
        //        .Include(p => p.Vehicle)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (parking == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(parking);
        //}

        //// POST: Parkings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var parking = await _context.Parkings.FindAsync(id);
        //    if (parking != null)
        //    {
        //        _context.Parkings.Remove(parking);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ParkingExists(int id)
        //{
        //    return _context.Parkings.Any(e => e.Id == id);
        //}
    }
}
