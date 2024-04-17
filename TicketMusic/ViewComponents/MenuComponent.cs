using Microsoft.AspNetCore.Mvc;
using TicketMusic.Services;

namespace Tommava.ViewComponents
{
    [ViewComponent(Name = "HeaderMain")]
    public class MenuComponent : ViewComponent
    {
        private ICommon _icommom;

        public MenuComponent(ICommon icommom)
        {
            _icommom = icommom;
        }
        public IViewComponentResult Invoke()
        {
            var category = _icommom.GetCategories();
            return View(category);
        }
    }
}
