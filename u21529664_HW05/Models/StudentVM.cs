using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u21529664_HW05.Models
{
    public class StudentVM
    {
        public List<Student> Students { get; set; }
        public Books Books { get; set; }
        public List<Class1> Class { get; set; }
    }
}