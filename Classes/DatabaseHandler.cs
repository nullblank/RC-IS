using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Printing.IndexedProperties;
using System.Diagnostics;

namespace RC_IS.Classes
{
    internal class DatabaseHandler : IDisposable
    {
        private readonly string connectionString;
        private MySqlConnection connection;
        public DatabaseHandler()
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["MyDatabase"].ConnectionString;
            Trace.WriteLine("DATAINFO: " + this.connectionString);
            OpenConnection();
        }

        private void OpenConnection()
        {
            if (connection == null)
            {
                connection = new MySqlConnection(connectionString);
            }

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public DataTable ExecuteQuery(string query)
        {
            OpenConnection();

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        public int ExecuteNonQuery(string query)
        {
            OpenConnection();

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                return command.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string query)
        {
            OpenConnection();

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                return command.ExecuteScalar();
            }
        }

        // Other methods for specific database operations can be added here

        public void Dispose()
        {
            CloseConnection();
            connection?.Dispose();
        }


    }
}
