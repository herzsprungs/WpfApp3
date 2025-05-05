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
        public LoginWindow()
        {
            InitializeComponent();
        }
        // Testing buttons handlers
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
