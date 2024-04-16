using Microsoft.AspNetCore.Mvc;
using TicketMusic.Services;

namespace Tommava.ViewComponents
{
    [ViewComponent(Name = "FooterMain")]
    public class FooterComponent : ViewComponent
    {
        private ICommon _icommom;

        public FooterComponent(ICommon icommom)
        {
            _icommom = icommom;
        }
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
