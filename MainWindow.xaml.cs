using MySql.Data.MySqlClient;
using RC_IS.Classes;
using RC_IS.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RC_IS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private LoadingForm loadingForm;
        public MainWindow()
        {
            InitializeComponent();
            MessageBox.Show("Please wait...");
            try
            {
                //loadingForm = new LoadingForm();
                //loadingForm.Show();
                ValidateDatabases();
            }
            finally
            {
                //loadingForm.Close();
            }
        }

        protected void ValidateDatabases()
        {
            DatabaseHandler dbHandler = new DatabaseHandler();
            if (!dbHandler.ValidateLocalDatabases())
            {
                MessageBox.Show("Cannot connect to local database. Please contact your system administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Close
            }

            MSDatabaseHandler msDatabaseHandler = new MSDatabaseHandler();
            if (!msDatabaseHandler.ValidateLocalDatabases())
            {
                MessageBox.Show("Cannot connect to reference database. Please contact your system administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            User user = new User(txtUsername.Text);
            if (user.Authenticate(txtPassword.Password))
            {
                Audit audit = new Audit(user);
                // Add login logging here
                RC_IS.Windows.Panel panel = new RC_IS.Windows.Panel(user);
                panel.Show();
                this.Close();
            }

        }
    }
}
