using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

        // AUTHENTICATE
        public bool AuthenticateUser(string enteredUsername, string enteredPassword)
        {
            OpenConnection();
            string query = "SELECT acc_name, acc_pass, acc_salt FROM tblacc WHERE acc_name = @Username";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", enteredUsername);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string accountName = reader["acc_name"].ToString();
                    string accountPass = reader["acc_pass"].ToString();
                    string salt = reader["acc_salt"].ToString();
                    // Hasbrown the potato's password and add salt
                    string enteredPasswordHash = HashPassword(enteredPassword, salt);
                    // Is it the same hashbrown??
                    return accountPass == enteredPasswordHash; // OPEN SAYS ME!
                }
            }

            return false; // NONE SHALL PASS!
        }

        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                // Combine salt and password bytes
                byte[] combinedBytes = saltBytes.Concat(passwordBytes).ToArray();
                // Compute the hash
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                // Convert the hash to a hexadecimal string
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashString;
            }
        }
        public void InsertUser(string username, string password)
        {
            // Generate a random salt
            string salt = GenerateSalt();

            // Hash the password with the generated salt
            string hashedPassword = HashPassword(password, salt);

            // Insert the user data into the database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Implement your database insertion logic here
                string query = "INSERT INTO tblacc (acc_name, acc_pass, acc_salt) VALUES (@Username, @Password, @Salt)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    command.Parameters.AddWithValue("@Salt", salt);

                    command.ExecuteNonQuery();
                }
            }
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(saltBytes);
            }

            // Convert the salt to a base64-encoded string
            string salt = Convert.ToBase64String(saltBytes);

            return salt;
        }
    }
}