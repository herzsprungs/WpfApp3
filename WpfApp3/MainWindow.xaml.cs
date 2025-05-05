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
using System.Windows.Threading;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Create a timer with 7 seconds interval
            timer = new DispatcherTimer();
            timer.Interval = System.TimeSpan.FromSeconds(7);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            // Stop the timer to prevent multiple triggers
            timer.Stop();

            // Create and show the LoginWindow
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

            // Close the current loading window
            this.Close();
        }
    }
}