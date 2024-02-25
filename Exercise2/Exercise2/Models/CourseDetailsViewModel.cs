using Exercise1.Models;

namespace Exercise2.Models
{
    public class CourseDetailsViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int Credits { get; set; }
        public IEnumerable<Student> RegisteredStudents { get; set; }
    }
}
