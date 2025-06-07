namespace Cwiczenia12PD.Models.DTOs;

public class AssignClientToTripDtoResponse
{
    public int ClientId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Pesel { get; set; } = null!;
    public DateTime RegisteredAt { get; set; }
    public DateTime? PaymentDate { get; set; }
}