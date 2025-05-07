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
    /// Interaction logic for StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        public string loggedInStudentId; // field to store student ID
        public StudentWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new StudentViewAvailableBooksPage());
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            switch (button.Tag.ToString())
            {
                case "AvailableBooks":
                    MainFrame.Navigate(new StudentViewAvailableBooksPage());
                    break;
                case "BorrowBooks":
                    MainFrame.Navigate(new StudentViewBorrowBooksPage());

                    break;
                case "LibraryVisits":
                    MainFrame.Navigate(new StudentViewsLibraryVisitPage(loggedInStudentId));
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