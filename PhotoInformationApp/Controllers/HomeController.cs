using Microsoft.AspNetCore.Mvc;
using PhotoInformationApp.Models;
using System.Diagnostics;
using System.Text.Json;

namespace PhotoInformationApp.Controllers
{
    public class HomeModel
    {
        public string? AlbumId { get; set; }
        public string? PhotoId { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private List<PhotoModel> _photos = new List<PhotoModel>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            await GetPhotos(null, null);
            ViewData["photos"] = _photos;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RefreshResults(HomeModel model)
        {
            await GetPhotos(model.AlbumId, model.PhotoId);
            ViewData["photos"] = _photos;
            return View("Index");
        }

        public async Task GetPhotos(string? albumId, string? photoId)
        {
            string queryRoot = "https://jsonplaceholder.typicode.com/photos?";

            try
            {
                using var client = new HttpClient();
                string url = queryRoot;
                if (albumId != null)
                    url += $"albumId={albumId}&";

                if (photoId != null)
                    url += $"id={photoId}";

                _logger.Log(LogLevel.Information, $"String: {url}");
                var endpoint = new Uri(url);
                var response = await client.GetAsync(endpoint);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _photos.Clear();
                    var models = JsonSerializer.Deserialize<List<PhotoModel>>(result);
                    if (models != null)
                        _photos = models;
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Exception Caught!");
                _logger.LogError($"Message: {e.Message}");
            }
        }
         
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}