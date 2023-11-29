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
            Dashboard form = new Dashboard(_user, this);
            form.Show();
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO: This line of code loads data into the 'rCISDataSet1.Research' table. You can move, or remove it, as needed.
        }


        private void Oopsie()
        {
            MessageBox.Show("NOT YET IMPLEMENTED! TODO: Should set filter and open research records that make up of this statistic. Add option to export to excel for further auditing", "NOT SO FAST!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void btnFilter_SEAIT_Click(object sender, RoutedEventArgs e)
        {
            Oopsie();
        }

        private void btnFilter_SAB_Click(object sender, RoutedEventArgs e)
        {
            Oopsie();
        }

        private void btnFilter_SHANS_Click(object sender, RoutedEventArgs e)
        {
            Oopsie();
        }

        private void btnFilter_STEH_Click(object sender, RoutedEventArgs e)
        {
            Oopsie();
        }

        private void btnFilter_GS_Click(object sender, RoutedEventArgs e)
        {
            Oopsie();
        }

    }
}
