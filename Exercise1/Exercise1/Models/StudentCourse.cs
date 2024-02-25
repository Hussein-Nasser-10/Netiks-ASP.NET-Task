using System;
using System.Collections.Generic;

namespace Exercise1.Models
{
    public partial class StudentCourse
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public int? StudentId { get; set; }

        public virtual Course? CodeNavigation { get; set; }
        public virtual Student? Student { get; set; }
    }

    public class RegistrationViewModel
    {
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Student> Students { get; set; }

    }

    public class CoursesDetailsViewModel
    {
        public Course Course { get; set; }
        public IEnumerable<Student> students { get; set; }

    }

    public class StudentsDetailsViewModel
    {
        public IEnumerable<Course> Courses { get; set; }
        public Student student { get; set; }

    }

    public class UnregisterModel
    {
        public int StudentId { get; set; }
        public string CourseCode { get; set; }
    }
}
