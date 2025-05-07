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
    /// Interaction logic for StudentViewAvailableBooksPage.xaml
    /// </summary>
    public partial class StudentViewAvailableBooksPage : Page
    {
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NorthVilleDatabase_by_RM_AND_JSConnectionString);

        public StudentViewAvailableBooksPage()
        {
            InitializeComponent();
            LoadAvailableBooks();
        }

        private void LoadAvailableBooks()
        {
            var books = db.vw_AvailableCopies.ToList();
            BooksDataGrid.ItemsSource = books;
        }

        private void BorrowButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (sender as Button)?.DataContext;

            if (selectedItem is vw_AvailableCopy selectedBook)
            {
                string studentID = GetLoggedInStudentID();

                if (studentID == null)
                {
                    MessageBox.Show("Unable to identify logged-in student.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get BookID (just pick one with that ISBN; SP handles availability)
                var book = db.Books.FirstOrDefault(b => b.I_ISBNID == selectedBook.I_ISBNID);
                if (book == null)
                {
                    MessageBox.Show("No matching book found.", "Not Available", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    db.sp_BorrowBook(studentID, book.B_BookID, DateTime.Now);
                    MessageBox.Show("Book borrowed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAvailableBooks(); // Refresh grid
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Borrow failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string GetLoggedInStudentID()
        {
            // Fetch the logged-in student's ID from App.Current.Properties (set during login)
            if (App.Current.Properties["LoggedInStudentID"] is string studentId)
            {
                return studentId;
            }

            return null; // Return null if no student ID is found
        }
    }
}
