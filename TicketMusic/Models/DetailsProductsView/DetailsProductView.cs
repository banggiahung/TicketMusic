namespace TicketMusic.Models.DetailsProductsView
{
    public class DetailsProductView
    {
        public Products Products { get; set; }
        public List<Products> ProductsList { get; set; }

        public ApplicationUser User { get; set; }
    }
}
