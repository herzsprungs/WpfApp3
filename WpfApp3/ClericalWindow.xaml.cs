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
    /// Interaction logic for ClericalWindow.xaml
    /// </summary>
    public partial class ClericalWindow : Window
    {
        public ClericalWindow()
        {
            InitializeComponent();
            NavigateToPage("ManageBooks");
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
                case "ManageBooks":
                    MainFrame.Navigate(new ClericalViewsManageBooksPage());
                    break;
                case "Analytics":
                    MainFrame.Navigate(new ClericalViewsTransactionPage());
                    break;
                case "CheckAvailability":
                    MainFrame.Navigate(new StudentViewAvailableBooksPage());
                    break;

                case "TimeTracking":
                    MainFrame.Navigate(new ClericalViewLibraryTimeInTimeOut());
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