using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u21529664_HW05.Models
{
    public class BookDetailsVM
    {
        public Books Books { get; set; }
        public List<Borrows> BorrowedBooks { get; set; }
    }
}