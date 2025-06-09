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
    
    public partial class LoginWindow : Window
    {
        
        DataClasses2DataContext db = new DataClasses2DataContext(Properties.Settings.Default.FacultyEvaluationConnectionString);

        public LoginWindow()
        {
            InitializeComponent();
        }


        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = db.UserLogins.FirstOrDefault(u => u.U_UserName == txtUsername.Text && u.U_Password == txtPassword.Password);

            if (user != null)
            {
                MessageBox.Show("Login Successful!", "Welcome", MessageBoxButton.OK, MessageBoxImage.Information);

                if (user.U_Role == "Student")
                {
                    var student = db.StudentsSouthvilles.FirstOrDefault(s => s.U_UserID == user.U_UserID);
                    if (student != null)
                    {
                        App.Current.Properties["LoggedInStudentID"] = student.S_StudentID;
                        new StudentWindowFacultyEval().Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Student information not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (user.U_Role == "Admin")
                {
                    var admin = db.AdminFacultyEvals.FirstOrDefault(a => a.U_UserID == user.U_UserID);
                    if (admin != null)
                    {
                        App.Current.Properties["LoggedInAdminID"] = admin.A_AdminID;
                        new AdminWindowFacultyEval().Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Admin information not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Unknown role. Access denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



    }
}