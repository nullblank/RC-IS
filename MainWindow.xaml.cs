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
        public MainWindow()
        {
            InitializeComponent();
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
            string enteredUsername = txtUsername.Text;
            string enteredPassword = txtPassword.Password;
            DatabaseHandler dbHandler = new DatabaseHandler();
            if (dbHandler.AuthenticateUser(enteredUsername, enteredPassword))
            {
                MessageBox.Show("Access Granted!");
                RC_IS.Windows.Panel form = new RC_IS.Windows.Panel();
                form.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Access Denied.");
            }


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //DatabaseHandler databaseHandler = new DatabaseHandler();
                //if (databaseHandler.InsertUser("dev", "dev", "Diego Gerard A. Diego"))
                //{
                //    MessageBox.Show("SuperUser successfully Inserted.");
                //}
                
            }
            catch (MySqlException a)
            {
                MessageBox.Show("MySQL Error: " + a.Message);
            }
        }
    }
}
