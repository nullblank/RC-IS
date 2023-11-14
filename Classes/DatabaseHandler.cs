using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace RC_IS.Classes
{
    internal class DatabaseHandler : IDisposable
    {
        private readonly string connectionString;
        private MySqlConnection connection;

        public DatabaseHandler()
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["MyDatabase"].ConnectionString;
            Trace.WriteLine("DatabaseHandler Initiated");
        }
        // ------------- CONTROLS -------------
        private void OpenConnection()
        {
            if (connection == null)
            {
                Trace.WriteLine("DatabaseHandler Opened");
                connection = new MySqlConnection(connectionString);
            }

            if (connection.State != ConnectionState.Open)
            {
                Trace.WriteLine("DatabaseHandler Opened");
                connection.Open();
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

        public DataTable ExecuteQueryWithParameters(string query, params MySqlParameter[] parameters)
        {
            DataTable dataTable = new DataTable();

            try
            {
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

        public void Dispose()
        {
            CloseConnection();
            connection?.Dispose();
            Trace.WriteLine("DatabaseHandler Closed and Disposed");
        }

        // ------------- CRUD -------------
        // CREATE
        public bool InsertUser(string username, string password, string description)
        {
            try
            {
                string salt = GenerateSalt();
                string hashedPassword = HashPassword(password, salt);
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO tblacc (acc_name, acc_pass, acc_salt, acc_desc) VALUES (@Username, @Password, @Salt, @Desc)";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", hashedPassword);
                        command.Parameters.AddWithValue("@Salt", salt);
                        command.Parameters.AddWithValue("@Desc", description);

                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (MySqlException e)
            {
                MessageBox.Show("ERROR INSERTING USER: " + e.Message);
                return false;
            }
        }

        // AUTHENTICATION
        public bool AuthenticateUser(string enteredUsername, string enteredPassword, User user)
        {
            try
            {
                OpenConnection();
                var (hashedPasswordFromDatabase, salt) = GetHashedPasswordAndSalt(enteredUsername);
                if (hashedPasswordFromDatabase != null && salt != null)
                {
                    string hashedEnteredPassword = HashPassword(enteredPassword, salt);
                    //MessageBox.Show(hashedEnteredPassword + ":" + hashedPasswordFromDatabase);
                    bool passwordMatches = hashedEnteredPassword == hashedPasswordFromDatabase;
                    if (passwordMatches)
                    {
                        MessageBox.Show("Login successful!");
                        var (userId, userDesc, userRole) = GetUserInfo(enteredUsername);
                        user.Username = enteredUsername;
                        user.Description = userDesc;
                        user.UserId = userId;
                        user.Role = userRole;
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Incorrect password.");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("User was not found.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error authenticating user: " + ex.Message);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        public (int userId, string userDesc, int userRole) GetUserInfo(string enteredUsername)
        {
            try
            {
                OpenConnection();
                string query = "SELECT acc_id, acc_desc, acc_role FROM tblacc WHERE acc_name = @Username";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", enteredUsername);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32("acc_id");
                            string userDesc = reader.GetString("acc_desc");
                            int userRole = reader.GetInt32("acc_role");
                            return (userId, userDesc, userRole);
                        }
                    }
                }
                return (0, null, 0);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error retrieving user info: " + ex.Message);
                return (0, null, 0);
            }
        }

        public (string hashedPassword, string salt) GetHashedPasswordAndSalt(string enteredUsername)
        {
            try
            {
                OpenConnection();

                string query = "SELECT acc_pass, acc_salt FROM tblacc WHERE acc_name = @Username";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", enteredUsername);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            byte[] accountHashedPassBytes = (byte[])reader["acc_pass"];
                            string accountHashedPass = Encoding.UTF8.GetString(accountHashedPassBytes);

                            byte[] accountSaltBytes = (byte[])reader["acc_salt"];
                            string salt = Encoding.UTF8.GetString(accountSaltBytes);

                            return (accountHashedPass, salt);
                        }
                    }
                }
                return (null, null);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error retrieving hashed password and salt: " + ex.Message);
                return (null, null);
            }
        }

        // ------------------ HELPER METHODS ------------------
        internal List<Schools> GetSchoolData()
        {
            try
            {
                OpenConnection();
                List<Schools> list = new List<Schools> ();
                string query = "SELECT * FROM tblschool";
                DataTable dt = ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    Schools schools = new Schools
                    {
                        Id = Convert.ToInt32(row["school_id"]),
                        Desc = row["description"].ToString(),
                    };
                    list.Add(schools);
                }
                return list;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine("ERROR GETTING SCHOOLDATA");
                return null;
            }
            finally
            {
                this.Dispose();
            }
            
        }

        internal List<Programs> GetProgramData(int schoolId)
        {
            try
            {
                OpenConnection();
                List<Programs> list = new List<Programs>();
                string query = "SELECT * FROM tblprograms WHERE school_id = @SchoolID";
                MySqlParameter parameter = new MySqlParameter("@SchoolID", schoolId);
                DataTable dt = ExecuteQueryWithParameters(query, parameter);
                foreach (DataRow row in dt.Rows)
                {
                    Programs programs = new Programs
                    {
                        Id = Convert.ToInt32(row["program_id"]),
                        Desc = row["description"].ToString(),
                    };
                    list.Add(programs);
                }
                return list;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine("ERROR GETTING SCHOOLDATA");
                return null;
            }
            finally
            {
                this.Dispose();
            }
        }
        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] combinedBytes = saltBytes.Concat(passwordBytes).ToArray();
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);

                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashString;
            }
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            string salt = Convert.ToBase64String(saltBytes);
            return salt;
        }

        internal void LogUserAction(User user, string action, string table, string row_id, string column)
        {
            try
            {
                OpenConnection();
                string query = "INSERT INTO tblaudit (acc_id, action, `table`, row_id, `column`) " +
                               "VALUES (@accountId, @action, @table, @rowId, @column)";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@accountId", user.UserId);
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@table", table);
                    cmd.Parameters.AddWithValue("@rowId", row_id);
                    cmd.Parameters.AddWithValue("@column", column);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                Trace.WriteLine("Error logging user action!: " + ex.Message);
            }
        }

        
    }
}