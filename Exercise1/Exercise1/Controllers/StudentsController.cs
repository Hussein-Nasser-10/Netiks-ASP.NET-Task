using Exercise1.Database;
using Exercise1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exercise1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : Controller
    {
        private readonly DbUniversityContext _context;

        public StudentsController(DbUniversityContext context)
        {
            _context = context;
        }

        //a.list all students
        [HttpGet]
        [Route("GetAllStudents")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _context.Students.ToListAsync();
            if (students == null || students.Count == 0)
            {
                return NotFound("No students found.");
            }
            return Ok(students);
        }

        // GET: api/students/{studentId}/courses
        //d.List a Student’s registered courses.
        [HttpGet("{studentId}/courses")]
        public async Task<ActionResult<IEnumerable<Course>>> GetStudentCourses(int studentId)
        {
            // Find the student by ID
            var student = await _context.Students
                .Where(s => s.Id == studentId).FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound("Student not found");
            }

            // Extract course codes from StudentCourses
            var courses = await _context.StudentCourses.Where(x => x.StudentId == studentId)
                .Select(x => new
                {
                    Code = x.CodeNavigation.Code,
                    Name = x.CodeNavigation.Name
                }).ToListAsync();

            return Ok(courses);
        }

        // PUT: api/students/{id}
        //f. Update a student's information
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentInputModel model)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            // Update student properties with values from the request body
            student.Name = model.Name;
            student.DateOfBirth = model.DateOfBirth;
            student.PhoneNumber = model.PhoneNumber;
            student.Address = model.Address;

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound("Student not found.");
                }
                else
                {
                    throw;
                }
            }

            return Ok("Student updated successfully.");
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

        //to get the StudentDetails
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            return Ok(student);
        }


        [HttpPost("unregister")]
        public async Task<ActionResult> UnregisterStudent([FromBody] UnregisterModel model)
        {
            // Validate the model
            if (model == null || model.StudentId <= 0 || string.IsNullOrEmpty(model.CourseCode))
            {
                return BadRequest("Invalid request parameters.");
            }

            // Find the student course entry to unregister
            var studentCourse = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == model.StudentId && sc.Code == model.CourseCode);

            if (studentCourse == null)
            {
                return NotFound("Student is not registered for the specified course.");
            }

            // Remove the student course entry
            _context.StudentCourses.Remove(studentCourse);
            await _context.SaveChangesAsync();

            return Ok("Student unregistered successfully.");
        }

        [HttpPost]
        [Route("AddStudent")]
        public async Task<ActionResult> addStudent(Student student)
        {
            try
            {
                var entityEntry = await _context.Students.AddAsync(student);
                await _context.SaveChangesAsync();
                return Ok();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal server error" + ex);
            }
        }

        [HttpDelete]
        [Route("DeleteStudent/{id}")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);

                if (student == null)
                {
                    return NotFound(); // Return a 404 Not Found if the student with the given id is not found.
                }

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal server error" + ex);
            }
        }

    }

    public class StudentInputModel
    {
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
