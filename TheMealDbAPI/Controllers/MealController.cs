using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TheMealDbAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MealController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public MealController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<string> GetAsyncByName(string mealName)
        {
            var httpClient = _clientFactory.CreateClient("TheMealAPI");
            var response = await httpClient.GetStringAsync($"search.php?s={mealName}");
            return response;
        }

        [HttpGet]
        [Route("random")]
        public async Task<string> GetAsyncRandom()
        {
            var httpClient = _clientFactory.CreateClient("TheMealAPI");
            var response = await httpClient.GetStringAsync($"random.php");
            return response;
        }
    }
}
