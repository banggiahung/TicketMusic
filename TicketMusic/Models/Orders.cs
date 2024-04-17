using System.ComponentModel.DataAnnotations;

namespace TicketMusic.Models
{
    public class Orders
    {
        [Key]
        public int OrderID { get; set; }
        public string OrderCode { get; set; }
        public string UserID { get; set; }
        public string Address { get; set; }
        public decimal TotalPrices { get; set; }
        public string PhoneNumber { get; set; }
        public string Customer_Name { get; set; }
        public bool OrderStatus { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual List<OrdersDetails> OrdersDetails { get; set; }
    }
}
