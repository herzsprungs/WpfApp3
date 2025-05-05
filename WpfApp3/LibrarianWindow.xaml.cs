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
    /// Interaction logic for LibrarianWindow.xaml
    /// </summary>
    public partial class LibrarianWindow : Window
    {
        public LibrarianWindow()
        {
            InitializeComponent();
            NavigateToPage("Users"); // Default page
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            NavigateToPage(button.Tag.ToString());
        }

        private void NavigateToPage(string pageName)
        {
            switch (pageName)
            {
                case "Users":
                    MainFrame.Navigate(new LibrarianAdminUsersCRUDpage());
                    break;
                case "Books":
                    MainFrame.Navigate(new LibrarianAdminBooksCRUDpage());
                    break;
                case "Students":
                    MainFrame.Navigate(new LibrarianAdminStudentsCRUDpage());
                    break;
                case "Courses":
                    MainFrame.Navigate(new LibrarianAdminCourseCRUDpage());
                    break;
                case "Transactions":
                    MainFrame.Navigate(new LibrarianAdminTransactionCRUDpage());
                    break;
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}
