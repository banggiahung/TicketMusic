using System.ComponentModel.DataAnnotations.Schema;

namespace TicketMusic.Models;

public class PaymentInformationModel
{
    public int ID { get; set; }
    public string OrderType { get; set; }
    public double Amount { get; set; }
    public string OrderDescription { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public int PackegeID { get; set; }

   
}