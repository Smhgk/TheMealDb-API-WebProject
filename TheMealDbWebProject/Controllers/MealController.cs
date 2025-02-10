using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TheMealDbWebProject.Models;

namespace TheMealDbWebProject.Controllers
{
    public class MealController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public MealController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index(string mealName, bool random)
        {

            if (string.IsNullOrEmpty(mealName) && !random)
            {
                return View(null);
            }

            var mealResponse = new MealResponse();

            if (!random)
            {
                mealResponse = await GetAsyncByName(mealName);
            }
            else
            {
                mealResponse = await GetAsyncByRandom();
            }

            if (mealResponse?.Meals != null)
            {
                return View(mealResponse.Meals[0]);
            }

            return View(null);
        }
        private async Task<HttpClient> GetAuthenticatedClientAsync()
        {
            var httpClient = _clientFactory.CreateClient("LocalAPI");
            var res = await httpClient.PostAsJsonAsync("auth/authenticate", new LoginRequestModel { UserName = "admin", Password = "password" });
            res.EnsureSuccessStatusCode();
            var strJwt = await res.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<JwtToken>(strJwt);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? "");
            return httpClient;
        }

        public async Task<MealResponse> GetAsyncByName(string mealName)
        {
            var httpClient = await GetAuthenticatedClientAsync();
            return await httpClient.GetFromJsonAsync<MealResponse>($"Meal?mealName={mealName}") ?? new MealResponse();
        }

        public async Task<MealResponse> GetAsyncByRandom()
        {
            var httpClient = await GetAuthenticatedClientAsync();
            return await httpClient.GetFromJsonAsync<MealResponse>("meal/random") ?? new MealResponse();
        }


    }
}
