using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for StudentViewBorrowBooksPage.xaml
    /// </summary>
    public partial class StudentViewBorrowBooksPage : Page
    {
        private DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NorthVilleDatabase_by_RM_AND_JSConnectionString);

        public StudentViewBorrowBooksPage()
        {
            InitializeComponent();
        }

        private void ViewBorrowing_Click(object sender, RoutedEventArgs e)
        {
            // Get the logged-in user's UserID from App.Current.Properties (set this during login)
            if (!(App.Current.Properties["LoggedInUserID"] is int loggedInUserID))
            {
                MessageBox.Show("Logged-in user not found. Please log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Join Users_table with NorthvilleStudent to get student record
            var student = (from s in db.NorthVilleStudents
                           join u in db.Users_tables on s.N_Email equals u.Username
                           where u.UserID == loggedInUserID
                           select s).FirstOrDefault();

            if (student != null)
            {
                string studentID = student.N_StudentID;

                // Query BorrowTransaction and related tables to fetch borrowed books
                var borrowedBooks = from bt in db.BorrowTransactions
                                    join b in db.Books on bt.B_BookID equals b.B_BookID
                                    join i in db.ISBNs on b.I_ISBNID equals i.I_ISBNID
                                    where bt.N_StudentID == studentID
                                    select new
                                    {
                                        BookTitle = i.I_BookName,
                                        Author = i.I_BookAuthor,
                                        BorrowDate = bt.T_BorrowDate,
                                        ReturnDate = bt.T_ReturnDate,
                                        Status = bt.T_TransactionNote
                                    };

                if (borrowedBooks.Any())
                {
                    string borrowedBooksMessage = "Borrowed Books:\n";
                    foreach (var book in borrowedBooks)
                    {
                        string borrowDateStr = book.BorrowDate;
                        string returnDateStr = book.ReturnDate;

                        DateTime borrowDate;
                        DateTime? returnDate = null;

                        if (DateTime.TryParse(borrowDateStr, out borrowDate))
                            borrowDateStr = borrowDate.ToShortDateString();
                        else
                            borrowDateStr = "Invalid date";

                        if (!string.IsNullOrEmpty(returnDateStr) && DateTime.TryParse(returnDateStr, out DateTime parsedReturnDate))
                            returnDate = parsedReturnDate;

                        string returnDateFormatted = returnDate?.ToShortDateString() ?? "Not returned yet";

                        borrowedBooksMessage += $"\nTitle: {book.BookTitle}\n" +
                                                $"Author: {book.Author}\n" +
                                                $"Borrow Date: {borrowDateStr}\n" +
                                                $"Return Date: {returnDateFormatted}\n" +
                                                $"Status: {book.Status}\n" +
                                                "---------------------------------------";
                    }

                    MessageBox.Show(borrowedBooksMessage, "Borrowed Books", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("You have not borrowed any books.", "No Borrowed Books", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Student record not found for this user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}