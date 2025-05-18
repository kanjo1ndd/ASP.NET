using ASP_SPR311.Data;
using ASP_SPR311.Data.Entities;
using ASP_SPR311.Models.Home;
using ASP_SPR311.Models.User;
using ASP_SPR311.Services.Kdf;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Text.Json;

namespace ASP_SPR311.Controllers
{
    public class UserController(DataContext dataContext, DataAccessor dataAccessor,IkdfService kdfService) : Controller
    {
        private static String signupFormKey = "UserSignupFormModel";
        private readonly DataContext _dataContext = dataContext;
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private readonly IkdfService _kdfService = kdfService;
        public ActionResult Index()
        {
            return View();
        }
        public IActionResult Signup()
        {
            UserSignupViewModel viewModel = new();
            if (HttpContext.Session.Keys.Contains(signupFormKey))
            {
                viewModel.FormModel =
                    JsonSerializer.Deserialize<UserSignupFormModel>(
                        HttpContext.Session.GetString(signupFormKey)!
                    );

                viewModel.ValidationErrors = ValidateSignUpFormModel(viewModel.FormModel!);

                if(viewModel.ValidationErrors.Count == 0)
                {
                    Guid userId = Guid.NewGuid();
                    _dataContext.UserData.Add(new()
                    {
                        Id = userId,
                        Name = viewModel.FormModel!.UserName,
                        Email = viewModel.FormModel!.UserEmail,
                        Phone = viewModel.FormModel!.UserPhone,
                    });
                    String salt = Guid.NewGuid().ToString()[..16];
                    _dataContext.UserAccesses.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Login = viewModel.FormModel!.UserLogin,
                        RoleId = "guest",
                        Salt = salt,
                        Dk = _kdfService.DerivedKey(
                            viewModel.FormModel!.UserPassword,
                            salt),
                    });
                    _dataContext.SaveChanges();
                }

                HttpContext.Session.Remove(signupFormKey);
            }
            return View(viewModel);
        }
        public IActionResult Signin()
        {
            AccessToken accessToken;
            try
            {
                accessToken = _dataAccessor.Authenticate(Request);
            }
            catch (Win32Exception ex)
            {
                return Json(new { status = ex.ErrorCode, message = ex.Message });
            }

            HttpContext.Session.SetString("userAccessId", accessToken.Sub.ToString());

            return Json(new { status = 200, message = "OK" });
        }
        public RedirectToActionResult Register([FromForm] UserSignupFormModel formModel)
        {
            HttpContext.Session.SetString(
                signupFormKey,
                JsonSerializer.Serialize(formModel)
            );
            return RedirectToAction(nameof(Signup));
        }

        private Dictionary<String, String> ValidateSignUpFormModel(UserSignupFormModel formModel) 
        {
            Dictionary<String, String> errors = [];
            if (formModel == null)
            {
                errors["Model"] = "Дані не передані";
            }
            else
            {
                if (String.IsNullOrEmpty(formModel.UserName))
                {
                    errors[nameof(formModel.UserName)] = "Ім'я необхідно ввести";
                }
                if (String.IsNullOrEmpty(formModel.UserEmail))
                {
                    errors[nameof(formModel.UserEmail)] = "E-mail необхідно ввести";
                }
                if (String.IsNullOrEmpty(formModel.UserPhone))
                {
                    errors[nameof(formModel.UserPhone)] = "Телефон необхідно ввести";
                }
                if (String.IsNullOrEmpty(formModel.UserLogin))
                {
                    errors[nameof(formModel.UserLogin)] = "Логін необхідно ввести";
                }
                else
                {
                    if(_dataContext.UserAccesses.FirstOrDefault(ua => ua.Login == formModel.UserLogin) != null)
                    {
                        errors[nameof(formModel.UserLogin)] = "Логін у вжитку. Виберіть інший";
                    }
                }
                if (String.IsNullOrEmpty(formModel.UserPassword))
                {
                    errors[nameof(formModel.UserPassword)] = "Пароль необхідно ввести";
                }
                if (formModel.UserPassword != formModel.UserRepeat)
                {
                    errors[nameof(formModel.UserRepeat)] = "Паролі не однакові";
                }

                ////////////////////////////////////////////////////////////////////
                if (formModel.BirthDate == null)
                {
                    errors[nameof(formModel.BirthDate)] = "Дата народження необхідна";
                }
                else if (formModel.BirthDate > DateTime.Now.AddYears(-5)) 
                {
                    errors[nameof(formModel.BirthDate)] = "Дата народження не може бути пізнішою за 5 років тому";
                }
                if (formModel.ClothingSize < 30 || formModel.ClothingSize > 60)
                {
                    errors[nameof(formModel.ClothingSize)] = "Розмір одягу повинен бути в межах 30-60";
                }
                if (formModel.ShoeSize < 20 || formModel.ShoeSize > 50)
                {
                    errors[nameof(formModel.ShoeSize)] = "Розмір взуття повинен бути в межах 20-50";
                }
                if (formModel.RingSize < 10 || formModel.RingSize > 30)
                {
                    errors[nameof(formModel.RingSize)] = "Розмір кільця повинен бути в межах 10-30";
                }
                if (!String.IsNullOrEmpty(formModel.SocialNetworkUrl))
                {
                    if (!Uri.TryCreate(formModel.SocialNetworkUrl, UriKind.Absolute, out _))
                    {
                        errors[nameof(formModel.SocialNetworkUrl)] = "Некоректне посилання";
                    }
                }
            }

            return errors;
        }
    }
}
