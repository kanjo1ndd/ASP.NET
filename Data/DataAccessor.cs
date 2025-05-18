using ASP_SPR311.Data.Entities;
using ASP_SPR311.Services.Kdf;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace ASP_SPR311.Data
{
    public class DataAccessor(DataContext dataContext, IkdfService kdfService, IHttpContextAccessor httpContextAccessor)
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IkdfService _kdfService = kdfService;
        private String ImagePath => $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}/Shop/Image/";
        public List<Category> AllCategories()
        {

            var categories = _dataContext
                .Categories
                .Where(c => c.DeletedAt == null)
                .AsNoTracking()
                .ToList();

            foreach (var category in categories) 
            {
                category.ImageUrl = ImagePath + category.ImageUrl;
            }

            return categories;

        }
        public Category? GetCategory(String slug)
        {
            var category = _dataContext 
                .Categories
                .Include(c => c.Products)
                .AsNoTracking()
                .FirstOrDefault(c => c.Slug == slug)
                ;

            if (category != null)
            {
                category.ImageUrl = ImagePath + category.ImageUrl;
                foreach (var product in category!.Products)
                {
                    product.ImagesCsv = String.Join(',',
                        product.ImagesCsv.Split(',').Select(i => ImagePath + i)
                    );
                }
            }

            return category;
        }

        public AccessToken Authenticate(HttpRequest Request)
        {
            String authHeader = Request.Headers.Authorization.ToString();
            if (String.IsNullOrEmpty(authHeader))
            {
                throw new Win32Exception(401, "Autorization header required");
            }
            String scheme = "Basic ";
            if (!authHeader.StartsWith(scheme))
            {
                throw new Win32Exception(401, $"Autorization scheme must be {scheme}");
            }
            String credentials = authHeader[scheme.Length..];
            String authData;
            try
            {
                authData = System.Text.Encoding.UTF8.GetString(
                    Base64UrlTextEncoder.Decode(credentials)
                );
            }
            catch
            {
                throw new Win32Exception(401, $"Not valid Base64 code '{credentials}'");
            }
            String[] parts = authData.Split(':', 2);
            if (parts.Length != 2)
            {
                throw new Win32Exception(401, $"Not valid credentials format (missing ':'?)");
            }
            String login = parts[0];
            String password = parts[1];
            var userAccess = _dataContext
                .UserAccesses
                .Include(ua => ua.UserData)
                .FirstOrDefault(ua => ua.Login == login);
            if (userAccess == null)
            {
                throw new Win32Exception(401, $"Credentials rejected");
            }
            if (_kdfService.DerivedKey(password, userAccess.Salt) != userAccess.Dk)
            {
                throw new Win32Exception(401, $"Credentials rejected");
            }

            AccessToken accessToken = new()
            {
                Jti = Guid.NewGuid(),
                Sub = userAccess.Id,
                Aud = userAccess.UserId,
                Iat = DateTime.Now,
                Nbf = null,
                Exp = DateTime.Now.AddMinutes(10),
                Iss = "ASP-SPR311",
                User = userAccess.UserData,
            };

            var existingToken = _dataContext.AccessTokens
                    .FirstOrDefault(t => t.Sub == userAccess.Id && t.Exp > DateTime.Now);

            if (existingToken != null)
            {
                existingToken.Exp = DateTime.Now.AddMinutes(10);
                _dataContext.SaveChanges();
                return existingToken;
            }
            else {
                _dataContext.AccessTokens.Add(accessToken);
                _dataContext.SaveChanges();
                return accessToken;
            }
        }

        public AccessToken Authorize(HttpRequest Request)
        {
            string authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                throw new Win32Exception(401, "Authorization header required");
            }

            string scheme = "Bearer ";
            if (!authHeader.StartsWith(scheme))
            {
                throw new Win32Exception(401, $"Authorization scheme must be {scheme}");
            }
            string credentials = authHeader[scheme.Length..];
            Guid jti;
            try
            {
                jti = Guid.Parse(credentials);
            }
            catch
            {
                throw new Win32Exception(401, "Authorization credential invalid formatted");
            }

            AccessToken? accessToken = _dataContext.AccessTokens.Include(at => at.User)
                .FirstOrDefault(at => at.Jti == jti);

            if (accessToken == null)
            {
                throw new Win32Exception(401, "Bearer credentials rejected");
            }

            if (accessToken.Exp < DateTime.Now)
            {
                throw new Win32Exception(401, "Bearer credentials expired");
            }

            return accessToken;
        }
    }
}
