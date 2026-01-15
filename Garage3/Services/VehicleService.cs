using Garage3.Data;
using Garage3.Models;
using Garage3.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Garage3.Services
{
	public class VehicleService
	{
		private readonly ApplicationDbContext _context;
		private readonly ReceiptService _receiptService;

		public VehicleService(ApplicationDbContext context, ReceiptService receiptService)
		{
			_context = context;
			_receiptService = receiptService;
		}

		public async Task AddAsync(Vehicle vehicle)
		{
			_context.Vehicles.Add(vehicle);

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
			{
				throw new DuplicateRegistrationNumberException(vehicle.RegistrationNumber);
			}
		}

		public async Task UpdateAsync(Vehicle vehicle)
		{

			try
			{
				_context.Update(vehicle);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
			{
				throw new DuplicateRegistrationNumberException(vehicle.RegistrationNumber);
			}
		}

		public async Task DeleteVehicle(int id)
		{
			var vehicle = await _context.Vehicles.FindAsync(id);
			if (vehicle != null)
			{
				_context.Vehicles.Remove(vehicle);
			}

			await _context.SaveChangesAsync();
		}

		public async Task<Receipt?> DeleteVehicleAndCreateReceipt(int id)
		{
			var vehicle = await _context.Vehicles.FindAsync(id);

			if (vehicle == null)
			{
				return null;
			}
				_context.Vehicles.Remove(vehicle);
				Receipt receipt = _receiptService.CreateReceipt(vehicle, checkoutTime: DateTime.Now);

			await _context.SaveChangesAsync();
			return receipt;
		}

		private static bool IsUniqueConstraintViolation(DbUpdateException ex)
		{
			// SQL Server
			if (ex.InnerException is SqlException sqlEx)
			{
				return sqlEx.Number == 2601 || sqlEx.Number == 2627;
			}

			// SQLite / PostgreSQL / etc → fallback
			return ex.InnerException?.Message.Contains("UNIQUE") == true;
		}

		public IEnumerable<VehicleViewModel> Select(Func<Vehicle, VehicleViewModel> p)
		{
			var vehicle = _context.Vehicles;
			return vehicle.Select(p);
		}

		public bool VehicleExists(int id)
			=> _context.Vehicles.Any(e => e.Id == id);

		public async Task<List<Vehicle>> GetAllAsync()
			=> await _context.Vehicles.ToListAsync();

		public async Task<Vehicle?> GetByIdAsync(int? id)
			=> await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == id);

		public async Task<Vehicle?> GetByRegistrationNumberAsync(string regNr)
			=> await _context.Vehicles.FirstOrDefaultAsync(v => v.RegistrationNumber == regNr);

		public void Update(Vehicle vehicle)
			=> _context.Update(vehicle);

		public IQueryable<Vehicle> GetAll()
		{
			return _context.Vehicles.AsQueryable();
		}
	}
}
