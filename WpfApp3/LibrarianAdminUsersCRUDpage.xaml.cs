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

        public LibrarianAdminUsersCRUDpage()
        {
            InitializeComponent();
            LoadUsers();
            usersDataGrid.CellEditEnding += UsersDataGrid_CellEditEnding;
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

        private void UsersDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var editedItem = e.Row.Item as UserDisplay;
                if (editedItem != null)
                {
                    var user = db.Users_tables.FirstOrDefault(u => u.UserID == editedItem.UserID);
                    if (user != null)
                    {
                        // Update the properties that were edited
                        user.Username = editedItem.Username;
                        user.Role = editedItem.Role;

                        // Only update password if it was changed (you might want to add a special editor for this)
                        // user.PasswordHash = HashPassword(editedItem.PasswordHash);

                        try
                        {
                            db.SubmitChanges();
                            MessageBox.Show("User updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error updating user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            // Refresh the data to show the original values
                            LoadUsers();
                        }
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

        private string HashPassword(string password)
        {
            // Implement proper password hashing here
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
