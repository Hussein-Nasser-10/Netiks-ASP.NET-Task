using System;
using System.Collections.Generic;

namespace Exercise1.Models
{
    public partial class Student
    {
        public Student()
        {
            StudentCourses = new HashSet<StudentCourse>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        public virtual ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
