using System;
using System.Collections.Generic;

namespace Exercise1.Models
{
    public partial class Course
    {
        public Course()
        {
            StudentCourses = new HashSet<StudentCourse>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }

        public virtual ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
