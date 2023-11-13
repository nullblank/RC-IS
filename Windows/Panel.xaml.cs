using RC_IS.Classes;
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

namespace RC_IS.Windows
{
    /// <summary>
    /// Interaction logic for Panel.xaml
    /// </summary>
    public partial class Panel : Window
    {
        private User _user;
        public Panel(User user)
        {
            _user = user;
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow form  = new MainWindow();
            form.Show();
        }

        private void btnResearchRecords_Click(object sender, RoutedEventArgs e)
        {
            Dashboard form = new Dashboard(_user);
            form.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblPanel.Text = "Panel - " + _user.Description;
        }

        private void btnResearchers_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("NOT YET IMPLEMENTED", "WARNING!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
