using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    
    public partial class ClericalViewLibraryTimeInTimeOut : Page
    {
        private DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NorthVilleDatabase_by_RM_AND_JSConnectionString);
        private string studentId;

        public ClericalViewLibraryTimeInTimeOut()
        {
            InitializeComponent();
            
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

            
            var latestVisit = db.LibraryVisits
            .Where(v => v.L_LibraryVisitID.StartsWith("LV")) 
            .ToList()  
            .Where(v => !string.IsNullOrEmpty(v.L_LibraryVisitID)) 
            .OrderByDescending(v => v.L_LibraryVisitID)
            .FirstOrDefault();

            int nextVisitId = 1; 

            if (latestVisit != null)
            {
                
                var numericPart = latestVisit.L_LibraryVisitID.Substring(2);
                if (int.TryParse(numericPart, out int lastId))
                {
                    nextVisitId = lastId + 1;
                }
            }

            
            string formattedVisitId = "LV" + nextVisitId.ToString("D3"); 

            var visit = new LibraryVisit
            {
                L_LibraryVisitID = formattedVisitId, 
                N_StudentID = studentId,
                T_TransactionID = latestTransaction?.T_TransactionID,
                L_LibraryVisitDate = DateTime.Today.ToString("yyyy-MM-dd"),  
                L_LibraryInTime = DateTime.Now.TimeOfDay,                    
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

            
            if (visit != null && DateTime.TryParse(visit.L_LibraryVisitDate, out DateTime visitDate))
            {
                if (visitDate.Date == DateTime.Today)
                {
                    
                    visit.L_LibraryOutTime = DateTime.Now.TimeOfDay;
                    db.SubmitChanges();

                    TimeOutValue.Text = visit.L_LibraryOutTime?.ToString(@"hh\:mm");
                    StatusText.Text = "Checked out at " + TimeOutValue.Text;
                    MessageBox.Show("Check-out successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();

                   
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