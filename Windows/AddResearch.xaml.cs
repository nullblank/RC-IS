using Microsoft.Win32;
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
using System.IO; // Added for file handling

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
            originalWindowState = this.WindowState;
            _user = user;
            LoadSchoolData();
            LoadAgendaData();
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

        private bool StaffExistsInGrid(DataGrid dataGrid, Staff staff) // Check if researcher exists in the selected list already (dgResearchersSelected)
        {
            foreach (var item in dataGrid.Items)
            {
                if (item is Staff existingResearcher && existingResearcher.Id == staff.Id)
                {
                    return true;
                }
            }
            return false;
        }

        private void LoadSchoolData() // Load school data from database to combobox (txtSchool)
        {
            txtSchool.ItemsSource = null;
            DatabaseHandler dbHandler = new DatabaseHandler();
            List<Schools> schools = dbHandler.GetSchoolData();
            txtSchool.ItemsSource = schools;
        }

        private void LoadAgendaData() // Load agenda data from database to combobox (txtAgenda)
        {
            txtAgenda.ItemsSource = null;
            DatabaseHandler dbHandler = new DatabaseHandler();
            List<Agenda> agendas = dbHandler.GetAgendaData();
            if (agendas != null && agendas.Any())
            {
                txtAgenda.ItemsSource = agendas;
            }
            else
            {
                Trace.WriteLine("No agenda data retrieved from the database.");
            }
        }

        private void LoadProgramData(int schoolId) // Load program data from database to combobox (txtCourse)
        {
            txtCourse.ItemsSource = null;
            DatabaseHandler dbHandler = new DatabaseHandler();
            List<Programs> programs = dbHandler.GetProgramData(schoolId);
            txtCourse.ItemsSource = programs;
        }


        // --------------- Event Handlers ---------------
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

        // txtSearchResearcher_TextChanged
        private async void txtSearchResearcher_TextChanged(object sender, TextChangedEventArgs e) // Search researcher event handler (txtSearchResearcher) (async) (await)
        { 
            dgResearchersList.ItemsSource = null;
            string searchKeyword = txtSearchResearcher.Text;
            MSDatabaseHandler dbHandler = new MSDatabaseHandler();
            switch (txtResearcherType.SelectedIndex)
            {
                case 0:
                    List<Researcher> researchers = await dbHandler.GetResearchersAsync(searchKeyword);
                    Trace.WriteLine($"Researcher count: {researchers.Count}");
                    dgResearchersList.ItemsSource = researchers;
                    break;
                case 1:
                    List<Staff> staff = await dbHandler.GetStaffAsync(searchKeyword);
                    Trace.WriteLine($"Staff count: {staff.Count}");
                    dgResearchersList.ItemsSource = staff;
                    break;
                default:
                    break;
            }



            
            

            
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
        private void Button_Click(object sender, RoutedEventArgs e) // Add authors to selected list
        {
            Button button = (Button)sender;
            if (button.DataContext is Researcher researcher)
            {
                if (!ResearcherExistsInGrid(dgResearchersSelected, researcher)) // Check if researcher exists in the selected list already (dgResearchersSelected)
                {
                    Trace.WriteLine($"Added Student: ID[{researcher.Id}], NAME[{researcher.Name}]");
                    dgResearchersSelected.Items.Add(researcher);
                    Trace.WriteLine($"DataContext is a Student: ID[{researcher.Id}], NAME[{researcher.Name}]");
                }
                else
                {
                    MessageBox.Show($"Researcher {researcher.Name} is already selected!");
                }
            }
            else if (button.DataContext is Staff staff)
            {
                if (!StaffExistsInGrid(dgResearchersSelected, staff)) // Check if researcher exists in the selected list already (dgResearchersSelected)
                {
                    Trace.WriteLine($"Added Staff: ID[{staff.Id}], NAME[{staff.Name}]");
                    dgResearchersSelected.Items.Add(staff);
                    Trace.WriteLine($"DataContext is a Staff: ID[{staff.Id}], NAME[{staff.Name}]");
                }
                else
                {
                    MessageBox.Show($"Staff {staff.Name} is already selected!");
                }
            }
            else
            {
                Trace.WriteLine($"DataContext is an unknown constructor");
            }    
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // Remove authors from selected list
        {
            Button button = (Button)sender;
            if (button.DataContext is Researcher researcher)
            {
                if (ResearcherExistsInGrid(dgResearchersSelected, researcher)) // Check if researcher exists in the selected list already (dgResearchersSelected)
                {
                    Trace.WriteLine($"Removed Student: ID[{researcher.Id}], NAME[{researcher.Name}]");
                    dgResearchersSelected.Items.Remove(researcher);
                    Trace.WriteLine($"DataContext is a Student: ID[{researcher.Id}], NAME[{researcher.Name}]");
                }
                else
                {
                    MessageBox.Show($"Researcher {researcher.Name} is not selected!");
                }
            }
            else if (button.DataContext is Staff staff)
            {
                if (StaffExistsInGrid(dgResearchersSelected, staff)) // Check if researcher exists in the selected list already (dgResearchersSelected)
                {
                    Trace.WriteLine($"Removed Staff: ID[{staff.Id}], NAME[{staff.Name}]");
                    dgResearchersSelected.Items.Remove(staff);
                    Trace.WriteLine($"DataContext is a Staff: ID[{staff.Id}], NAME[{staff.Name}]");
                }
                else
                {
                    MessageBox.Show($"Staff {staff.Name} is not selected!");
                }
            }
            else
            {
                Trace.WriteLine($"DataContext is an unknown constructor");
            }
        }

        private Staff _staff;
        private async void Button_Click_2(object sender, RoutedEventArgs e) // Set adviser
        {
            Button button = (Button)sender;
            _staff = (Staff)button.DataContext;
            Trace.WriteLine($"Selected Adviser: ID[{_staff.Id}], NAME[{_staff.Name}]");
            lblSelectedAdviser.Text = _staff.Name;
        }

        private async void txtSearchAdviser_TextChanged(object sender, TextChangedEventArgs e) // Search adviser event handler (txtSearchAdviser) (async) (await)
        {
            dgAdvisers.ItemsSource = null;
            string searchKeyword = txtSearchAdviser.Text;

            MSDatabaseHandler dbHandler = new MSDatabaseHandler();
            List<Staff> staff = await dbHandler.GetStaffAsync(searchKeyword);

            dgAdvisers.ItemsSource = staff;
        }

        private void txtSchool_SelectionChanged(object sender, SelectionChangedEventArgs e) // When a school is selected, load the programs of that school
        {
            Schools selectedData = (Schools)txtSchool.SelectedItem;
            int selectedId = selectedData.Id;
            Trace.WriteLine("SELECTED SCHOOL ID: " + selectedId.ToString());
            LoadProgramData(selectedId);
        }

        private void txtAgenda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtAgenda.SelectedItem != null && txtAgenda.SelectedItem is Agenda selectedData)
            {
                int selectedId = selectedData.Id;
                Trace.WriteLine("SELECTED Agenda ID: " + selectedId.ToString());
            }
            else
            {
                Trace.WriteLine("No item selected or selected item is not of type Agenda.");
            }
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

        private void Button_Click_3(object sender, RoutedEventArgs e) // Add panelist to selected list
        {
            Button button = (Button)sender;
            Staff staff = (Staff)button.DataContext;

            if (!StaffExistsInGrid(dgPanelistSelected, staff)) // Check if staff exists in the selected list already (dgPanelistSelected)
            {
                Trace.WriteLine($"Added Panelist: ID[{staff.Id}], NAME[{staff.Name}]");
                dgPanelistSelected.Items.Add(staff);
            }
            else
            {
                MessageBox.Show($"Panelist {staff.Name} is already selected!");
            }
        }

        private async void txtSearchPanelist_TextChanged(object sender, TextChangedEventArgs e) // Search panelist event handler
        {
            dgPanelist.ItemsSource = null;
            string searchKeyword = txtSearchPanelist.Text;

            MSDatabaseHandler dbHandler = new MSDatabaseHandler();
            List<Staff> staff = await dbHandler.GetStaffAsync(searchKeyword);

            dgPanelist.ItemsSource = staff;
        }

        private void dgPanelist_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void dgPanelistSelected_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e) // Remove panelist from selected list
        {
            Button button = (Button)sender;
            Staff staff = (Staff)button.DataContext;
            Trace.WriteLine($"Removed Student: ID[{staff.Id}], NAME[{staff.Name}]");
            dgPanelistSelected.Items.Remove(staff);
        }

        private void btnBrowseFiles_Click(object sender, RoutedEventArgs e) // Browse files event handler (btnBrowseFiles)
        {
            List<ResearchFiles> list  = new List<ResearchFiles>();
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "PDF Files (*.pdf)|*.pdf|Word Files (*.docx)|*.docx|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filepath in openFileDialog.FileNames)
                {
                    ResearchFiles files = new ResearchFiles
                    {
                        FileName = System.IO.Path.GetFileName(filepath),
                        FilePath = filepath,
                    };
                    list.Add(files);
                }
                dgFilesSelected.ItemsSource = list;
            }
            
        }

        private void dgFilesSelected_BeginningEdit(object sender, DataGridBeginningEditEventArgs e) // Prevent editing of the selected files
        {
            e.Cancel = true;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e) // Remove file from selected list (dgFilesSelected)
        {
            Button button = (Button)sender;
            ResearchFiles files = (ResearchFiles)button.DataContext;

            // Remove the item from the underlying data source
            List<ResearchFiles> list = (List<ResearchFiles>)dgFilesSelected.ItemsSource;
            list.Remove(files);

            // Update the DataGrid by resetting the ItemsSource
            dgFilesSelected.ItemsSource = null;
            dgFilesSelected.ItemsSource = list;
        }

        private void btnDebugUpload_Click(object sender, RoutedEventArgs e) // Debug button for uploading files to database
        {
            if (dgFilesSelected.Items.Count > 0)
            {
                List<ResearchFiles> list = (List<ResearchFiles>)dgFilesSelected.ItemsSource;
                DatabaseHandler dbHandler = new DatabaseHandler();
                dbHandler.StoreDocument(list);
            }
            else
            {
                Trace.WriteLine("NO FILES SELECTED");
            }   
        }


        private void btnSubmit_Click(object sender, RoutedEventArgs e) // Submits the entire list as a new research paper document
        {
            List<TextBox> textBoxes = new List<TextBox>
            {
                txtTitle, txtYear, txtSchool, txtCourse, txtAgenda
            };

            List<DataGrid> dataGrids = new List<DataGrid>
            {
                dgPanelistSelected, dgResearchersSelected, dgFilesSelected
            };

            if (emptyControls.Count > 0)
            {
                // Handle the empty controls (e.g., show a message)
                string errorMessage = "The following controls are empty:\n" + string.Join("\n", emptyControls);
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ValidationHelper validator = new ValidationHelper();
                List<string> emptyControls = validator.GetEmptyControls(textBoxes, dataGrids);

                Schools selectedSchool = (Schools)txtSchool.SelectedItem;
                Programs selectedProgram = (Programs)txtCourse.SelectedItem;
                Agenda selectedAgenda = (Agenda)txtAgenda.SelectedItem;
                Papers paper = new Papers
                {
                    Title = txtTitle.Text,
                    Year = Int32.Parse(txtYear.Text),
                    SchoolID = selectedSchool.Id,
                    ProgramID = selectedProgram.Id,
                    AgendaID = selectedAgenda.Id,
                    AdviserID = _staff.Id,
                    Authors = (List<Researcher>)dgResearchersSelected.ItemsSource,
                    Panelist = (List<Staff>)dgPanelistSelected.ItemsSource,
                    Files = (List<ResearchFiles>)dgFilesSelected.ItemsSource,
                };
                DatabaseHandler dbHandler = new DatabaseHandler();
                dbHandler.InsertPaper(paper);
                dbHandler.InsertAdviser(paper);
                dbHandler.InsertAuthors(paper);
                dbHandler.InsertPanelist(paper);
            }

            
        }

        
    }
}
