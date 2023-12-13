using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Security.Cryptography;

namespace RC_IS.Classes
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public int Role { get; set; }
        public User(string username)
        {
            Username = username;
        }

        public bool InsertUser(string username, string password, string description, int role) // Refactored
        {
            try
            {
                DatabaseHandler handler = new DatabaseHandler();
                string salt = GenerateSalt();
                string hashedPassword = HashPassword(password, salt);
                string query = "INSERT INTO tblacc (acc_name, acc_pass, acc_salt, acc_desc, acc_role) VALUES (@Username, @Password, @Salt, @Desc, @Role)";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@Username", username),
                    new MySqlParameter("@Password", hashedPassword),
                    new MySqlParameter("@Salt", salt),
                    new MySqlParameter("@Desc", description),
                    new MySqlParameter("@Role", role)
                };
                //string query = "INSERT INTO tblacc (acc_name, acc_pass, acc_salt, acc_desc) VALUES (@Username, @Password, @Salt, @Desc)";
                //List<MySqlParameter> parameters = new List<MySqlParameter>
                //{
                //    new MySqlParameter("@Username", username),
                //    new MySqlParameter("@Password", hashedPassword),
                //    new MySqlParameter("@Salt", salt),
                //    new MySqlParameter("@Desc", description)
                //};
                handler.ExecuteNonQueryWithParameters(query, parameters.ToArray());
                return true;
            }
            catch (MySqlException e)
            {
                MessageBox.Show("ERROR INSERTING USER: " + e.Message);
                return false;
            }
        }

        public bool Authenticate(string password) // Refactored
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                return AuthenticateUser(Username, password, this);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error authenticating user: " + ex.Message);
                return false;
            }
        }

        public bool AuthenticateUser(string enteredUsername, string enteredPassword, User user)
        {
            try
            {
                DatabaseHandler handler = new DatabaseHandler();
                var (hashedPasswordFromDatabase, salt) = GetHashedPasswordAndSalt(enteredUsername);
                if (hashedPasswordFromDatabase != null && salt != null)
                {
                    string hashedEnteredPassword = HashPassword(enteredPassword, salt);
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

        public (int userId, string userDesc, int userRole) GetUserInfo(string enteredUsername)
        {
            try
            {
                DatabaseHandler handler = new DatabaseHandler();
                string query = "SELECT acc_id, acc_desc, acc_role FROM tblacc WHERE acc_name = @Username";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@Username", enteredUsername)
                };
                using (MySqlDataReader reader = handler.ExecuteReaderWithParameters(query, parameters.ToArray()))
                {
                    if (reader.Read())
                    {
                        int userId = reader.GetInt32("acc_id");
                        string userDesc = reader.GetString("acc_desc");
                        int userRole = reader.GetInt32("acc_role");
                        return (userId, userDesc, userRole);
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
                DatabaseHandler handler = new DatabaseHandler();
                string query = "SELECT acc_pass, acc_salt FROM tblacc WHERE acc_name = @Username";
                List<MySqlParameter> parameter = new List<MySqlParameter>
                {
                    new MySqlParameter("@Username", enteredUsername)
                };
                using (MySqlDataReader reader = handler.ExecuteReaderWithParameters(query, parameter.ToArray()))
                {
                    if (reader != null)
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
                    else
                    {
                        MessageBox.Show("Could not authenticate user: connection returns NULL!");
                        return (null, null);
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

    } // Class end
}
