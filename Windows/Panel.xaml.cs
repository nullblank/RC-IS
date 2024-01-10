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
            LoadStatisctics(); // Populates cbYear only with school years that exist in the currently registered research papers.
            cbYear.SelectedIndex = 0; // Selects the most recent school year in the records.
            if (cbYear.SelectedValue != null) // Loads all school's research paper count in a given school year.
            {
                LoadPaperCount();
            }
        }

        private void LoadPaperCount() // Loads all school's research paper count in a given school year.
        {
            int year = FormatYearD(cbYear.SelectedValue.ToString()); // Extracting the selected year from the combo box and formatting the string.
            DatabaseHandler dbHandler = new DatabaseHandler(); // Creating an instance of the DatabaseHandler class
            lblSEAIT.Text = dbHandler.GetPaperCount(year, 1); // Retrieving and displaying the paper count for the schools based on the current school year.
            lblSAB.Text = dbHandler.GetPaperCount(year, 2);
            lblSHANS.Text = dbHandler.GetPaperCount(year, 3);
            lblSTEH.Text = dbHandler.GetPaperCount(year, 4);
            lblGS.Text = dbHandler.GetPaperCount(year, 5);
        }

        private void LoadStatisctics() // Populates cbYear only with school years that exist in the currently registered research papers.
        {
            DatabaseHandler dbHandler = new DatabaseHandler();
            List<int> year = dbHandler.GetExistingSchoolYears(); // This function grabs the distinct school years that exist in the database for listing.
            cbYear.Items.Clear(); // Clears the combobox
            foreach (int y in year){ cbYear.Items.Add(FormatYear(y.ToString())); } // Iterates over each of the years in the list, formats then adds them to the combobox
        }
        private void cbYear_SelectionChanged(object sender, SelectionChangedEventArgs e) // Event handler for whenever a new school year is selected.
        {
            LoadPaperCount();
            lblYear.Text = cbYear.SelectedValue.ToString();
        }
        static int FormatYearD(string input) // Helper function.
        {
            if (input.Length != 9 || input[4] != '-' || !int.TryParse(input.Replace("-", ""), out _))
            {
                throw new ArgumentException("Invalid input. It should be in the format '####-####'.");
            }

            return int.Parse(input.Replace("-", ""));
        }
        static string FormatYear(string input) // Helper function.
        {
            if (input.Length != 8 || !int.TryParse(input, out _))
            {
                throw new ArgumentException("Invalid input. It should be an eight-digit string.");
            }
            return $"{input.Substring(0, 4)}-{input.Substring(4, 4)}";
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) // Function used to allow draging of the window
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) // Minimize Window
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) // Close this window and initialize and open Login Window
        {
            this.Hide();
            MainWindow form  = new MainWindow();
            form.Show();
        }

        private void btnResearchRecords_Click(object sender, RoutedEventArgs e) // Closes this window and opens the Dashboard
        {
            Dashboard form = new Dashboard(_user, this);
            form.Show();
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO: This line of code loads data into the 'rCISDataSet1.Research' table. You can move, or remove it, as needed or whatever lol.
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
