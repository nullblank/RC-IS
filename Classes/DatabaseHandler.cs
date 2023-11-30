using MySql.Data.MySqlClient;
using RC_IS.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace RC_IS.Classes
{
    internal class DatabaseHandler : IDisposable
    {
        private readonly string connectionString;
        private MySqlConnection connection;
        public DatabaseHandler()
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["DbURC"].ConnectionString;
            Trace.WriteLine("DatabaseHandler Initiated");
        }
        private void OpenConnection()
        {
            if (connection == null)
            {
                connection = new MySqlConnection(connectionString);
                Trace.WriteLine("DatabaseHandler Opened");
            }

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                Trace.WriteLine("DatabaseHandler Opened");
            }
        }
        private void CloseConnection()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                Trace.WriteLine("DatabaseHandler Closed");
            }
        }
        public void Dispose()
        {
            CloseConnection();
            connection?.Dispose();
            Trace.WriteLine("DatabaseHandler Closed and Disposed");
        }

        public DataTable ExecuteQuery(string query)
        {
            try
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
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error executing query: {e.Message}");
                return null;
            }
        }

        public DataTable ExecuteQueryWithParameters(string query, params MySqlParameter[] parameters)
        {
            DataTable dataTable = new DataTable();
            try
            {
                OpenConnection();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error executing query: " + ex.Message);
            }
            return dataTable;
        }

        public int ExecuteNonQuery(string query)
        {
            try
            {
                OpenConnection();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                Trace.WriteLine($"Error executing non query: {query}");
                return -1;
            }
        }

        public int ExecuteNonQueryWithParameters(string query, params MySqlParameter[] parameters)
        {
            try
            {
                OpenConnection();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters);
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                Trace.WriteLine($"Error executing non query: {query}");
                return -1;
            }
        }

        internal MySqlDataReader ExecuteReaderWithParameters(string query, MySqlParameter[] parameter)
        {
            try
            {
                OpenConnection();
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddRange(parameter);
                MySqlDataReader reader = command.ExecuteReader();
                return reader;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error executing reader: {e.Message}");
                return null;
            }
        }

        public object ExecuteScalar(string query)
        {
            try
            {
                OpenConnection();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    return command.ExecuteScalar();
                }
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error executing scalar: {e.Message}");
                return null;
            }
        }

        private object ExecuteScalar(string query, MySqlParameter[] mySqlParameters)
        {
            try
            {
                OpenConnection();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddRange(mySqlParameters);
                    return command.ExecuteScalar();
                }
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error executing scalar: {e.Message}");
                return null;
            }
        }

        internal T ExecuteScalar<T>(T query, MySqlParameter parameter)
        {
            try
            {
                OpenConnection();
                using (MySqlCommand command = new MySqlCommand(query.ToString(), connection))
                {
                    command.Parameters.Add(parameter);
                    return (T)command.ExecuteScalar();
                }
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error executing scalar: {e.Message}");
                return default(T);
            }
        }

        internal List<int> GetYear()
        {
            List<int> years = new List<int>();
            try
            {
                OpenConnection();
                string query = "SELECT DISTINCT paper_year FROM tblpapers ORDER BY paper_year DESC";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            years.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error executing scalar: {e.Message}");
            }
            return years;
        }

        internal string GetPaperCount(int year, int schoolId)
        {
            string count = "";
            try
            {
                OpenConnection();
                string query = "SELECT COUNT(*) FROM tblpapers WHERE paper_year = @year AND school_id = @schoolId ORDER BY school_id DESC LIMIT 5";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@year", year),
                    new MySqlParameter("@schoolId", schoolId)
                };
                count = ExecuteScalar(query, parameters.ToArray()).ToString();
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error executing scalar: {e.Message}");
            }
            return count;
        }

        internal bool ValidateLocalDatabases()
        {
            try
            {
                OpenConnection();
                string query = "SELECT COUNT(*) FROM tblacc";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.ExecuteScalar();
                }
                return true;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error executing scalar: {e.Message}");
                return false;
            }
        }
    }
}