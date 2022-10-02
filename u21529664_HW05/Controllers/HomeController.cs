using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using u21529664_HW05.Models;

namespace u21529664_HW05.Controllers
{
    public class HomeController : Controller
    {
        private ServiceClass Services = new ServiceClass();
        public ActionResult Index()
        {
            BooksVM books = new BooksVM();
            books.Books = Services.GetBooks();
            books.Authors = Services.GetAuthors();
            books.Types = Services.GetBookTypes();
            return View(books);
        }

        public ActionResult BookDetails(int BookId)
        {
         
            BookDetailsVM bookDetails = new BookDetailsVM();
            bookDetails.BorrowedBooks = Services.GetBorrows(BookId);
            bookDetails.Books = Services.GetBooks().Where(bd => bd.BookID== BookId).FirstOrDefault();
            return View(bookDetails);
        }
       

        public ActionResult Students(int BookId)
        {
            
            StudentVM studentDetails = new StudentVM();
            List<Student> students = Services.GetStudents();
            List<Borrows> books = Services.GetBorrows(BookId);
            foreach (var student in students)
            {
                for (int k = 0; k < books.Count(); k++)
                {
                    string name = student.Name + " " + student.Surname;
                    if (books[k].StudentName == name && (books[k].BroughtDate == "" || books[k].BroughtDate == null))
                    {
                        student.Book = true;
                    }
                    else
                    {
                        student.Book = false;

                    }
                }
            }
            studentDetails.Students = students;
            studentDetails.Books = Services.GetBooks().Where(sd => sd.BookID == BookId).FirstOrDefault();
            studentDetails.Class = Services.GetStudentClasses();
            return View(studentDetails);
        }
        public ActionResult BorrowBook(int BookId, int StudentId)
        {
            Services.BorrowBook(BookId, StudentId);
            BookDetailsVM bookDetails = new BookDetailsVM();
            bookDetails.BorrowedBooks = Services.GetBorrows(BookId);
            bookDetails.Books = Services.GetBooks().Where(bd => bd.BookID == BookId).FirstOrDefault();
            return View("BookDetails", bookDetails);
        }

        public ActionResult ReturnBook(int BookId, int StudentId)
        {
            Services.ReturnBook(BookId, StudentId);

            BookDetailsVM bookDetails = new BookDetailsVM();
            bookDetails.BorrowedBooks = Services.GetBorrows(BookId);
            bookDetails.Books = Services.GetBooks().Where(bd => bd.BookID == BookId).FirstOrDefault();
            return View("BookDetails", bookDetails);

        }
        public ActionResult SearchBook(int BookType = 0, int BookAuthor = 0, string BookName = null)
        {

            BooksVM booksDetails = new BooksVM();
            booksDetails.Books = Services.BookSearch(BookName, BookType, BookAuthor);
            booksDetails.Authors = Services.GetAuthors();
            booksDetails.Types = Services.GetBookTypes();
            return View("Index", booksDetails);
        }

        public ActionResult SearchStudent(int BookId, string sname = null, string sclass = null)
        {
            
            StudentVM studentDetails = new StudentVM
            {
                Students = Services.StudentSearchInfo(sname, sclass),
                Books = Services.GetBooks().Where(myBook => myBook.BookID==BookId).FirstOrDefault(),
                Class = Services.GetStudentClasses()

            };
            return View("Students", studentDetails);
        }
    }
}