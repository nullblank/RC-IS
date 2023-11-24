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
    public class Programs
    {
        public int Id { get; set; }
        public string Desc { get; set; }

        internal List<Programs> GetProgramData(int schoolId)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                List<Programs> list = new List<Programs>();
                Programs def = new Programs
                {
                    Id = 0,
                    Desc = "--",
                };
                list.Add(def);
                string query = "SELECT * FROM tblprograms WHERE school_id = @SchoolID";
                MySqlParameter parameter = new MySqlParameter("@SchoolID", schoolId);
                DataTable dt = dbHandler.ExecuteQueryWithParameters(query, parameter);
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
        }

        internal string GetProgramData(Papers paper)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                string query = "SELECT * FROM tblprograms WHERE program_id = @ProgramID";
                MySqlParameter parameter = new MySqlParameter("@ProgramID", paper.ProgramID);
                DataTable dt = dbHandler.ExecuteQueryWithParameters(query, parameter);
                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(row["description"].ToString()))
                    {
                        return row["description"].ToString();
                    }
                }
                return null;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"ERROR GETTING PROGRAM DATA: {e.Message}");
                return null;
            }
        }

    }
}
