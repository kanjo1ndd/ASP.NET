using ASP_SPR311.Data;
using ASP_SPR311.Data.Entities;
using ASP_SPR311.Middleware;
using ASP_SPR311.Models;
using ASP_SPR311.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace ASP_SPR311.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class ApiUserControler(DataAccessor dataAccessor) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;

        [HttpGet]
        public RestResponse Authenticate()
        {
            var res = new RestResponse()
            {
                Service = "Api User Authentication",
                DataType = "object",
                CacheTime = 600,
            };
            try
            {
                res.Data = _dataAccessor.Authenticate(Request);
            }
            catch (Win32Exception e)
            {
                res.Status = new()
                {
                    IsOk = false,
                    Code = e.ErrorCode,
                    Phrase = e.Message
                };
                res.Data = null;
            }

            return res;
        }

        

        [HttpPost]
        public RestResponse SignUp(UserApiSignupFormModel model)
        {
            var res = new RestResponse
            {
                Service = "API User Registration",
                DataType = "object",
                CacheTime = 0,
                Data = model
            };

            return res;
        }

        [HttpGet("profile")]
        public RestResponse Profile()
        {
            var res = new RestResponse()
            {
                Service = "Api User Profile",
                DataType = "object",
                CacheTime = 600
            };
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                res.Data = (HttpContext.Items["AccessToken"] as AccessToken)?.User;
            }
            else
            {
                res.Status = new()
                {
                    IsOk = false,
                    Code = 401,
                    Phrase = HttpContext.Items[nameof(AuthTokenMiddleware)]?.ToString() ?? ""
                };
            }
            return res;
        }
    }
}
