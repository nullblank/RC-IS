using RC_IS.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            LoadStatisctics();
            cbYear.SelectedIndex = 0;
            if (cbYear.SelectedValue != null)
            {
                
                LoadPaperCount();
            }
        }

        private void LoadPaperCount()
        {
            int year = FormatYearD(cbYear.SelectedValue.ToString());
            DatabaseHandler dbHandler = new DatabaseHandler();
            lblSEAIT.Text = dbHandler.GetPaperCount(year, 1);
            lblSAB.Text = dbHandler.GetPaperCount(year, 2);
            lblSHANS.Text = dbHandler.GetPaperCount(year, 3);
            lblSTEH.Text = dbHandler.GetPaperCount(year, 4);
            //lblGS.Text = dbHandler.GetPaperCount(year, 5);
        }

        private void LoadStatisctics()
        {
            DatabaseHandler dbHandler = new DatabaseHandler();
            List<int> year = dbHandler.GetYear();
            cbYear.Items.Clear();
            foreach (int y in year)
            {
                cbYear.Items.Add(FormatYear(y.ToString()));
            }
        }

        static int FormatYearD(string input)
        {
            if (input.Length != 9 || input[4] != '-' || !int.TryParse(input.Replace("-", ""), out _))
            {
                throw new ArgumentException("Invalid input. It should be in the format '####-####'.");
            }

            return int.Parse(input.Replace("-", ""));
        }
        static string FormatYear(string input)
        {
            if (input.Length != 8 || !int.TryParse(input, out _))
            {
                throw new ArgumentException("Invalid input. It should be an eight-digit string.");
            }
            return $"{input.Substring(0, 4)}-{input.Substring(4, 4)}";
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

        private void cbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadPaperCount();
            lblYear.Text = cbYear.SelectedValue.ToString();
        }
    }
}
