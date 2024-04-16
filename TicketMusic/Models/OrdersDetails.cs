using System.ComponentModel.DataAnnotations;

namespace TicketMusic.Models
{
    public class OrdersDetails
    {
        [Key]
        public int OrderDetailsID { get; set; }
        public int OrderID { get; set; }
        public int ProductsID { get; set; }
        public int ProductsVariants { get; set; }
        public int Quantity { get; set; }
        public decimal ProductsPrice { get; set; }

        public virtual ProductVariants ProductVariants { get; set; }
    }
}
