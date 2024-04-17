namespace TicketMusic.Models.ViewHome
{
    public class ViewHomeIndex
    {
        public List<Products> ProductsPopular { get; set; }
        public List<Products> ProductsMost { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
