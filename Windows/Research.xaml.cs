using RC_IS.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private WindowState originalWindowState;
        private User _user;
        private Window _window;
        public Dashboard(User user, Window window)
        {
            InitializeComponent();
            originalWindowState = this.WindowState;
            _user = user;
            _window = window;
            lblWelcome.Text = "Welcome " + _user.Description;
            LoadPapers();
        }

        private void LoadPapers()
        {
            dgPapers.ItemsSource = null;
            DatabaseHandler dbHandler = new DatabaseHandler();
            List<Papers> papers = dbHandler.GetPapers();
            dgPapers.ItemsSource = papers;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            _window.Show();
            this.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddResearches_Click(object sender, RoutedEventArgs e)
        {
            AddResearch form = new AddResearch(_user, this);
            form.Show();
            this.Close();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        private void ToggleMaximize()
        {
            if (this.WindowState != WindowState.Maximized)
            {
                // Save the current window state before maximizing
                originalWindowState = this.WindowState;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                // Restore the window to the original size
                this.WindowState = originalWindowState;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dgPapers_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void txtYear_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, "[0-9]"))
            {
                e.Handled = true;
            }
        }

        private void txtYear_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string numericText = new string(textBox.Text.Where(char.IsDigit).ToArray());
            if (numericText.Length > 4)
            {
                numericText = numericText.Insert(4, "-");
            }
            if (numericText.Length > 9)
            {
                numericText = numericText.Substring(0, 9);
            }
            textBox.Text = numericText;
            textBox.CaretIndex = numericText.Length;
        }
    }
}
