using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_IS.Classes
{
    class Authors
    {
        int Id { get; set; }
        string Name { get; set; }

        internal void InsertAuthors(Papers paper) // Insert authors into database
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                foreach (object author in paper.Authors) // Loop through authors
                {
                    if (author is Researcher researcher) // If author is a researcher
                    {
                        Trace.WriteLine($"Inserting Student: [ID]{researcher.Id}, [NAME]{researcher.Name}");
                        string query = "INSERT INTO tblauthors (paper_id, author_name, student_id) VALUES (@PaperId, @AuthorName, @Id)";
                        List<MySqlParameter> parameters = new List<MySqlParameter>
                        {
                            new MySqlParameter("@PaperId", paper.Id),
                            new MySqlParameter("@AuthorName", researcher.Name),
                            new MySqlParameter("@Id", researcher.Id)
                        };
                        dbHandler.ExecuteQueryWithParameters(query, parameters.ToArray());
                    }

                    else if (author is Staff staff) // If author is a staff
                    {
                        Trace.WriteLine($"Inserting Staff: [ID]{staff.Id}, [NAME]{staff.Name}");
                        string query = "INSERT INTO tblauthors (paper_id, author_name, employee_id) VALUES (@PaperId, @AuthorName, @Id)";
                        List<MySqlParameter> parameters = new List<MySqlParameter>
                        {
                            new MySqlParameter("@PaperId", paper.Id),
                            new MySqlParameter("@AuthorName", staff.Name),
                            new MySqlParameter("@Id", staff.Id)
                        };
                        dbHandler.ExecuteQueryWithParameters(query, parameters.ToArray());
                    }

                    else
                    {
                        Trace.WriteLine("Attempted insert of an unknown author object!");
                    }
                }
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error inserting authors! {e.Message}");
            }
        }
        internal List<object> GetAuthors(int id)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                List<object> authors = new List<object>();
                string query = "SELECT * FROM tblauthors WHERE paper_id = @PaperId";
                MySqlParameter parameter = new MySqlParameter("@PaperId", id);
                DataTable dt = dbHandler.ExecuteQueryWithParameters(query, parameter);
                foreach (DataRow row in dt.Rows)
                {
                    if (row["student_id"] != DBNull.Value)
                    {
                        Researcher researcher = new Researcher
                        {
                            Id = Convert.ToInt32(row["student_id"]),
                            Name = row["author_name"].ToString(),
                        };
                        Trace.WriteLine($"GET: [TYPE]STUDENT [ID]{researcher.Id} [NAME]{researcher.Name}");
                        authors.Add(researcher);
                    }
                    else if (row["employee_id"] != DBNull.Value)
                    {
                        Staff staff = new Staff
                        {
                            Id = Convert.ToInt32(row["employee_id"]),
                            Name = row["author_name"].ToString(),
                        };
                        Trace.WriteLine($"GET: [TYPE]STAFF [ID]{staff.Id} [NAME]{staff.Name}");
                        authors.Add(staff);
                    }
                }
                return authors;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error getting authors: {e.Message}");
                return null;
            }
        }
    }
}
