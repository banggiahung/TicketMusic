using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TicketMusic.Models
{
    public class ProductVariants
    {
        [Key]
        public int ProductVariantsID { get; set; }
        public string VariantsValue { get; set; }
        public decimal PriceVariants { get; set; }
        public int ProductID { get; set; }
        public int QuantityTicket { get; set; }

        [ForeignKey("ProductID")]
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Products Products { get; set; }
    }
}
