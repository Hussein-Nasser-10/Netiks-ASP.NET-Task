using Microsoft.AspNetCore.Mvc;
using Exercise1.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Exercise2.Models;
using System.Text;
using static Exercise1.Controllers.Models;

namespace Exercise2.Controllers
{
    public class CoursesController : Controller
    {
        private readonly HttpClient _httpClient;

        public CoursesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7135/api/");
        }

        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("courses/GetAllCourses");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<IEnumerable<Exercise1.Models.Course>>(data);
                return View("Courses",courses);
            }
            else
            {
                // Handle error
                return Content("Error: Unable to retrieve courses.");
            }
        }

        public async Task<IActionResult> Details(string code)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"courses/{code}/GetCourseDetails");
            //HttpResponseMessage response1 = await _httpClient.GetAsync($"courses/{code}/students");


            if (response.IsSuccessStatusCode)
            {

                string data = await response.Content.ReadAsStringAsync();
                var course = JsonConvert.DeserializeObject<CourseDetailsViewModel>(data);
                //string data1 = await response1.Content.ReadAsStringAsync();
                //var students = JsonConvert.DeserializeObject<IEnumerable<Student>>(data1);
                //var viewModel = new CoursesDetailsViewModel
                //{
                //    Course = course,
                //    students = students
                //};
                //var viewModel = new CourseDetailsViewModel
                //{
                //    Code = course.Code,
                //};
                return View("CourseDetails", course);
            }
            else
            {
                // Handle error
                return Content($"Error: Unable to retrieve details for course with code {code}.");
            }
        }

        public IActionResult GoToAdd()
        {

            return View("AddCourses");

        }

        //public async Task<IActionResult> Delete(string id)
        //{

        //    HttpResponseMessage response = await _httpClient.DeleteAsync($"courses/DeleteCourse/{id}");
        //    HttpResponseMessage response1 = await _httpClient.GetAsync("courses/GetAllCourses");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string data = await response1.Content.ReadAsStringAsync();
        //        var courses = JsonConvert.DeserializeObject<IEnumerable<Exercise1.Models.Course>>(data);
        //        return View("Courses", courses); // Return a success message if the student is unregistered successfully
        //    }
        //    else
        //    {
        //        // Handle error
        //        string errorMessage = $"Error: Unable to delete course. Status Code: {response.StatusCode}";
        //        return Content(errorMessage);
        //    }
        //}

        public async Task<IActionResult> Add(Course course)
        {
            string jsonData = JsonConvert.SerializeObject(course);
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage responseadd = await _httpClient.PostAsync("courses/AddCourse", content);
            HttpResponseMessage response = await _httpClient.GetAsync("courses/GetAllCourses");
            if (responseadd.IsSuccessStatusCode && response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<IEnumerable<Exercise1.Models.Course>>(data);
                return View("Courses", courses);
            }
            else
            {
                return Content("Error: canot add the same code course.");
            }

        }

        public async Task<IActionResult> Regestration(Course course)
        {
            HttpResponseMessage response = await _httpClient.GetAsync("courses/GetAllCourses");
            HttpResponseMessage response1 = await _httpClient.GetAsync("students/GetAllStudents");
            string data = await response.Content.ReadAsStringAsync();
            var courses = JsonConvert.DeserializeObject<IEnumerable<Exercise1.Models.Course>>(data);
            string data1 = await response1.Content.ReadAsStringAsync();
            var students = JsonConvert.DeserializeObject<IEnumerable<Exercise1.Models.Student>>(data1);
            var viewModel = new RegistrationViewModel
            {
                Courses = courses,
                Students = students
            };

            return View("~/Views/Register/Register.cshtml", viewModel);

        }

        [HttpPost]
        public async Task<IActionResult> Register(string courseId, int studentId)
        {

            var model = new RegistrationInputModel
            {
                StudentId = studentId,
                CourseCode = courseId
            };
            string jsonModel = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("registrations/register", content);


            if (response.IsSuccessStatusCode)
            {
                HttpResponseMessage response2 = await _httpClient.GetAsync("courses/GetAllCourses");
                HttpResponseMessage response1 = await _httpClient.GetAsync("students/GetAllStudents");
                string data = await response2.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<IEnumerable<Exercise1.Models.Course>>(data);
                string data1 = await response1.Content.ReadAsStringAsync();
                var students = JsonConvert.DeserializeObject<IEnumerable<Exercise1.Models.Student>>(data1);
                var viewModel = new RegistrationViewModel
                {
                    Courses = courses,
                    Students = students
                };

                return View("~/Views/Register/Register.cshtml", viewModel);
            }
            else
            {
                return Content("Error: u cant add this resigtration because its added befoe.");

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
                return RedirectToAction("Details", new { code = courseCode });
            }
            else
            {
                return Content("cannot unrigester");
            }
        }

    }
}
