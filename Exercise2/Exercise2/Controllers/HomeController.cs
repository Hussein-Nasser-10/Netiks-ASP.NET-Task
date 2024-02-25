using Exercise1.Models;
using Exercise2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;

namespace Exercise2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        //private readonly HttpClient _httpClient;

        //public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        //{
        //    _logger = logger;
        //    _httpClient = httpClient;
        //    //_httpClient.BaseAddress = new Uri("https://localhost:7135/");
        //}

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

        }

        //private async Task<IEnumerable<Student>> GetStudentsFromApiAsync()
        //{
        //    var httpClient = _httpClient.CreateClient("Exercise1Api");

        //    var response = await httpClient.GetAsync("/api/students");
        //    response.EnsureSuccessStatusCode();

        //    var students = await response.Content.ReadAsAsync<IEnumerable<Student>>();
        //    return students;
        //}

        //public async Task<IActionResult> Index()
        //{
        //    var students = await GetStudentsFromApiAsync();
        //    return View(students);
        //}

        //public async Task<IActionResult> Index()
        //{
        //    HttpResponseMessage response = await _httpClient.GetAsync("example");

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Deserialize response and process data
        //        var responseData = await response.Content.ReadAsStringAsync();

        //        // Here you can deserialize the responseData if it's JSON or any other format
        //        // For example, if responseData is JSON, you can deserialize it like this:
        //        // var data = JsonConvert.DeserializeObject<MyModel>(responseData);

        //        // You can also directly return the responseData if you don't need to deserialize it
        //        return Ok(responseData);
        //    }
        //    else
        //    {
        //        // Handle error
        //        return StatusCode((int)response.StatusCode);
        //    }
        //}


        public IActionResult Index()
        {
            return View();
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