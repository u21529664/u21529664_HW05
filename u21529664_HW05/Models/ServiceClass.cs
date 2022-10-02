using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using u21529664_HW05.Models;
using System.Configuration;


namespace u21529664_HW05.Models
{
    public class ServiceClass
    {
       private String ConnectionString;

        public ServiceClass()
        {
            // Fetch connection string from Web config
            ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
        
        // Get Book details
        public List<Books> GetBooks()
        {
            List<Books> BookInfo = new List<Books>();
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                myConnection.Open();

                string BooksQuery="SELECT books.bookId as ID, books.name as Name,authors.surname as Author, types.name as Type,books.pagecount as PageCount, books.point as Points  FROM Books " +
                    " INNER JOIN authors on books.authorId = authors.authorId " +
                    " INNER JOIN types on books.typeId = types.typeId ";

                using (SqlCommand cmd = new SqlCommand(BooksQuery, myConnection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Books book = new Books
                            {
                               BookID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString(),
                               AuthorName= reader["Author"].ToString(),
                                PageCount = Convert.ToInt32(reader["PageCount"]),
                                Points = Convert.ToInt32(reader["Points"]),
                                BookType = reader["Type"].ToString(),
                            };
                            BookInfo.Add(book);
                        }
                    }
                }
                myConnection.Close();
            }

           // get book status
            foreach (var book in BookInfo)
            {
                // Get list of borrowed books
                List<Borrows> borrowedBooks = GetBorrows(book.BookID);

                // check if the book is booked out or available
                if (borrowedBooks.Where(k => k.BroughtDate == "").Count() == 1)
                {
                    book.Status = "Book Out";
                }
                else
                {
                    book.Status = "Available";
                }
            }
            return BookInfo;
        }

        // get Borrowed books information
        public List<Borrows> GetBorrows(int b = 0)
        {
            List<Borrows> BorrowedBooksInfo = new List<Borrows>();
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {

                myConnection.Open(); 

                string BorrowedBooksQuery =
                    " SELECT CONCAT( students.name,' ',students.surname) as Student, takenDate, broughtDate, borrows.bookId ,  borrows.borrowId FROM students " +
                    " INNER JOIN borrows on students.studentId = borrows.studentId " +
                    " INNER JOIN books on books.bookId = borrows.bookId " +
                    "WHERE borrows.bookId = " + b;

                if (b == 0)
                {
                    BorrowedBooksQuery =
                    " SELECT CONCAT( students.name,' ',students.surname) as Student, takenDate, broughtDate, borrows.bookId ,  borrows.borrowId FROM students " +
                    " INNER JOIN borrows on students.studentId= borrows.studentId " +
                    " INNER JOIN books on books.bookId = borrows.bookId ";
                }

                using (SqlCommand cmd = new SqlCommand(BorrowedBooksQuery, myConnection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Borrows book = new Borrows
                            {
                                BookID = Convert.ToInt32(reader["bookId"]),
                                BorrowID = Convert.ToInt32(reader["borrowId"]),
                                BroughtDate = reader["broughtDate"].ToString(),
                                TakenDate = reader["takenDate"].ToString(),
                                StudentName = reader["Student"].ToString(),
                            };
                            BorrowedBooksInfo.Add(book);
                        }
                    }
                }
                myConnection.Close();
            }


            return BorrowedBooksInfo;
        }

        //Get Authors information
        public List<Authors> GetAuthors()
        {
            List<Authors>AuthorsInfo = new List<Authors>();
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM authors", myConnection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Authors author = new Authors
                            {
                                AuthorID = Convert.ToInt32(reader["authorId"]),
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString()
                            };
                            AuthorsInfo.Add(author);
                        }
                    }
                }
                myConnection.Close();
            }
            return AuthorsInfo;

        }

        // get Book Types
        public List<Types> GetBookTypes()
        {
            List<Types> BookTypes = new List<Types>();
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM types", myConnection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Types type = new Types
                            {
                                TypeID= Convert.ToInt32(reader["typeId"]),
                                TypeName = reader["name"].ToString(),

                            };
                            BookTypes.Add(type);
                        }
                    }
                }
                myConnection.Close();
            }
            return BookTypes;

        }

        // get Students information
        public List<Student> GetStudents()
        {
            List<Student>StudentsInfo = new List<Student>();
            using (SqlConnection myConnection  = new SqlConnection(ConnectionString))
            {
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM students", myConnection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Student student = new Student
                            { 
                                StudentID = Convert.ToInt32(reader["studentId"]),
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString(),
                                Class = reader["class"].ToString(),
                                Point = Convert.ToInt32(reader["point"])

                            };
                            StudentsInfo.Add(student);
                        }
                    }
                }
                myConnection.Close();
            }
            return StudentsInfo;

        }
        //get Student's classes information
        public List<Class1> GetStudentClasses()
        {
            List<Class1> classesInfo = new List<Class1>();
            foreach (Student student in GetStudents())
            {
                Class1 student_class = new Class1
                {
                    ClassName = student.Class
                };
                if (classesInfo.Where(c => c.ClassName == student.Class).Count() == 0)
                {
                    classesInfo.Add(student_class);
                }
            }
            return classesInfo;
        }

        //Borrows book function
        public void BorrowBook(int BookId, int StudentId)
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                myConnection.Open();
                string BorrowsQuery = "INSERT INTO borrows( studentId, bookId, takenDate) " +
                    "values(@studentId,@bookId,@takenDate) ";

                using (SqlCommand cmd = new SqlCommand(BorrowsQuery, myConnection))
                {

                    cmd.Parameters.Add(new SqlParameter("@studentId", StudentId));
                    cmd.Parameters.Add(new SqlParameter("@bookId", BookId));
                    cmd.Parameters.Add(new SqlParameter("@takenDate", DateTime.Now));
                    cmd.ExecuteNonQuery();
                }
                myConnection.Close();
            }

            GetStudents().Where(s => s.StudentID == StudentId).FirstOrDefault().Book = true;

        }
        //Return book function

        public void ReturnBook(int BookId, int StudentId)
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                myConnection.Open();
                string ReturnQuery = "UPDATE borrows set broughtDate = @broughtDate where borrows.studentId = @studentId  AND borrows.bookId = @bookId and broughtDate IS NULL";
                ;
                using (SqlCommand cmd = new SqlCommand(ReturnQuery, myConnection))
                {

                    cmd.Parameters.Add(new SqlParameter("@studentId", StudentId));
                    cmd.Parameters.Add(new SqlParameter("@bookId", BookId));
                    cmd.Parameters.Add(new SqlParameter("@broughtDate", DateTime.Now));
                    cmd.ExecuteNonQuery();
                }
                myConnection.Close();
            }

        }
        //Book Search info
        public List<Books> BookSearch(string BookName, int BookType, int BookAuthor)
        {
            string BookSearchQuery = "search not found";


            // Search for Book Name only
            if (BookName !=null && BookType ==0 && BookAuthor== 0)
            {
                BookSearchQuery =
                " SELECT books.bookId as ID, books.pagecount as PageCount, books.point as Points, books.name as Name, types.name as Type, authors.surname as Author  FROM Books " +
                " INNER JOIN authors ON books.authorId = authors.authorId " +
                " INNER JOIN types ON books.typeId = types.typeId " +
                " WHERE books.name LIKE '%" + BookName + "%'";
            }
            // Search for Book Type only
            if (BookName == null && BookType > 0 && BookAuthor ==0)
            {
                BookSearchQuery =
                " SELECT books.bookId as ID, books.pagecount as PageCount, books.point as Points, books.name as Name, types.name as Type, authors.surname as Author  FROM Books " +
                " INNER JOIN authors on books.authorId = authors.authorId " +
                " INNER JOIN types on books.typeId = types.typeId " +
                " WHERE types.typeId LIKE '%" + BookType + " %'";
            }
         // Search for Book Author only
            if (BookName == null && BookType ==0 && BookAuthor > 0)
            {
                BookSearchQuery =
                " SELECT books.bookId as ID, books.pagecount as PageCount, books.point as Points, books.name as Name, types.name as Type, authors.surname as Author  FROM Books " +
                " INNER JOIN authors on books.authorId = authors.authorId " +
                " INNER JOIN types on books.typeId = types.typeId " +
                " WHERE authors.authorId LIKE '%" + BookAuthor + "%'";
            }

            // Search for Book type and author
            if (BookType >0 && BookAuthor >0)
            {
                BookSearchQuery =
                " SELECT books.bookId as ID, books.pagecount as PageCount, books.point as Points, books.name as Name, types.name as Type, authors.surname as Author  FROM Books " +
                " INNER JOIN authors on books.authorId = authors.authorId " +
                " INNER JOIN types on books.typeId = types.typeId " +
                " WHERE books.typeId = " + BookType + "AND books.authorId = " + BookAuthor;
            }

            // Search for Book type and name
            if (BookType > 0 && BookName != null)
            {
                BookSearchQuery =
                " SELECT books.bookId as ID, books.pagecount as PageCount, books.point as Points, books.name as Name, types.name as Type, authors.surname as Author  FROM Books " +
                " INNER JOIN authors on books.authorId = authors.authorId " +
                " INNER JOIN types on books.typeId = types.typeId " +
                " WHERE books.typeId = " + BookType + "AND books.name LIKE '%" + BookName + "%'";
            }

            // Search for Book author and name
            if (BookAuthor>0 && BookName != null)
            {
                BookSearchQuery =
                " SELECT books.bookId as ID, books.pagecount as PageCount, books.point as Points, books.name as Name, types.name as Type, authors.surname as Author  FROM Books " +
                " INNER JOIN authors on books.authorId = authors.authorId " +
                " INNER JOIN types on books.typeId = types.typeId " +
                " WHERE books.authorId="+ BookAuthor+"AND books.name LIKE '%" + BookName + "%'";
            }

            // Search for Book type, author and name
            if (BookType>0&& BookAuthor>0 && BookName !=null)
            {
                BookSearchQuery =
                " SELECT books.bookId as ID, books.pagecount as PageCount, books.point as Points, books.name as Name, types.name as Type, authors.surname as Author  FROM Books " +
                " INNER JOIN authors on books.authorId = authors.authorId " +
                " INNER JOIN types on books.typeId = types.typeId " +
                " WHERE books.typeId = " + BookType + "AND books.name LIKE '%" + BookName + "%'" + " AND books.authorId =" + BookAuthor;
            }

            
            List<Books> booksInfo = new List<Books>();
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                myConnection.Open();

                using (SqlCommand cmd = new SqlCommand(BookSearchQuery, myConnection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Books book = new Books
                            {
                                BookID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString(),
                                AuthorName = reader["Author"].ToString(),
                                PageCount = Convert.ToInt32(reader["PageCount"]),
                                Points = Convert.ToInt32(reader["Points"]),
                                BookType = reader["Type"].ToString()
                            };
                            booksInfo.Add(book);
                        }
                    }
                }
                myConnection.Close();
            }

            return booksInfo;
        }

        //Search for Student Info
        public List<Student> StudentSearchInfo(string sname,string sclass)
        {
            List<Student> StudentsInfo = new List<Student>();
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                myConnection.Open();

                string SearchStudentQuery = "Search not found";

                //Filter by student'sclass
                if (sclass != null)
                {
                    SearchStudentQuery = "SELECT * FROM students " +
                    "WHERE class LIKE '%" +sclass + "%'";
                }

                // Filter by both the student's name and class
                if (sname != null && sclass != null)
                {
                    SearchStudentQuery = "SELECT * FROM students" + " WHERE class LIKE '%" + sclass + "%' AND name LIKE '%" + sname + "%'";
                }
                //Filter by student's name 
                if (sclass == "none")
                {
                   SearchStudentQuery = "SELECT * FROM students "
                    + "WHERE name LIKE '%" + sname + "%'";
                }
            
                using (SqlCommand cmd = new SqlCommand(SearchStudentQuery, myConnection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            Student student = new Student
                            {
                                StudentID = Convert.ToInt32(reader["studentId"]),
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString(),
                                Class = reader["class"].ToString(),
                                Point = Convert.ToInt32(reader["point"])

                            };
                            StudentsInfo.Add(student);
                        }

                    }
                }
                myConnection.Close();
            }

            return StudentsInfo;
        }

       
            

    }
} 