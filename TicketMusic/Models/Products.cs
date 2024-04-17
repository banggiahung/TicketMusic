
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace TicketMusic.Models
{
    public class Products
    {
        [Key]
        public int IDProduct { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int CategoryID { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public DateTime TimeEvent { get; set; }
        public string? Location { get; set; }
        public int ViewCount { get; set; }

        public string? ImageEvent { get; set; }
        public bool IsPopular { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Categories Categories { get; set; }

        public virtual ICollection<ProductVariants> ProductVariants { get; set; }
    }
}
