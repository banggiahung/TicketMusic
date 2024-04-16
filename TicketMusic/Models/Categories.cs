using System.ComponentModel.DataAnnotations;

namespace TicketMusic.Models
{
    public class Categories
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Slug { get; set; }
        public bool IsDefault { get; set; }

    }
}
