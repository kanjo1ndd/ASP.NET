using ASP_SPR311.Data;
using ASP_SPR311.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_SPR311.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class ApiCategoryControler(DataAccessor dataAccessor) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        [HttpGet]
        public RestResponse Categories() 
        {
            return new()
            {
                Service = "API Products Categories",
                DataType = "array",
                CacheTime = 600,
                Data = _dataAccessor.AllCategories(),
            };
        }

        [HttpGet("{id}")]

        public RestResponse Category(String id)
        {
            return new()
            {
                Service = "API Products Categories",
                DataType = "object",
                CacheTime = 600,
                Data = _dataAccessor.GetCategory(id),
            };
        }
    }
}
