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
    /// Interaction logic for ClericalViewsManageBooksPage.xaml
    /// </summary>
    public partial class ClericalViewsManageBooksPage : Page
    {
        private DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NorthVilleDatabase_by_RM_AND_JSConnectionString);
        public ClericalViewsManageBooksPage()
        {
            InitializeComponent();
            LoadBooks();
        }

        public class BookDisplay
        {
            public string B_BookID { get; set; }
            public ISBN ISBN { get; set; }
            public string G_GenreID { get; set; }
        }

        private void LoadBooks()
        {
            var books = db.Books
                .Select(b => new BookDisplay
                {
                    B_BookID = b.B_BookID,
                    ISBN = b.ISBN,
                    G_GenreID = b.G_GenreID
                }).ToList();

            booksDataGrid.ItemsSource = books;
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateBookInputs()) return;

            var newISBN = new ISBN
            {
                I_ISBNID = txtISBN.Text.Trim(),
                I_BookName = txtBookTitle.Text.Trim(),
                I_BookAuthor = txtAuthor.Text.Trim(),
                I_PublicationYear = txtPubYear.Text.Trim() // Store as string
            };

            var newBook = new Book
            {
                B_BookID = Guid.NewGuid().ToString(), // Generate string ID
                ISBN = newISBN,
                G_GenreID = "1" // Default genre as string
            };

            try
            {
                db.ISBNs.InsertOnSubmit(newISBN);
                db.Books.InsertOnSubmit(newBook);
                db.SubmitChanges();

                MessageBox.Show("Book added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadBooks();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding book: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateBookInputs()
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtISBN.Text) ||
                string.IsNullOrWhiteSpace(txtBookTitle.Text) ||
                string.IsNullOrWhiteSpace(txtAuthor.Text) ||
                string.IsNullOrWhiteSpace(txtPubYear.Text))
            {
                MessageBox.Show("Please fill in all fields", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate publication year format
            if (!int.TryParse(txtPubYear.Text, out int year) || year < 1900 || year > DateTime.Now.Year)
            {
                MessageBox.Show("Please enter a valid publication year (1900-current year)",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BooksDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var editedItem = e.Row.Item as BookDisplay;
                if (editedItem != null)
                {
                    var book = db.Books.FirstOrDefault(b => b.B_BookID == editedItem.B_BookID);
                    var isbn = db.ISBNs.FirstOrDefault(i => i.I_ISBNID == editedItem.ISBN.I_ISBNID);

                    if (book != null && isbn != null)
                    {
                        // Update ISBN information
                        isbn.I_BookName = editedItem.ISBN.I_BookName;
                        isbn.I_BookAuthor = editedItem.ISBN.I_BookAuthor;

                        // Validate and update publication year
                        if (int.TryParse(editedItem.ISBN.I_PublicationYear, out int validYear))
                        {
                            isbn.I_PublicationYear = validYear.ToString();
                        }

                        // Update genre ID
                        book.G_GenreID = editedItem.G_GenreID;

                        db.SubmitChanges();
                    }
                }
            }
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                string bookId = btn.Tag.ToString();
                var book = db.Books.FirstOrDefault(b => b.B_BookID == bookId);

                if (book != null)
                {
                    var confirm = MessageBox.Show($"Delete book '{book.ISBN.I_BookName}'?",
                        "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (confirm == MessageBoxResult.Yes)
                    {
                        var isbnUsage = db.Books.Count(b => b.I_ISBNID == book.I_ISBNID);

                        db.Books.DeleteOnSubmit(book);
                        if (isbnUsage == 1) db.ISBNs.DeleteOnSubmit(book.ISBN);

                        db.SubmitChanges();
                        LoadBooks();
                    }
                }
            }
        }

        private void ClearInputs()
        {
            txtISBN.Clear();
            txtBookTitle.Clear();
            txtAuthor.Clear();
            txtPubYear.Clear();
        }
    }
}
