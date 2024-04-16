using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [ForeignKey("ProductID")]
        [JsonIgnore]
        public virtual Products Products { get; set; }
    }
}
