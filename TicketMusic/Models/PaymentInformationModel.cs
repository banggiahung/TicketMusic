using System.ComponentModel.DataAnnotations.Schema;

namespace TicketMusic.Models;

public class PaymentInformationModel
{
    public decimal Amount { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string OrderType { get; set; }
    public string Address { get; set; }
    public string? OrderCode { get; set; }
    public string ListProducts { get; set; }

   
}