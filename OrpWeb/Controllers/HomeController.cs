using Business.Communication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("PrivacyPolicy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("TermsOfServices")]
        public IActionResult TermsConditions()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// test notification send functionality
        /// </summary>
        /// <returns></returns>
        public IActionResult NotificationSend()
        {
            //Test Push Notification
            short deviceType = 1;
            //- TODO -- put mobile device token here
            string deviceToken = "";
            var message = PushNotification.SendPushNotification(deviceType, deviceToken, "Test Notification Fire", "Test Notification Type", "Orp");

            return View();
        }
    }
}