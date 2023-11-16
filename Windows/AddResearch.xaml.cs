using RC_IS.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public AddResearch(User user) // Constructor for AddResearch window (called from MainWindow) 
        {
            InitializeComponent();
            DatabaseHandler dbHander = new DatabaseHandler();
            originalWindowState = this.WindowState;
            _user = user;
            LoadSchoolData();
        }

        // ------------- Methods -------------

        private void ToggleMaximize() // Maximize window to fullscreen and vice versa
        {
            if (this.WindowState != WindowState.Maximized)
            {
                originalWindowState = this.WindowState;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = originalWindowState;
            }
        }
        
        private void LoadSchoolData() // Load school data from database to combobox (txtSchool)
        {
            txtSchool.ItemsSource = null;
            DatabaseHandler dbHandler = new DatabaseHandler();
            List<Schools> schools = dbHandler.GetSchoolData();
            txtSchool.ItemsSource = schools;
        }

        private void LoadProgramData(int schoolId) // Load program data from database to combobox (txtCourse)
        {
            txtCourse.ItemsSource = null;
            DatabaseHandler dbHandler = new DatabaseHandler();
            List<Programs> programs = dbHandler.GetProgramData(schoolId);
            txtCourse.ItemsSource = programs;
        }


        // --------------- Event Handlers ---------------
        private void txtSchool_SelectionChanged(object sender, SelectionChangedEventArgs e) // When a school is selected, load the programs of that school
        {
            Schools selectedData = (Schools)txtSchool.SelectedItem;
            int selectedId = selectedData.Id;
            Trace.WriteLine("SELECTED SCHOOL ID: " + selectedId.ToString());
            LoadProgramData(selectedId);
        }

        

        
        private void txtCourse_SelectionChanged(object sender, SelectionChangedEventArgs e) // When a program is selected, assign ID
        {
            Programs selectedData = (Programs)txtCourse.SelectedItem;
            if (selectedData != null)
            {
                int selectedId = selectedData.Id;
                Trace.WriteLine("SELECTED COURSE ID: " + selectedId.ToString());
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) // Close window button event handler (btnClose)
        {
            this.Close();
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e) // Maximize window button event handler (btnMaximize)
        {
            ToggleMaximize();
        }

        

        private void btnMinimize_Click(object sender, RoutedEventArgs e) // Minimize window button event handler (btnMinimize)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) // Window drag event handler (Window_MouseDown)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btnAddUnRegStaff_Click(object sender, RoutedEventArgs e) // Set unregistered staff as research paper adviser. 
        {
            MessageBoxResult adviserBoxResult =  MessageBox.Show("The current name is going to be set as an UNREGISTERED ENTITY in the system. Do you wish to procede?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (adviserBoxResult.Equals(MessageBoxResult.OK))
            {
                //add constructor here
                lblSelectedAdviser.Text = txtSearchAdviser.Text;
            }
            else
            {
                txtSearchAdviser.Text = "NO ADVISER SELECTED";
            }
        }

        private async void txtSearchResearcher_TextChanged(object sender, TextChangedEventArgs e) // Search researcher event handler (txtSearchResearcher) (async) (await)
        { 
            dgResearchersList.ItemsSource = null;
            string searchKeyword = txtSearchResearcher.Text;

            MSDatabaseHandler dbHandler = new MSDatabaseHandler();
            List<Researcher> researchers = await dbHandler.GetResearchersAsync(searchKeyword);

            dgResearchersList.ItemsSource = researchers;
        }


        /*
        private void txtSearchResearcher_TextChanged(object sender, TextChangedEventArgs e) // Search researcher event handler (txtSearchResearcher)
        {
            dgResearchersList.ItemsSource = null;
            string searchKeyword = txtSearchResearcher.Text;
            MSDatabaseHandler dbHandler = new MSDatabaseHandler();
            List<Researcher> researchers = dbHandler.GetResearchers(searchKeyword);
            dgResearchersList.ItemsSource = researchers;

        }
        */

        private void btnAddUnRegResearcher_Click(object sender, RoutedEventArgs e) // Add unregistered researcher as research paper author.
        {
            if (!string.IsNullOrEmpty(txtSearchResearcher.Text))
            {
                MessageBoxResult adviserBoxResult = MessageBox.Show("The current name is going to be set as an UNREGISTERED ENTITY in the system. Do you wish to procede?", "WARNING!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (adviserBoxResult.Equals(MessageBoxResult.OK))
                {
                    // Add the researcher to the selected list
                    Researcher researcher = new Researcher
                    {
                        // Default Id for unregistered researchers is 0
                        Id = 0,
                        Name = txtSearchResearcher.Text,
                    };
                    dgResearchersSelected.Items.Add(researcher);
                }
                else
                {
                    txtSearchResearcher.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Please enter a researcher's name", "Error: No input", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e) // Add researcher to selected list
        {
            Button button = (Button)sender;
            Researcher researcher = (Researcher)button.DataContext;

            if (!ResearcherExistsInGrid(dgResearchersSelected, researcher))
            {
                dgResearchersSelected.Items.Add(researcher);
                //DEBUG
                //if (dgResearchersList.ItemsSource is ObservableCollection<Researcher> researchers)
                //{
                //    researchers.Remove(researcher);
                //}
            }
            else
            {
                MessageBox.Show($"Researcher {researcher.Name} is already selected!");
            }         
        }

        private bool ResearcherExistsInGrid(DataGrid dataGrid, Researcher researcher) // Check if researcher exists in the selected list already (dgResearchersSelected)
        {
            foreach (var item in dataGrid.Items)
            {
                if (item is Researcher existingResearcher && existingResearcher.Id == researcher.Id)
                {
                    return true;
                }
            }
            return false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // Remove researcher from selected list
        {
            Button button = (Button)sender;
            Researcher researcher = (Researcher)button.DataContext;
            // MessageBox.Show($"You have removed ID: {researcher.Id}"); // DEBUG
            dgResearchersSelected.Items.Remove(researcher);
        }

        private void dgResearchersSelected_BeginningEdit(object sender, DataGridBeginningEditEventArgs e) // Prevent editing of the selected researchers
        {
            e.Cancel = true;
        }

        private void dgResearchersList_BeginningEdit(object sender, DataGridBeginningEditEventArgs e) // Prevent editing of the researchers list
        {
            e.Cancel = true;
        }

        private void dgAdvisers_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string searchKeyword = txtSearchAdviser.Text;
            DatabaseHandler dbHandler = new DatabaseHandler();
            List<Staff> staff = dbHandler.GetStaff(searchKeyword);
            if (staff.Count > 0)
            {
                int staffId = staff[0].Id;
                string staffName = staff[0].Name;
                lblSelectedAdviser.Text = staffName;
            }
            else
            {
                MessageBox.Show("Error getting staff member's ID", "DEADLY ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
