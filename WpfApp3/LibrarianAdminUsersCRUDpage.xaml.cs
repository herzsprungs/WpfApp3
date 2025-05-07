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
    
    public partial class LibrarianAdminUsersCRUDpage : Page
    {
        private DataClasses1DataContext db = new DataClasses1DataContext();
        private Users_table _selectedUser;

        public LibrarianAdminUsersCRUDpage()
        {
            InitializeComponent();
            LoadUsers();
        }

        public class UserDisplay
        {
            public int UserID { get; set; }  
            public string Username { get; set; }
            public string Role { get; set; }
            public string PasswordHash { get; set; }
        }

        private void LoadUsers()
        {
            var users = db.Users_tables
                .Select(u => new UserDisplay
                {
                    UserID = u.UserID,
                    Username = u.Username,
                    PasswordHash = u.PasswordHash, 
                    Role = u.Role
                }).ToList();

            usersDataGrid.ItemsSource = users;
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewUsername.Text) ||
                string.IsNullOrWhiteSpace(txtNewPass.Password))
            {
                MessageBox.Show("Please fill all required fields");
                return;
            }

          
            int nextUserId = db.Users_tables.Any()
                ? db.Users_tables.Max(u => u.UserID) + 1
                : 1;

            var newUser = new Users_table
            {
                UserID = nextUserId,  
                Username = txtNewUsername.Text.Trim(),
                PasswordHash = HashPassword(txtNewPass.Password),
                Role = (cmbRole.SelectedItem as ComboBoxItem)?.Content.ToString()
            };

            try
            {
                db.Users_tables.InsertOnSubmit(newUser);
                db.SubmitChanges();
                LoadUsers();
                ClearAddForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding user: {ex.Message}");
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                int userId;
                if (int.TryParse(btn.Tag.ToString(), out userId))  // Convert userId from string to int
                {
                    _selectedUser = db.Users_tables.FirstOrDefault(u => u.UserID == userId);

                    if (_selectedUser != null)
                    {
                        
                        txtNewUsername.Text = _selectedUser.Username;
                        cmbRole.SelectedValue = _selectedUser.Role;
                        MessageBox.Show($"Editing user: {_selectedUser.Username}");
                    }
                }
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                int userId;
                if (int.TryParse(btn.Tag.ToString(), out userId))  
                {
                    var user = db.Users_tables.FirstOrDefault(u => u.UserID == userId);

                    if (user != null && MessageBox.Show($"Delete user {user.Username}?",
                        "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        db.Users_tables.DeleteOnSubmit(user);
                        db.SubmitChanges();
                        LoadUsers();
                    }
                }
            }
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedUser = usersDataGrid.SelectedItem as Users_table;
        }

        private string HashPassword(string password)
        {
           
            return password;
        }

        private void ClearAddForm()
        {
            txtNewUsername.Clear();
            txtNewPass.Clear();
            cmbRole.SelectedIndex = 0;
        }
    }
}
