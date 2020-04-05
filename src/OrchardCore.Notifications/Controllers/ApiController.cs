using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Notifications.Hubs;
using OrchardCore.Notifications.ViewModels;
using OrchardCore.Mvc.Utilities;

namespace OrchardCore.Notifications.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Api"), IgnoreAntiforgeryToken, AllowAnonymous]
    public class ApiController : Controller
    {
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAuthorizationService _authorizationService;

        public ApiController(IPushNotificationService pushNotificationService, IEventPublisher publisher, IAuthorizationService authorizationService)
        {
            _pushNotificationService = pushNotificationService;
            _eventPublisher = publisher;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        [Route("device")]
        public async Task<IActionResult> Register(DeviceRegistrationRequest registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (TryExtractPlatform(out MobilePlatform platform))
            {
                var registrationId = await _pushNotificationService.CreateRegistrationId(registration.Handle);
                await _pushNotificationService.Register(platform, registration.Handle, registrationId, new[] { registration.Tag });
                return Ok(new DeviceRegistrationResult { RegistrationId = registrationId });
            }

            return BadRequest("Unknown device platform or missing user-agent.");
        }

        [HttpDelete]
        [Route("device")]
        public async Task<IActionResult> Unregister(DeviceRegistrationResult registration)
        {
            await _pushNotificationService.Unregister(registration.RegistrationId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody]IDictionary<string, string> properties)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageNotificationSettings))
            {
                return this.ChallengeOrForbid();
            }

            ModelState.Clear();
            var viewmodel = NotificationViewModel.FromModel(properties);

            if (!TryValidateModel(viewmodel))
            {
                BadRequest();
            }

            await _eventPublisher.SendNotication(viewmodel.ToModel());
            return Ok();
        }

        private bool TryExtractPlatform(out MobilePlatform platform)
        {
            var regex = new Regex("(?<windows>windows)|(?<apple>ipad|iphone|mac os)|(?<android>android)", RegexOptions.IgnoreCase);
            var match = regex.Match(Request.Headers["User-Agent"].ToString()).Groups.Values
                .Where(m => m.Success && m.Name.Length > 1)
                .Select(m => m.Name)
                .FirstOrDefault();

            return Enum.TryParse(match, out platform);
        }
    }
}
