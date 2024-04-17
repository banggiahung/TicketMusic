namespace TicketMusic.Models.ProductsViewModel
{
    public class ProductsCRUDViewModel
    {
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

        public IFormFile? PrPath { get; set; }
        public string Variants { get; set; }
        public static implicit operator ProductsCRUDViewModel(Products _Products)
        {
            return new ProductsCRUDViewModel
            {
                IDProduct = _Products.IDProduct,
                Name = _Products.Name,
                Slug = _Products.Slug,
                CategoryID = _Products.CategoryID,
                ShortDescription = _Products.ShortDescription,
                Description = _Products.Description,
                TimeEvent = _Products.TimeEvent,
                CreateDate = _Products.CreateDate,
                Location = _Products.Location,
                ViewCount = _Products.ViewCount,
                ImageEvent = _Products.ImageEvent,
                UpdateDate = _Products.UpdateDate,
                IsPopular = _Products.IsPopular,
              

            };
        }

        public static implicit operator Products(ProductsCRUDViewModel vm)
        {
            return new Products
            {
                IDProduct = vm.IDProduct,
                Name = vm.Name,
                Slug = vm.Slug,
                CategoryID = vm.CategoryID,
                ShortDescription = vm.ShortDescription,
                Description = vm.Description,
                TimeEvent = vm.TimeEvent,
                CreateDate = vm.CreateDate,
                Location = vm.Location,
                ViewCount = vm.ViewCount,
                ImageEvent = vm.ImageEvent,
                UpdateDate = vm.UpdateDate,
                IsPopular = vm.IsPopular,

            };
        }
    }
}
