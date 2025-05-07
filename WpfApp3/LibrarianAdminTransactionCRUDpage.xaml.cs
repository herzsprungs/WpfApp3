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
    /// Interaction logic for LibrarianAdminTransactionCRUDpage.xaml
    /// </summary>
    public partial class LibrarianAdminTransactionCRUDpage : Page
    {
        private DataClasses1DataContext db = new DataClasses1DataContext();
        private BorrowTransaction _selectedTransaction;

        public LibrarianAdminTransactionCRUDpage()
        {
            InitializeComponent();
            LoadTransactions();
            GenerateNextTransactionId(); // Add this line
        }

        public class TransactionDisplay
        {
            public string T_TransactionID { get; set; }
            public string N_StudentID { get; set; }
            public string T_BorrowDate { get; set; }
            public string T_ReturnDate { get; set; }
            public string T_TransactionNote { get; set; }
        }

        private void LoadTransactions()
        {
            var transactions = db.BorrowTransactions
                .Select(t => new TransactionDisplay
                {
                    T_TransactionID = t.T_TransactionID,
                    N_StudentID = t.N_StudentID,
                    T_BorrowDate = t.T_BorrowDate,
                    T_ReturnDate = t.T_ReturnDate,
                    T_TransactionNote = t.T_TransactionNote
                }).ToList();

            transactionsDataGrid.ItemsSource = transactions;
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTransactionID.Text) ||
                string.IsNullOrWhiteSpace(txtStudentID.Text) ||
                string.IsNullOrWhiteSpace(txtBorrowDate.Text))
            {
                MessageBox.Show("Please fill in Transaction ID, Student ID, and Borrow Date.");
                return;
            }

            var newTransaction = new BorrowTransaction
            {
                T_TransactionID = txtTransactionID.Text.Trim(),
                N_StudentID = txtStudentID.Text.Trim(),
                T_BorrowDate = txtBorrowDate.Text.Trim(),
                T_ReturnDate = txtReturnDate.Text.Trim(),
                T_TransactionNote = "Borrowed" // Default note
            };

            try
            {
                db.BorrowTransactions.InsertOnSubmit(newTransaction);
                db.SubmitChanges();
                LoadTransactions();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding transaction: {ex.Message}");
            }
        }

        private void EditTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                string transactionId = btn.Tag.ToString();
                _selectedTransaction = db.BorrowTransactions.FirstOrDefault(t => t.T_TransactionID == transactionId);

                if (_selectedTransaction != null)
                {
                    txtTransactionID.Text = _selectedTransaction.T_TransactionID;
                    txtStudentID.Text = _selectedTransaction.N_StudentID;
                    txtBorrowDate.Text = _selectedTransaction.T_BorrowDate;
                    txtReturnDate.Text = _selectedTransaction.T_ReturnDate;

                    MessageBox.Show($"Editing transaction: {_selectedTransaction.T_TransactionID}");
                }
            }
        }

        private void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                string transactionId = btn.Tag.ToString();
                var transaction = db.BorrowTransactions.FirstOrDefault(t => t.T_TransactionID == transactionId);

                if (transaction != null && MessageBox.Show($"Delete transaction {transaction.T_TransactionID}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    db.BorrowTransactions.DeleteOnSubmit(transaction);
                    db.SubmitChanges();
                    LoadTransactions();
                }
            }
        }

        private void TransactionsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedTransaction = transactionsDataGrid.SelectedItem as BorrowTransaction;
        }

        private void GenerateNextTransactionId()
        {
            // Get the highest existing transaction ID
            var lastTransaction = db.BorrowTransactions
                .OrderByDescending(t => t.T_TransactionID)
                .FirstOrDefault();

            if (lastTransaction != null && lastTransaction.T_TransactionID.StartsWith("T"))
            {
                // Extract the numeric part and increment
                if (int.TryParse(lastTransaction.T_TransactionID.Substring(1), out int lastNumber))
                {
                    txtTransactionID.Text = $"T{(lastNumber + 1).ToString("D3")}";
                    return;
                }
            }

            // Default to T001 if no transactions exist or format is invalid
            txtTransactionID.Text = "T001";
        } 

        private void ClearForm()
        {
            GenerateNextTransactionId(); // Regenerate ID when form is cleared
            txtStudentID.Clear();
            txtBorrowDate.Clear();
            txtReturnDate.Clear();
        }
    }
}
