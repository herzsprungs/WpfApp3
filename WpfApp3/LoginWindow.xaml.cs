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
using System.Windows.Shapes;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        // 1. Initialize the DataContext
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NorthVilleDatabase_by_RM_AND_JSConnectionString);

        public LoginWindow()
        {
            InitializeComponent();
        }

        // 2. Login Button Click
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = db.Users_tables.FirstOrDefault(u => u.Username == txtUsername.Text);

            if (user != null && user.PasswordHash == txtPassword.Password)
            {
                MessageBox.Show("Login Successful!", "Welcome", MessageBoxButton.OK, MessageBoxImage.Information);

                // Check if the username matches an email in the students table to get the student ID
                var student = db.NorthVilleStudents
                    .FirstOrDefault(s => s.N_Email.ToLower() == user.Username.ToLower()); // Case insensitive comparison

                if (student != null)
                {
                    // Store the logged-in student's ID globally (e.g., in App.Current.Properties or Session)
                    App.Current.Properties["LoggedInStudentID"] = student.N_StudentID;
                }

                // Handle user role-based navigation
                switch (user.Role)
                {
                    case "Librarian":
                        new LibrarianWindow().Show();
                        break;
                    case "Clerical":
                        new ClericalWindow().Show();
                        break;
                    case "Student":
                        new StudentWindow().Show();
                        break;
                    default:
                        MessageBox.Show("Unknown role. Access denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 3. Role Quick Access Buttons
        private void StudentTest_Click(object sender, RoutedEventArgs e)
        {
            new StudentWindow().Show();
            this.Close();
        }

        private void ClericalTest_Click(object sender, RoutedEventArgs e)
        {
            new ClericalWindow().Show();
            this.Close();
        }

        private void LibrarianTest_Click(object sender, RoutedEventArgs e)
        {
            new LibrarianWindow().Show();
            this.Close();
        }
    }
}