using RC_IS.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            LoadSchool();
            LoadAgenda();
        }

        internal void LoadPapers()
        {
            dgPapers.ItemsSource = null;
            Papers paper = new Papers();
            List<Papers> papers = paper.GetPapers();
            dgPapers.ItemsSource = papers;
        }

        private void LoadSchool() // Load school data from database to combobox (txtSchool)
        {
            txtSchool.ItemsSource = null;
            Schools school = new Schools();
            List<Schools> schools = school.GetSchoolData();
            txtSchool.ItemsSource = schools;
            txtSchool.SelectedIndex = 0;
        }

        private void LoadAgenda() // Load agenda data from database to combobox (txtAgenda)
        {
            txtAgenda.ItemsSource = null;
            Agenda agenda = new Agenda();
            List<Agenda> agendas = agenda.GetAgendaData();
            if (agendas != null && agendas.Any())
            {
                txtAgenda.ItemsSource = agendas;
                txtAgenda.SelectedIndex = 0;
            }
            else
            {
                Trace.WriteLine("No agenda data retrieved from the database.");
            }
        }

        private void LoadProgram(int schoolId) // Load program data from database to combobox (txtCourse)
        {
            txtProgram.ItemsSource = null;
            Programs program = new Programs();
            List<Programs> programs = program.GetProgramData(schoolId);
            txtProgram.ItemsSource = programs;
            txtProgram.SelectedIndex = 0;
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
            loadPapers();
        }

        public void loadPapers()
        {
            dgPapers.ItemsSource = null;
            string title = txtSearch.Text;
            string query = ConstructQuery();
            int year = GetNumericValueWithoutHyphen(txtYear.Text) == "" ? 0 : int.Parse(GetNumericValueWithoutHyphen(txtYear.Text));
            int schoolId = txtSchool.SelectedItem == null ? 0 : ((Schools)txtSchool.SelectedItem).Id;
            int programId = txtProgram.SelectedItem == null ? 0 : ((Programs)txtProgram.SelectedItem).Id;
            int agendaId = txtAgenda.SelectedItem == null ? 0 : ((Agenda)txtAgenda.SelectedItem).Id;
            Papers paper = new Papers();
            List<Papers> papers = paper.GetPapers(query, title, year, schoolId, programId, agendaId);
            dgPapers.ItemsSource = papers;
        }

        private string GetNumericValueWithoutHyphen(string formattedText)
        {
            return new string(formattedText.Where(char.IsDigit).ToArray());
        }

        private string ConstructQuery()
        {
            string query = "SELECT * FROM tblpapers WHERE LOWER(paper_title) LIKE LOWER(@PaperTitle)";
            if (!string.IsNullOrEmpty(txtYear.Text))
            {
                query += " AND paper_year LIKE @PaperYear";
            }
            if (txtSchool.SelectedIndex != 0)
            {
                query += " AND school_id = @PaperSchool";
            }
            if (txtProgram.SelectedIndex != 0)
            {
                query += " AND program_id = @PaperProgram";
            }
            if (txtAgenda.SelectedIndex != 0)
            {
                query += " AND agenda_id = @PaperAgenda";
            }
            return query;
        }

        private void btnAddResearches_Click(object sender, RoutedEventArgs e)
        {
            AddResearch form = new AddResearch(_user, this);
            form.Show();
            this.Hide();
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

        private void txtSchool_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Schools selectedData = (Schools)txtSchool.SelectedItem;
            int selectedId = selectedData.Id;
            Trace.WriteLine("SELECTED SCHOOL ID: " + selectedId.ToString());
            LoadProgram(selectedId);
        }

        private void txtProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Programs selectedData = (Programs)txtProgram.SelectedItem;
            if (selectedData != null)
            {
                int selectedId = selectedData.Id;
                Trace.WriteLine("SELECTED COURSE ID: " + selectedId.ToString());
            }
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

        private void Button_Click_1(object sender, RoutedEventArgs e) // View button
        {
            Button button = (Button)sender;
            Papers selectedPaper = (Papers)button.DataContext;
            Staff staff = new Staff();
            Authors authors = new Authors();
            ResearchFiles files = new ResearchFiles();

            Papers toEdit = new Papers
            {
                Id = selectedPaper.Id,                              //Label
                Title = selectedPaper.Title,                        //Label
                Year = selectedPaper.Year,                          //Label
                AdviserID = staff.GetAdviserId(selectedPaper.Id),
                SchoolID = selectedPaper.SchoolID,                  
                ProgramID = selectedPaper.ProgramID,                
                AgendaID = selectedPaper.AgendaID,
                Authors = authors.GetAuthors(selectedPaper.Id),     //ComboBox
                Panelist = staff.GetPanelists(selectedPaper.Id),    //ComboBox
                Files = files.GetFiles(selectedPaper.Id),           //ComboBox
            };
            
            ViewResearch form = new ViewResearch(toEdit, this); // Pass the selected paper to the form
            form.Show();
        }
    }
}
