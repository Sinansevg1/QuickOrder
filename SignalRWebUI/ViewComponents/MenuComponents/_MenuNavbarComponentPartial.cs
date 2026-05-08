using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace SignalRWebUI.ViewComponents.MenuComponents
{
    public class _MenuNavbarComponentPartial:ViewComponent
    {
       public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
