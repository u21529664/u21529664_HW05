using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u21529664_HW05.Models
{
    public class Books
    {
        public int BookID { get; set; }
        public string Name { get; set; }
        public int PageCount { get; set; }
        public int Points { get; set; }
        public string AuthorName { get; set; }
        public string Status { get; set; }
        public string BookType { get; set; }


    }
}