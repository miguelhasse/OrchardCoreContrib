using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Notifications.ViewModels;

namespace OrchardCore.Notifications.ViewComponents
{
    public class NotificationPropertiesViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(NotificationViewModel model)
        {
            return Task.FromResult<IViewComponentResult>(View(model));
        }
    }
}
