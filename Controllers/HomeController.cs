using ASP_SPR311.Models;
using ASP_SPR311.Models.Home;
using ASP_SPR311.Services.Timestamp;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace ASP_SPR311.Controllers
{
    public class HomeController : Controller
    {
		private readonly OtpService _otpService;

		private readonly ILogger<HomeController> _logger;

        private readonly ITimestampService _timestampService;
        public HomeController(ILogger<HomeController> logger, ITimestampService timestampService, OtpService otpService)
        {
            _logger = logger;
			_timestampService = timestampService;
			_otpService = otpService;
		}

        public IActionResult Index()
        {
			string otp = _otpService.GenerateOtp(6);
			return View("Index", otp);
		}
        public IActionResult Intro()
        {
            return View();
        }
		public IActionResult IoC()
		{
            ViewData["timestamp"] = _timestampService.Timestamp;
            ViewData["timestampCode"] = _timestampService.GetHashCode();
			return View();
		}
        public RedirectToActionResult Register(HomeModelsFormModel formModel)
        {
            HttpContext.Session.SetString(
                "HomeModelsFormModel",
                JsonSerializer.Serialize(formModel)
            );

            return RedirectToAction(nameof(Models));
        }
        public ViewResult Models()
        {
            HomeModelsViewModel viewModel = new();
            if (HttpContext.Session.Keys.Contains("HomeModelsFormModel"))
            {
                viewModel.FormModel =
                    JsonSerializer.Deserialize<HomeModelsFormModel>(
                        HttpContext.Session.GetString("HomeModelsFormModel")!
                    );
                HttpContext.Session.Remove("HomeModelsFormModel");
            }
            return View(new HomeModelsViewModel());
        }
        public IActionResult Razor()
		{
			return View();
		}
		public IActionResult Privacy()
        {
            return View();
        }

        public JsonResult Ajax(HomeModelsFormModel formModel)
        {
            return Json(formModel);
        }

        public JsonResult AjaxJson([FromBody] HomeModelsFormModel formModel)
        {
            return Json(formModel);
        }


        //-------------------------------------------
        //-------------------------------------------
        //public JsonResult SubmitReviewJson([FromBody] ReviewAjaxJsonModel model)
        //{
        //    return Json(model);
        //}
        //public JsonResult SubmitReviewForm([FromForm] ReviewAjaxFormModel model)
        //{
        //    return Json(model);
        //}
        //public RedirectToActionResult Register(ReviewAjaxFormModel model)
        //{
        //    HttpContext.Session.SetString(
        //        "ReviewAjaxFormModel",
        //        JsonSerializer.Serialize(model)
        //    );

        //    return RedirectToAction(nameof(Models));
        //}
        //public ViewResult Models()
        //{
        //    ReviewViewModel viewModel = new();

        //    if (HttpContext.Session.Keys.Contains("ReviewAjaxFormModel"))
        //    {
        //        viewModel.FormModel =
        //            JsonSerializer.Deserialize<ReviewAjaxFormModel>(
        //                HttpContext.Session.GetString("ReviewAjaxFormModel")!
        //            );
        //        HttpContext.Session.Remove("ReviewAjaxFormModel");
        //    }

        //    return View(viewModel);
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
