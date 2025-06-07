using Cwiczenia12PD.Models.DTOs;

namespace Cwiczenia12PD.Services;

public interface IDbService
{
    Task<object> GetTripsAsync(int page, int pageSize);
    Task DeleteClientAsync(int idClient);
    Task<AssignClientToTripDtoResponse> AssignClientToTripAsync(int idTrip, AssignClientToTripDto dto);

}