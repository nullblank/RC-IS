using MySql.Data.MySqlClient;
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
        // 
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
            finally
            {
                this.Dispose();
            }
        }
        // READ
        internal List<Staff> GetStaff(string searchKeyword) // REPLACE WITH GET STAFF <-- Current function is for testing purposes only
        {
            try
            {
                OpenConnection();
                List<Staff> list = new List<Staff>();
                string query = "SELECT * FROM tblres WHERE res_fname LIKE @keyWord OR res_lname LIKE @keyWord OR res_mi LIKE @keyWord LIMIT 10";
                MySqlParameter parameter = new MySqlParameter("@keyWord", "%" + searchKeyword + "%");
                DataTable dt = ExecuteQueryWithParameters(query, parameter);
                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(row["res_fname"].ToString()))
                    {
                        Staff staff = new Staff
                        {
                            Id = Convert.ToInt32(row["res_id"]),
                            Name = row["res_fname"].ToString() + " " + row["res_mi"].ToString() + " " + row["res_lname"].ToString(),
                        };
                        list.Add(staff);
                    }
                }
                return list;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine("ERROR GETTING STAFF DATA");
                return null;
            }
            finally
            {
                this.Dispose();
            }
        }
        internal List<Researcher> GetResearchers(string researcherName)
        {
            try
            {
                OpenConnection();
                List<Researcher> list = new List<Researcher>();
                string query = "SELECT * FROM tblres WHERE res_fname LIKE @FirstName OR res_lname LIKE @FirstName OR res_mi LIKE @FirstName LIMIT 10";
                MySqlParameter parameter = new MySqlParameter("@FirstName", "%" + researcherName + "%");
                DataTable dt = ExecuteQueryWithParameters(query, parameter);
                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(row["res_fname"].ToString()))
                    {
                        Researcher researcher = new Researcher
                        {
                            Id = Convert.ToInt32(row["res_id"]),
                            Name = row["res_fname"].ToString() + " " + row["res_mi"].ToString() + " " + row["res_lname"].ToString(),
                        };
                        list.Add(researcher);
                    }
                }
                return list;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine("ERROR GETTING RESEARCHERDATA");
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
                Trace.WriteLine("ERROR GETTING PROGRAMDATA");
                return null;
            }
            finally
            {
                this.Dispose();
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
        }

        internal List<Agenda> GetAgendaData() // Get all agenda data from database
        {
            try
            {
                OpenConnection();
                List<Agenda> list = new List<Agenda>();
                string query = "SELECT * FROM tblagenda";
                DataTable dt = ExecuteQuery(query);
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Agenda agenda = new Agenda
                        {
                            Id = Convert.ToInt32(row["agenda_id"]),
                            Desc = row["description"].ToString(),
                        };
                        Trace.WriteLine($"Agenda ID: {agenda.Id} | Agenda Description: {agenda.Desc}"); 
                        list.Add(agenda);
                    }
                }
                else
                {
                    Trace.WriteLine("DataTable is null or empty. No data retrieved from the database.");
                }
                return list;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"ERROR GETTING AGENDADATA: {e.Message}");
                return null;
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
            finally
            {
                this.Dispose();
            }
        }

        public void InsertDocuments(List<ResearchFiles> files, Papers paper)
        {
            try
            {
                OpenConnection();
                string query = "INSERT INTO tblfiles (paper_id, files_content, files_name) VALUES (@PaperID, @DocumentData, @DocumentName)";

                foreach (ResearchFiles file in files)
                {
                    byte[] documentBytes = File.ReadAllBytes(file.FilePath);

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PaperID", paper.Id);
                        command.Parameters.AddWithValue("@DocumentName", file.FileName);
                        command.Parameters.AddWithValue("@DocumentData", documentBytes);

                        command.ExecuteNonQuery();
                    }
                }
                Trace.WriteLine("Document stored successfully!");
            }
            catch (MySqlException ex)
            {
                Trace.WriteLine($"Error storing document: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }
        internal void InsertPaper(Papers paper)
        {
            try
            {
                OpenConnection();
                string insertPaperQuery = "INSERT INTO tblpapers (paper_title, paper_year, school_id, program_id, agenda_id) VALUES (@PaperID, @PaperYear, @SchoolID, @ProgramID, @AgendaID)";
                using (MySqlCommand command = new MySqlCommand(insertPaperQuery, connection))
                {
                    command.Parameters.AddWithValue("@PaperTitle", paper.Title);
                    command.Parameters.AddWithValue("@PaperYear", paper.Year);
                    command.Parameters.AddWithValue("@SchoolID", paper.SchoolID);
                    command.Parameters.AddWithValue("@ProgramID", paper.ProgramID);
                    command.Parameters.AddWithValue("@AgendaID", paper.AgendaID);
                    command.ExecuteNonQuery();
                }
                CloseConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting data: {ex.Message}");
                // Handle exceptions as needed
            }
        }

        internal void InsertAuthors(Papers paper) // Insert authors into database
        {
            try
            {
                OpenConnection();
                foreach (object author in paper.Authors) // Loop through authors
                {
                    if (author is Researcher researcher) // If author is a researcher
                    {
                        Trace.WriteLine($"Inserting Student: [ID]{researcher.Id}, [NAME]{researcher.Name}");
                        string query = "INSERT INTO tblauthors (paper_id, student_id) VALUES (@PaperId, @Id)";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@PaperId", paper.Id);
                            command.Parameters.AddWithValue("@Id", researcher.Id);
                            command.ExecuteNonQuery();
                        }
                    }

                    else if (author is Staff staff) // If author is a staff
                    {
                        Trace.WriteLine($"Inserting Staff: [ID]{staff.Id}, [NAME]{staff.Name}");
                        string query = "INSERT INTO tblauthors (paper_id, employee_id) VALUES (@PaperId, @Id)";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@PaperId", paper.Id);
                            command.Parameters.AddWithValue("@Id", staff.Id);
                            command.ExecuteNonQuery();
                        }
                    }

                    else
                    {
                        Trace.WriteLine("Attempted insert of an unknown author object!");
                    }
                }
                CloseConnection();
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error inserting authors! {e.Message}");
            }
        }

        internal void InsertPanelist(Papers paper) // Insert panelists into database
        {
            try
            {
                OpenConnection();
                foreach (Staff staff in paper.Panelist) // Loop through panelists
                {
                    string query = "INSERT INTO tblpanelists (paper_id, employee_id) VALUES (@PaperId, @Id)";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PaperId", paper.Id);
                        command.Parameters.AddWithValue("@Id", staff.Id);
                        command.ExecuteNonQuery();
                    }
                }
                CloseConnection();
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error inserting panelist: {e.Message}");
            }
        }

        internal void InsertAdviser(Papers paper)
        {
            try
            {
                OpenConnection();
                string query = "INSERT INTO tbladvisers (paper_id, employee_id) VALUES (@PaperId, @Id)";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PaperId", paper.Id);
                    command.Parameters.AddWithValue("@Id", paper.AdviserID);
                    command.ExecuteNonQuery();
                }
                CloseConnection();
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error inserting adviser: {e.Message}");
            }
        }
    }
}