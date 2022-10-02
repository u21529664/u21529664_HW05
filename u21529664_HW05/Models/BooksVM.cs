using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u21529664_HW05.Models
{
    public class BooksVM
    {
        public List<Books> Books { get; set; }

        public List<Authors> Authors { get; set; }
         public List<Types> Types { get; set; }
    }
}