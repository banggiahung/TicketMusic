using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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


        [ForeignKey("ProductsVariants")]
        public virtual ProductVariants ProductVariants { get; set; }

        [ForeignKey("ProductsID")]
        public virtual Products Products { get; set; }

        [ForeignKey("OrderID")]
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Orders Orders { get; set; }
    }
}
