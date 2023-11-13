﻿using MySql.Data.MySqlClient;
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
            User user  = new User(txtUsername.Text);
            if (user.Authenticate(txtPassword.Password))
            {
                Audit audit = new Audit(user);
                audit.Log(user.Description + " has logged in!", null, null, null);
                RC_IS.Windows.Panel panel = new RC_IS.Windows.Panel(user);
                panel.Show();
                this.Close();
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                //DEBUGGING PURPOSES ONLY

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
