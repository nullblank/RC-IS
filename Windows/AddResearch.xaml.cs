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
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;

namespace RC_IS.Windows
{
    /// <summary>
    /// Interaction logic for AddResearch.xaml
    /// </summary>
    public partial class AddResearch : Window
    {
        private WindowState originalWindowState;
        private User _user;
        private Staff _staff = new Staff();
        private Window _form;
        private bool isEdit;
        private Papers _paper;

        public AddResearch(User user, Window form) // Constructor for AddResearch window (called from MainWindow) Add
        {
            InitializeComponent();
            originalWindowState = this.WindowState;
            _user = user;
            LoadSchoolData();
            LoadAgendaData();
            isEdit = false;
            _form = form;
            lblHeader.Text = "ADD NEW RESEARCH PAPER";
        }
        public AddResearch(Papers papers, ViewResearch form) // Constructor for AddResearch window (called from MainWindow) Edit
        {
            InitializeComponent();
            originalWindowState = this.WindowState;
            _paper = papers;
            _form = form;
            PrepareForEdit();
            lblHeader.Text = $"EDITING RESEARCH PAPER: {_paper.Id}";
        }

        private void PrepareForEdit()
        {
            LoadSchoolData();
            LoadAgendaData();
            isEdit = true;
            if (isEdit)
            {
                SelectItemById<Schools>(_paper.SchoolID, txtSchool);
                SelectItemById<Agenda>(_paper.AgendaID, txtAgenda);
                SelectItemById<Programs>(_paper.ProgramID, txtCourse);
                txtTitle.Text = _paper.Title;
                txtYear.Text = _paper.Year.ToString();
                _staff.Id = _paper.AdviserID;
                _staff.Name = _paper.AdviserName;
                lblSelectedAdviser.Text = _staff.Name;
                PaperToDataGrid();
            }
        }

        private void PaperToDataGrid()
        {
            if (_paper != null)
            {
                foreach (object author in _paper.Authors)
                {
                    dgResearchersSelected.Items.Add(author);
                }
                foreach (object panelist in _paper.Panelist)
                {
                    dgPanelistSelected.Items.Add(panelist);
                }
                foreach (object file in _paper.Files) 
                {
                    dgFilesSelected.Items.Add(file);
                }
            }

        }

        private void SelectItemById<T>(int targetId, ComboBox comboBox)
        {
            T selectedItem = comboBox.Items.OfType<T>().FirstOrDefault(item => GetId(item) == targetId);
            if (selectedItem != null)
            {
                comboBox.SelectedItem = selectedItem;
            }
            else
            {
                MessageBox.Show("The item with the matching Id was not found in the ComboBox items source.");
            }
        }
        private int GetId<T>(T item)
        {
            var property = item.GetType().GetProperty("Id");
            return property != null ? (int)property.GetValue(item) : 0; // Assuming Id is of type int
        }








        private void LoadSchoolData() // Load school data from database to combobox (txtSchool)
        {
            
            txtSchool.ItemsSource = null;
            Schools school = new Schools();
            txtSchool.ItemsSource = school.GetSchoolData();
        }

        private void LoadAgendaData() // Load agenda data from database to combobox (txtAgenda)
        {
            txtAgenda.ItemsSource = null;
            Agenda agenda = new Agenda();
            List<Agenda> agendas = agenda.GetAgendaData();
            if (agendas != null && agendas.Any())
            {
                txtAgenda.ItemsSource = agendas;
            }
            else
            {
                Trace.WriteLine("No agenda data retrieved from the database.");
            }
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

        

        private void LoadProgramData(int schoolId) // Load program data from database to combobox (txtCourse)
        {
            txtCourse.ItemsSource = null;
            Programs program = new Programs();
            List<Programs> programs = program.GetProgramData(schoolId);
            txtCourse.ItemsSource = programs;
        }


        // --------------- Event Handlers ---------------
        private void btnClose_Click(object sender, RoutedEventArgs e) // Close window button event handler (btnClose)
        {
            if (isEdit)
            {
                _form = new ViewResearch(_paper);
            }
            _form.Show();
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

        private string GetNumericValueWithoutHyphen(string formattedText)
        {
            return new string(formattedText.Where(char.IsDigit).ToArray());
        }

        

        private Papers CreatePaperObject()
        {
            int year = ParseYear(txtYear.Text);
            Schools selectedSchool = (Schools)txtSchool.SelectedItem;
            Programs selectedProgram = (Programs)txtCourse.SelectedItem;
            Agenda selectedAgenda = (Agenda)txtAgenda.SelectedItem;

            List<object> authors = dgResearchersSelected.Items.Cast<object>().ToList();
            List<Staff> panelists = dgPanelistSelected.Items.Cast<Staff>().ToList();
            List<ResearchFiles> files = dgFilesSelected.Items.Cast<ResearchFiles>().ToList();

            return new Papers
            {
                Title = txtTitle.Text,
                Year = year,
                AdviserName = _staff.Name,
                SchoolID = selectedSchool?.Id ?? 0, // Use null-conditional operator to handle possible null
                ProgramID = selectedProgram?.Id ?? 0,
                AgendaID = selectedAgenda?.Id ?? 0,
                AdviserID = _staff?.Id ?? 0,
                Authors = authors,
                Panelist = panelists,
                Files = files,
            };
        }

        private int ParseYear(string yearText)
        {
            string numericValue = GetNumericValueWithoutHyphen(yearText);
            return int.Parse(numericValue);
        }

        private void InsertPaperToDatabase(Papers paper)
        {
            Papers papers = new Papers();
            Staff staff = new Staff();
            Authors author = new Authors();
            ResearchFiles files = new ResearchFiles();

            paper.Id = papers.InsertPaper(paper);
            if (paper.Id > 0)
            {
                staff.InsertAdviser(paper);
                author.InsertAuthors(paper);
                staff.InsertPanelist(paper);
                files.InsertDocuments(paper);
                MessageBox.Show($"Successfully inserted paper with id of [ID]{paper.Id} to database!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _form.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to insert paper to database!", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnSubmit_Click(object sender, RoutedEventArgs e) // Submits the entire list as a new research paper document
        {

            try
            {
                if (isEdit)
                {
                    if (ValidateControls())
                    {
                        ValdatePapers();
                        Papers papers = new Papers();
                        Staff staff = new Staff();
                        Authors author = new Authors();
                        ResearchFiles files = new ResearchFiles();
                        papers.UpdatePaper(_paper);
                        staff.UpdateAdviser(_paper);
                        staff.UpdatePanelists(_paper);
                        author.UpdateAuthors(_paper);
                        files.UpdateDocuments(_paper);
                        MessageBox.Show($"Successfully updated paper with id of [ID]{_paper.Id} to database!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        ViewResearch form = new ViewResearch(_paper);
                        form.Show();
                        this.Close();
                    }
                }
                else
                {
                    if (ValidateControls())
                    {
                        Papers paper = CreatePaperObject();
                        InsertPaperToDatabase(paper);
                        Trace.WriteLine("Validation returns clear!");
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ValdatePapers()
        {
            Papers paper = CreatePaperObject();
            if (_paper.Title != paper.Title)
            {
                _paper.Title = paper.Title;
            }
            if (_paper.Year != paper.Year)
            {
                _paper.Year = paper.Year;
            }
            if (_paper.SchoolID != paper.SchoolID)
            {
                _paper.SchoolID = paper.SchoolID;
            }
            if (_paper.ProgramID != paper.ProgramID)
            {
                _paper.ProgramID = paper.ProgramID;
            }
            if (_paper.AgendaID != paper.AgendaID)
            {
                _paper.AgendaID = paper.AgendaID;
            }
            if (_paper.AdviserID != paper.AdviserID)
            {
                _paper.AdviserID = paper.AdviserID;
            }
            if (_paper.AdviserName != paper.AdviserName)
            {
                _paper.AdviserName = paper.AdviserName;
            }
            if (_paper.Authors != paper.Authors)
            {
                _paper.Authors = paper.Authors;
            }
            if (_paper.Panelist != paper.Panelist)
            {
                _paper.Panelist = paper.Panelist;
            }
        }

        private bool ValidateControls()
        {
            List<ControlInfo> allControls = new List<ControlInfo> // List of all controls that need to be validated
            {
                new ControlInfo { Name = "Title is empty", Control = txtTitle },
                new ControlInfo { Name = "Year is empty", Control = txtYear },
                new ControlInfo { Name = "No school selected", Control = txtSchool },
                new ControlInfo { Name = "No course/program selected", Control = txtCourse },
                new ControlInfo { Name = "No agenda selected", Control = txtAgenda },
                new ControlInfo { Name = "No panelists selected", Control = dgPanelistSelected },
                new ControlInfo { Name = "No authors selected", Control = dgResearchersSelected },
                new ControlInfo { Name = "No files seleected", Control = dgFilesSelected },
            };

            ValidationHelper validator = new ValidationHelper();
            List<ControlInfo> emptyControls = validator.GetEmptyControls(allControls);

            if (emptyControls.Count > 0)
            {
                string errorMessage = "The following need attention:\n" +
                    string.Join("\n", emptyControls.Select(info => info.Name));

                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void Year_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, "[0-9]"))
            {
                e.Handled = true;
            }
        }

        private void Year_TextChanged(object sender, TextChangedEventArgs e)
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

