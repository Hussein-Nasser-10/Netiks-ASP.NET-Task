using Exercise1.Database;
using Exercise1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exercise1.Controllers
{

    [ApiController]
        [Route("api/[controller]")]
        public class CoursesController : Controller
        {
            private readonly DbUniversityContext _context; 

            public CoursesController(DbUniversityContext context)
            {
                _context = context;
            }

        // GET: api/courses
        //b.List all Courses
        [HttpGet]
        [Route("GetAllCourses")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            if (courses == null || courses.Count == 0)
            {
                return NotFound("No courses found.");
            }
            return Ok(courses);
        }

        // PUT: api/courses/{code}
        //g. Update a course
        [HttpPut("{code}")]
        public async Task<IActionResult> UpdateCourse(string code, [FromBody] CourseInputModel model)
        {
            var course = await _context.Courses.FindAsync(code);

            if (course == null)
            {
                return NotFound("Course not found.");
            }

            // Update course properties with values from the request body
            course.Name = model.Name;
            course.Credits = model.Credits;

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(code))
                {
                    return NotFound("Course not found.");
                }
                else
                {
                    throw;
                }
            }

            return Ok("Course updated successfully.");
        }

        private bool CourseExists(string code)
        {
            return _context.Courses.Any(e => e.Code == code);
        }

        // GET: api/courses/{code}/students
        //h. List a course's registered students
        [HttpGet("{code}/students")]
        public async Task<ActionResult<IEnumerable<Student>>> GetCourseStudents(string code)
        {
            var course = await _context.Courses.Include(c => c.StudentCourses).ThenInclude(sc => sc.Student).FirstOrDefaultAsync(c => c.Code == code);

            if (course == null)
            {
                return NotFound("Course not found.");
            }

            var registeredStudents = course.StudentCourses.Select(sc => sc.Student).ToList();

            return Ok(registeredStudents);
        }


        //to get the courseDetails
        [HttpGet("{code}/GetCourseDetails")]
        public async Task<ActionResult<Course>> GetCourse(string code)
        {
            var course = await _context.Courses
                .Include(x => x.StudentCourses)
                .Where(x => x.Code.Equals(code))
                .Select(x => new
                {
                    Code = x.Code,
                    Name = x.Name,
                    Credits = x.Credits,
                    RegisteredStudents = x.StudentCourses.Select(x => x.Student).ToList()
                })
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound("Course not found.");
            }

            return Ok(course);
        }

        [HttpPost]
        [Route("addCourse")]
        public async Task<ActionResult> addCourse(Course course)
        {
            try
            {
                var entityEntry = await _context.Courses.AddAsync(course);
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
        [Route("DeleteCourse/{id}")]
        public async Task<ActionResult> DeleteCourse(string id)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);

                if (course == null)
                {
                    return NotFound(); // Return a 404 Not Found if the student with the given id is not found.
                }

                _context.Courses.Remove(course);
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
    public class CourseInputModel
    {
        public string Name { get; set; }
        public int Credits { get; set; }
    }
}

