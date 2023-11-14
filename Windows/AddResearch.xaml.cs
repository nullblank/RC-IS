using RC_IS.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for AddResearch.xaml
    /// </summary>
    public partial class AddResearch : Window
    {
        private WindowState originalWindowState;
        private User _user;
        private List<Schools> _schools;
        public AddResearch(User user)
        {
            InitializeComponent();
            DatabaseHandler dbHander = new DatabaseHandler();
            originalWindowState = this.WindowState;
            _user = user;
            LoadSchoolData();
        }

        private void LoadSchoolData()
        {
            txtSchool.ItemsSource = null;
            DatabaseHandler dbHandler = new DatabaseHandler();
            _schools = dbHandler.GetSchoolData();
            txtSchool.ItemsSource = _schools;
        }

        private void txtSchool_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Schools selectedData = (Schools)txtSchool.SelectedItem;
            int selectedId = selectedData.Id;
            Trace.WriteLine("SELECTED SCHOOL ID: " + selectedId.ToString());
            LoadProgramData(selectedId);
        }

        private void LoadProgramData(int schoolId)
        {
            txtCourse.ItemsSource = null;
            DatabaseHandler dbHandler = new DatabaseHandler();
            List<Programs> programs = dbHandler.GetProgramData(schoolId);
            txtCourse.ItemsSource = programs;
        }

        
        private void txtCourse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Programs selectedData = (Programs)txtCourse.SelectedItem;
            if (selectedData != null)
            {
                int selectedId = selectedData.Id;
                Trace.WriteLine("SELECTED COURSE ID: " + selectedId.ToString());
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void Window_MouseDown_1(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
