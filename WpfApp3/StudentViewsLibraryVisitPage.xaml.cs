using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
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
    /// Interaction logic for StudentViewsLibraryVisitPage.xaml
    /// </summary>
    public partial class StudentViewsLibraryVisitPage : Page
    {
        private DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NorthVilleDatabase_by_RM_AND_JSConnectionString);
        private string studentId;

        public StudentViewsLibraryVisitPage(string loggedInStudentId)
        {
            InitializeComponent();
            studentId = loggedInStudentId;
            StudentIdTextBox.Text = studentId;

            TimeInButton.Click += TimeInButton_Click;
            TimeOutButton.Click += TimeOutButton_Click;
        }

        private void TimeInButton_Click(object sender, RoutedEventArgs e)
        {
            string studentId = StudentIdTextBox.Text.Trim();

            var student = db.NorthVilleStudents.FirstOrDefault(s => s.N_StudentID == studentId);
            if (student == null)
            {
                MessageBox.Show("Student ID not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var latestTransaction = db.BorrowTransactions
                .Where(t => t.N_StudentID == studentId)
                .OrderByDescending(t => t.T_BorrowDate)
                .FirstOrDefault();

            // Retrieve the highest L_LibraryVisitID in the format LVxxx
            var latestVisit = db.LibraryVisits
    .Where(v => v.L_LibraryVisitID.StartsWith("LV")) // Filter only those starting with "LV"
    .ToList()  // Execute the query and bring the data into memory
    .Where(v => !string.IsNullOrEmpty(v.L_LibraryVisitID)) // Now filter for non-empty IDs in memory
    .OrderByDescending(v => v.L_LibraryVisitID)
    .FirstOrDefault();

            int nextVisitId = 1; // Default value if no records exist

            if (latestVisit != null)
            {
                // Extract the numeric part of the L_LibraryVisitID (e.g., LV001 -> 1)
                var numericPart = latestVisit.L_LibraryVisitID.Substring(2);
                if (int.TryParse(numericPart, out int lastId))
                {
                    nextVisitId = lastId + 1;
                }
            }

            // Format the nextVisitId to match the "LVxxx" pattern
            string formattedVisitId = "LV" + nextVisitId.ToString("D3"); // "D3" ensures 3 digits, e.g., LV001, LV002

            var visit = new LibraryVisit
            {
                L_LibraryVisitID = formattedVisitId,  // Use the formatted ID
                N_StudentID = studentId,
                T_TransactionID = latestTransaction?.T_TransactionID,
                L_LibraryVisitDate = DateTime.Today.ToString("yyyy-MM-dd"),  // string
                L_LibraryInTime = DateTime.Now.TimeOfDay,                    // TimeSpan?
                L_LibraryOutTime = null
            };

            db.LibraryVisits.InsertOnSubmit(visit);
            db.SubmitChanges();

            TimeInValue.Text = visit.L_LibraryInTime?.ToString(@"hh\:mm");
            StatusText.Text = "Checked in at " + TimeInValue.Text;
            MessageBox.Show("Check-in successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TimeOutButton_Click(object sender, RoutedEventArgs e)
        {
            string studentId = StudentIdTextBox.Text.Trim();

            var visit = db.LibraryVisits
                .Where(v => v.N_StudentID == studentId &&
                            v.L_LibraryOutTime == null)
                .OrderByDescending(v => v.L_LibraryVisitID)
                .FirstOrDefault();

            // Handle the date parsing after retrieving the records
            if (visit != null && DateTime.TryParse(visit.L_LibraryVisitDate, out DateTime visitDate))
            {
                if (visitDate.Date == DateTime.Today)
                {
                    // Proceed with check-out logic if the visit date is today
                    visit.L_LibraryOutTime = DateTime.Now.TimeOfDay;
                    db.SubmitChanges();

                    TimeOutValue.Text = visit.L_LibraryOutTime?.ToString(@"hh\:mm");
                    StatusText.Text = "Checked out at " + TimeOutValue.Text;
                    MessageBox.Show("Check-out successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Navigate back to LoginWindow
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();

                    // Close the current window (if it's hosted in a Window)
                    Window.GetWindow(this)?.Close();
                }
                else
                {
                    MessageBox.Show("No active visit found for today!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No active visit found for today!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
