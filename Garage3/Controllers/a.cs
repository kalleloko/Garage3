using Garage3.Data;
using Garage3.Exceptions;
using Garage3.Models;
using Garage3.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Garage3.Controllers
{
    public class a : Controller
    {
        private readonly VehicleService _vehicleService;

        public a(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            return View(await _vehicleService.GetAllAsync());
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _vehicleService.GetByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            ViewData["VehicleTypeList"] = new SelectList(_vehicleService.GetVehicleTypes());
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            VehicleRules.FormatVehicle(vehicle);
            try
            {
                await _vehicleService.AddAsync(vehicle);
                TempData["Message"] = $"Vehicle {vehicle.RegistrationNumber} checked in successfully";
                return RedirectToAction(nameof(HomePage));
            }
            catch (DuplicateRegistrationNumberException ex)
            {
                ModelState.AddModelError(
                    nameof(vehicle.RegistrationNumber),
                    ex.Message
                );
            }

            ViewData["VehicleTypeList"] = new SelectList(_vehicleService.GetVehicleTypes(), vehicle.Type);
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _vehicleService.GetByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["VehicleTypeList"] = new SelectList(_vehicleService.GetVehicleTypes(), vehicle.Type);
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

            if (ModelState.IsValid)
            {
                VehicleRules.FormatVehicle(vehicle);
                try
                {
                    await _vehicleService.UpdateAsync(vehicle);
                    TempData["Message"] = $"Vehicle {vehicle.RegistrationNumber} updated successfully";
                    return RedirectToAction(nameof(HomePage));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_vehicleService.VehicleExists(vehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DuplicateRegistrationNumberException ex)
                {
                    ModelState.AddModelError(
                        nameof(vehicle.RegistrationNumber),
                        ex.Message
                    );
                }
                ViewData["VehicleTypeList"] = new SelectList(_vehicleService.GetVehicleTypes(), vehicle.Type);
                return View(vehicle);
            }
            return RedirectToAction(nameof(HomePage));
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _vehicleService.GetByIdAsync(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return View(GetVehicleDetailViewModel(vehicle));
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Receipt? receipt = await _vehicleService.DeleteVehicleAndCreateReceipt(id);

            return RedirectToAction(nameof(Receipt), receipt);
        }

        public async Task<IActionResult> Receipt(Receipt receipt)
        {
            if (receipt == null)
                return NotFound();
            return View(receipt);
        }

        //VehicleView
        public async Task<IActionResult> HomePage(int page = 1, string? search = null, string? sort = nameof(VehicleViewModel.ArrivalTime), string? type = "")
        {
            int pageSize = 20;

            //Keep track of current filter for the search box
            ViewData["CurrentFilter"] = search;

            // Determine sort order for each column (toggle ascending/descending)
            ViewData["CurrentSort"] = sort; // stores current column + direction
            ViewData["TypeSortParam"] = sort == "Type" ? "Type_desc" : "Type";
            ViewData["RegSortParam"] = sort == "RegistrationNumber" ? "RegistrationNumber_desc" : "RegistrationNumber";
            ViewData["ArrivalSortParam"] = sort == "ArrivalTime" ? "ArrivalTime_desc" : "ArrivalTime";
            ViewData["ParkingTimeSortParam"] = sort == "ParkingTime" ? "ParkingTime_desc" : "ParkingTime";

            ViewData["CurrentType"] = type;


            //Get all
            IQueryable<Vehicle> query = _vehicleService.GetAll();

            ViewData["TypesSelectList"] = query
                .GroupBy(v => new
                {
                    v.VehicleTypeId,
                    v.Type.Name
                })
                .OrderBy(g => g.Key.Name)
                .Select(g => new SelectListItem
                {
                    Text = $"{g.Key.Name} ({g.Count()})",
                    Value = g.Key.VehicleTypeId.ToString()
                })
                .ToList();

            // Apply search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(v =>
                    v.RegistrationNumber != null &&
                    v.RegistrationNumber.Contains(search));
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(v => v.Type != null && v.Type.Name == type);
            }

            Expression<Func<Vehicle, DateTime?>> ArrivalTimeExpression = (Vehicle v) =>
                    v.Parkings
                        .Where(p => p.DepartTime == null)
                        .Select(p => p.ArrivalTime)
                        .FirstOrDefault();
            // Sort
            query = sort switch
            {
                "Type" => query.OrderBy(v => v.Type.Name),
                "Type_desc" => query.OrderByDescending(v => v.Type.Name),
                "RegistrationNumber" => query.OrderBy(v => v.RegistrationNumber),
                "RegistrationNumber_desc" => query.OrderByDescending(v => v.RegistrationNumber),
                "ArrivalTime" => query.OrderBy(ArrivalTimeExpression),
                "ArrivalTime_desc" => query.OrderByDescending(ArrivalTimeExpression),
                "ParkingTime" => query.OrderByDescending(ArrivalTimeExpression),
                "ParkingTime_desc" => query.OrderBy(ArrivalTimeExpression),
                _ => query.OrderBy(ArrivalTimeExpression)
            };



            var totalItems = await query.CountAsync();

            var vehicles = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new VehicleViewModel
                {
                    Id = v.Id,
                    ArrivalTime = v.ArrivalTime,
                    RegistrationNumber = v.RegistrationNumber,
                    Type = v.Type,
                })
                .ToListAsync();

            var result = new PagedResult<VehicleViewModel>
            {
                Items = vehicles,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(result);
        }

        //ParkVehicleDetailView
        public async Task<IActionResult> VehicleDetail(int? id)
        {
            if (id == null)
                return NotFound();

            var vehicle = await _vehicleService.GetByIdAsync(id);

            if (vehicle == null)
                return NotFound();

            return View(GetVehicleDetailViewModel(vehicle));

        }

        private static VehicleDetailViewModel GetVehicleDetailViewModel(Vehicle vehicle)
        {
            return new VehicleDetailViewModel
            {
                Id = vehicle.Id,
                Type = vehicle.Type,
                ArrivalTime = vehicle.ArrivalTime,
                Brand = vehicle.Brand,
                Color = vehicle.Color,
                Model = vehicle.Model,
                RegistrationNumber = vehicle.RegistrationNumber,
                WheelCount = vehicle.WheelCount,
            };
        }

    }
}
