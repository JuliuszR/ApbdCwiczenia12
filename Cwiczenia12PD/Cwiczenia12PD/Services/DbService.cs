using Cwiczenia12PD.Data;
using Cwiczenia12PD.Models;
using Cwiczenia12PD.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Cwiczenia12PD.Services;

public class DbService : IDbService
{
    private readonly ApbdContext _context;

    public DbService(ApbdContext context)
    {
        _context = context;
    }

    public async Task<object> GetTripsAsync(int page, int pageSize)
    {
        var totalCount = await _context.Trips.CountAsync();

        var trips = await _context.Trips
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new
            {
                t.Name,
                t.Description,
                t.DateFrom,
                t.DateTo,
                t.MaxPeople,
                Countries = t.IdCountries.Select(c => new { c.Name }),
                Clients = t.ClientTrips.Select(ct => new
                {
                    ct.IdClientNavigation.FirstName,
                    ct.IdClientNavigation.LastName
                })
            })
            .ToListAsync();

        var allPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return new
        {
            pageNum = page,
            pageSize = pageSize,
            allPages = allPages,
            trips = trips
        };
    }

    public async Task DeleteClientAsync(int idClient)
    {
        var client = await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
            throw new KeyNotFoundException($"Client with id {idClient} not found.");

        if (client.ClientTrips.Any())
            throw new InvalidOperationException("Cannot delete client who is assigned to one or more trips.");

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
    }

    public async Task<AssignClientToTripDtoResponse> AssignClientToTripAsync(int idTrip, AssignClientToTripDto dto)
    {
        var trip = await _context.Trips.FindAsync(idTrip);
        if (trip == null)
            throw new KeyNotFoundException($"Trip with id {idTrip} not found.");

        if (trip.DateFrom <= DateTime.Now)
            throw new InvalidOperationException("Cannot sign up for a trip that has already started.");

        if (await _context.Clients.AnyAsync(c => c.Pesel == dto.Pesel))
            throw new InvalidOperationException("Client with this PESEL already exists.");

        bool already = await _context.ClientTrips
            .Include(ct => ct.IdClientNavigation)
            .AnyAsync(ct => ct.IdTrip == idTrip && ct.IdClientNavigation.Pesel == dto.Pesel);
        if (already)
            throw new InvalidOperationException("Client with this PESEL is already assigned to this trip.");

        var client = new Client
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Telephone = dto.Telephone,
            Pesel = dto.Pesel
        };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var ct = new ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = dto.PaymentDate
        };
        _context.ClientTrips.Add(ct);
        await _context.SaveChangesAsync();

        return new AssignClientToTripDtoResponse()
        {
            ClientId = client.IdClient,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Pesel = client.Pesel,
            RegisteredAt = ct.RegisteredAt,
            PaymentDate = ct.PaymentDate
        };
    }
}