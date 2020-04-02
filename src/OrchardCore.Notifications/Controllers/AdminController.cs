using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Notifications;
using OrchardCore.Notifications.ViewModels;

namespace OrchardCore.Notifications.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly INotifier _notifier;
        private readonly IEventPublisher _publisher;
        private readonly IHtmlLocalizer H;

        public AdminController(
            IHtmlLocalizer<AdminController> h,
            IAuthorizationService authorizationService,
            INotifier notifier,
            IEventPublisher publisher,
            IStringLocalizer<AdminController> stringLocalizer)
        {
            _authorizationService = authorizationService;
            _notifier = notifier;
            _publisher = publisher;

            H = h;
            T = stringLocalizer;
        }

        private IStringLocalizer T { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageNotificationSettings))
            {
                return Forbid();
            }

            return View(NotificationViewModel.FromModel(new Notification()));
        }

        [HttpPost, ActionName(nameof(Index))]
        public async Task<IActionResult> Publish(NotificationViewModel model)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageNotificationSettings))
            {
                return Forbid();
            }

            ModelState.Clear();

            if (TryValidateModel(model))
            {
                try
                {
                    await _publisher.SendNotication(model.ToModel());
                    _notifier.Success(H["Message sent successfully"]);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("*", ex.Message);
                }
            }

            return View(model);
        }
    }
}
