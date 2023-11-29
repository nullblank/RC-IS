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
    /// Interaction logic for ViewResearch.xaml
    /// </summary>
    public partial class ViewResearch : Window
    {
        private WindowState originalWindowState;
        private Dashboard _form;
        private Papers _papers;
        public ViewResearch(Papers papers)
        {
            if (papers != null)
            {
                InitializeComponent();
                _papers = papers;
                SetData();
            }
            else
            {
                MessageBox.Show("ILLEGAL ACTION! Paper constructor is either fabricated or not complete!");
                this.Close();
            }
        }

        public ViewResearch(Papers papers, Dashboard form)
        {
            if (papers != null)
            {
                InitializeComponent();
                _papers = papers;
                _form = form;
                SetData();
            }
            else
            {
                MessageBox.Show("ILLEGAL ACTION! Paper constructor is either fabricated or not complete!");
                this.Close();
            }
        }

        private void SetData()
        {
            lblTitle.Text = _papers.Title;
            lblPaperId.Content = _papers.Id.ToString();
            lblYear.Content = ParseYear(_papers.Year);
            lblAdviser.Content = SetAdviser(_papers.Id);
            SetAuthors();
            SetPanelists();
            SetFiles();
        }

        private void SetFiles()
        {
            dgFiles.ItemsSource = _papers.Files;
        }

        private string SetAdviser(int id)
        {
            Staff staff  = new Staff();
            staff = staff.GetAdviser(id);
            _papers.AdviserName = staff.Name;
            return staff.Name;
        }
        private void SetPanelists()
        {
            dgPanelists.ItemsSource = _papers.Panelist;
        }

        private void SetAuthors()
        {
            dgAuthors.ItemsSource = _papers.Authors;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private string ParseYear(int year)
        {
            string yearString = year.ToString();
            if (yearString.Length == 8)
            {
                string yearParsed = $"{yearString.Substring(0, 4)}-{yearString.Substring(4, 4)}";
                return yearParsed;
            }
            else
            {
                return "InvalidYear";
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        private void ToggleMaximize()
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (_form != null)
            {
                _form.LoadPapers();
            }
            this.Close();
        }

        private void dgAuthors_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void dgPanelists_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e) // TODO: Download file
        {
            ResearchFiles file = (ResearchFiles)dgFiles.SelectedItem;
            if (file != null)
            {
                file.DownloadDocument(_papers, file.FileName);
            }
            else
            {
                MessageBox.Show("Please select a file to download!");
            }
        }

        private void btnCloser_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            AddResearch addResearch = new AddResearch(_papers);
            addResearch.Show();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_form != null)
                _form.LoadPapers();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_form != null)
                _form.LoadPapers();
        }
    }
}
