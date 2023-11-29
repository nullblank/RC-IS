using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace RC_IS.Classes
{
    public class Staff
    {
        public int Id { get; set; }
        public string Name { get; set; }

        internal List<Staff> GetStaff(string searchKeyword) // REPLACE WITH GET STAFF <-- Current function is for testing purposes only
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                List<Staff> list = new List<Staff>();
                string query = "SELECT * FROM tblres WHERE res_fname LIKE @keyWord OR res_lname LIKE @keyWord OR res_mi LIKE @keyWord LIMIT 10";
                MySqlParameter parameter = new MySqlParameter("@keyWord", "%" + searchKeyword + "%");
                DataTable dt = dbHandler.ExecuteQueryWithParameters(query, parameter);
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
                Trace.WriteLine($"MySqlException thrown at RC-IS.Classes.Staff.GetStaff(): {e.Message}");
                return null;
            }
        }

        internal Staff GetAdviser(int id) // REPLACE WITH GET STAFF <-- Current function is for testing purposes only
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                string query = "SELECT * FROM tbladvisers WHERE paper_id = @Id";
                MySqlParameter parameter = new MySqlParameter("@Id", id);
                DataTable dt = dbHandler.ExecuteQueryWithParameters(query, parameter);
                DataRow row = dt.Rows[0];
                Staff staff = new Staff
                {
                    Id = Convert.ToInt32(row["employee_id"]),
                    Name = row["employee_name"].ToString(),
                };
                return staff;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"MySqlException thrown at RC-IS.Classes.Staff.GetAdviser(): {e.Message}");
                return null;
            }
        }

        internal void InsertPanelist(Papers paper) // Insert panelists into database
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                foreach (Staff staff in paper.Panelist) // Loop through panelists
                {
                    string query = "INSERT INTO tblpanelists (paper_id, employee_id, employee_name) VALUES (@PaperId, @Id, @EmpName)";
                    MySqlParameter[] parameters =
                    {
                        new MySqlParameter("@PaperId", paper.Id),
                        new MySqlParameter("@Id", staff.Id),
                        new MySqlParameter("@EmpName", staff.Name)
                    };
                    dbHandler.ExecuteNonQueryWithParameters(query, parameters.ToArray());
                }
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error inserting panelist: {e.Message}");
            }
        }

        internal List<Staff> GetPanelists(int id)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                List<Staff> list = new List<Staff>();
                string query = "SELECT * FROM tblpanelists WHERE paper_id = @PaperId";
                MySqlParameter parameter = new MySqlParameter("@PaperId", id);
                DataTable dt = dbHandler.ExecuteQueryWithParameters(query, parameter);
                foreach (DataRow row in dt.Rows)
                {
                    Staff staff = new Staff
                    {
                        Id = Convert.ToInt32(row["employee_id"]),
                        Name = row["employee_name"].ToString(),
                    };
                    Trace.WriteLine($"GET: [TYPE]PANELIST [ID] {staff.Id}  [NAME] {staff.Name}");
                    list.Add(staff);
                }
                return list;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"MySqlException thrown at RC-IS.Classes.Staff.GetPanelists(): {e.Message}");
                return null;
            }
        }

        internal void InsertAdviser(Papers paper)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                string query = "INSERT INTO tbladvisers (paper_id, employee_id, employee_name) VALUES (@PaperId, @Id, @EmpName)";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@PaperId", paper.Id),
                    new MySqlParameter("@Id", paper.AdviserID),
                    new MySqlParameter("@EmpName", paper.AdviserName)
                };
                dbHandler.ExecuteNonQueryWithParameters(query, parameters.ToArray());
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error inserting adviser: {e.Message}");
            }
        }

        internal void UpdateAdviser(Papers paper)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                string query = "UPDATE tbladvisers SET employee_id = @Id, employee_name = @EmpName WHERE paper_id = @PaperId";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@PaperId", paper.Id),
                    new MySqlParameter("@Id", paper.AdviserID),
                    new MySqlParameter("@EmpName", paper.AdviserName)
                };
                dbHandler.ExecuteNonQueryWithParameters(query, parameters.ToArray());
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error updating adviser: {e.Message}");
            }
        }

        internal void UpdatePanelists(Papers paper)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                string query = "DELETE FROM tblpanelists WHERE paper_id = @PaperId";
                MySqlParameter parameter = new MySqlParameter("@PaperId", paper.Id);
                dbHandler.ExecuteNonQueryWithParameters(query, parameter);
                InsertPanelist(paper);
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error updating panelists: {e.Message}");
            }
        }

        internal int GetAdviserId(int id)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                string query = "SELECT employee_id FROM tbladvisers WHERE paper_id = @PaperId";
                MySqlParameter parameter = new MySqlParameter("@PaperId", id);
                DataTable dt = dbHandler.ExecuteQueryWithParameters(query, parameter);
                DataRow row = dt.Rows[0];
                return Convert.ToInt32(row["employee_id"]);
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"MySqlException thrown at RC-IS.Classes.Staff.GetAdviserId(): {e.Message}");
                return 0;
            }
        }
    }
}
