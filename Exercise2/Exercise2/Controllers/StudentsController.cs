using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using Exercise1.Database;
using Exercise1.Models;

namespace Exercise2.Controllers
{
    public class StudentsController : Controller
    {
        private readonly HttpClient _httpClient;
      

        public StudentsController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7135/api/");
        }

        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("students/GetAllStudents");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var students = JsonConvert.DeserializeObject<IEnumerable<Exercise1.Models.Student>>(data);
                return View("Students",students);
            }
            else
            {
                // Handle error
                return Content("Error: Unable to retrieve students.");
            }
        }


        public async Task<IActionResult> Details(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"students/{id}");
            HttpResponseMessage response1 = await _httpClient.GetAsync($"students/{id}/courses");


            if (response.IsSuccessStatusCode && response1.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var student = JsonConvert.DeserializeObject<Exercise1.Models.Student>(data);
                string data1 = await response1.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<IEnumerable<Course>>(data1);
                var viewModel = new StudentsDetailsViewModel
                {
                    Courses = courses,
                    student = student
                };
                return View("StudentDetails", viewModel);
            }
            else
            {
                // Handle error
                return Content($"Error: Unable to retrieve details for student with ID {id}.");
            }
        }

        public async Task<IActionResult> Unregister(int id, string courseCode)
        {
            var viewModel = new UnregisterModel
            {
                CourseCode = courseCode,
                StudentId = id
            };
            string jsonData = JsonConvert.SerializeObject(viewModel);
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("students/unregister", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id });
            }
            else
            {
                return Content("canot unrigester");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {

            HttpResponseMessage response = await _httpClient.DeleteAsync($"students/DeleteStudent/{id}");
            HttpResponseMessage response1 = await _httpClient.GetAsync("students/GetAllStudents");
            if (response.IsSuccessStatusCode)
            {
                string data = await response1.Content.ReadAsStringAsync();
                var students = JsonConvert.DeserializeObject<IEnumerable<Exercise1.Models.Student>>(data);
                return View("Students", students);
            }
            else
            {
                string errorMessage = $"Error:Unregister student from the course before deleting the student please. Status Code: {response.StatusCode}";
                return Content(errorMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(Student student)
        {
            string jsonData = JsonConvert.SerializeObject(student);

            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage responseadd = await _httpClient.PostAsync("students/AddStudent", content);
            HttpResponseMessage response = await _httpClient.GetAsync("students/GetAllStudents");
            if (responseadd.IsSuccessStatusCode && response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var students = JsonConvert.DeserializeObject<IEnumerable<Exercise1.Models.Student>>(data);
                return View("Students", students);
            }
            else
            {
                return Content("Error: Unable to retrieve students.");
            }

        }

        public IActionResult GoToAdd()
        {

            return View("RegisterStudents");

        }

    }
}
