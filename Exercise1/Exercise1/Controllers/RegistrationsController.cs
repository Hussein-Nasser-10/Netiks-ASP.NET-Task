using Exercise1.Database;
using Exercise1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Exercise1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationsController : Controller
    {
        private readonly DbUniversityContext _context; 

        public RegistrationsController(DbUniversityContext context)
        {
            _context = context;
        }

        // POST: api/registrations/register
        [HttpPost("register")]
        public async Task<ActionResult<IEnumerable<StudentCourse>>> RegisterStudent([FromBody] Models.RegistrationInputModel model)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == model.StudentId);
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Code == model.CourseCode);

            if (student == null || course == null)
            {
                return NotFound();
            }

            var existingRegistration = await _context.StudentCourses.FirstOrDefaultAsync(sc => sc.StudentId == model.StudentId && sc.Code == model.CourseCode);

            if (existingRegistration != null)
            {
                return Conflict("Student is already registered for this course.");
            }

            var registration = new StudentCourse
            {
                StudentId = model.StudentId,
                Code = model.CourseCode
            };

            await _context.StudentCourses.AddAsync(registration);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                StudentId = registration.StudentId,
                Code = registration.Code
            });
        }

        // DELETE: api/registrations/unregister
        //e. Unregister a student from a course
        [HttpDelete("unregister")]
        public async Task<IActionResult> UnregisterStudent([FromBody] Models.RegistrationInputModel model)
        {
            var registration = await _context.StudentCourses.FirstOrDefaultAsync(sc => sc.StudentId == model.StudentId && sc.Code == model.CourseCode);

            if (registration == null)
            {
                return NotFound("Student is not registered for this course.");
            }
            
            _context.StudentCourses.Remove(registration);
            await _context.SaveChangesAsync();

            return Ok("Student unregistered successfully.");
        }

        // GET: api/registrations
        //c.list all registrations
        [HttpGet]
        [Route("ListAllRegistrations")]
        public async Task<ActionResult<IEnumerable<StudentCourse>>> GetRegistrations()
        {
            var registrations = await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.CodeNavigation)
                .Select(x => new
                {
                    StudentName = x.Student.Name,
                    CourseName = x.CodeNavigation.Name
                })
                .ToListAsync();

            if (registrations.Count == 0)
            {
                return NoContent(); // Returns 204 No Content status code
            }

            return Ok(registrations);
        }
    }

    public class Models
    {
        public class RegistrationInputModel
        {
            [Required]
            public int StudentId { get; set; }
            [Required]
            public string CourseCode { get; set; }
        }
    }
}
