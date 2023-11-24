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
    public class Schools
    {
        public int Id { get; set; }
        public string Desc { get; set; }

        internal string GetSchoolData(Papers paper)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                string query = "SELECT * FROM tblschool WHERE school_id = @SchoolID";
                MySqlParameter parameter = new MySqlParameter("@SchoolID", paper.SchoolID);
                DataTable dt = dbHandler.ExecuteQueryWithParameters(query, parameter);
                foreach (DataRow row in dt.Rows)
                {
                    return row["tinydesc"].ToString();
                }
                Trace.WriteLine($"ERROR GETTING SCHOOLDATA!");
                return null;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"ERROR GETTING SCHOOLDATA: {e.Message}");
                return null;
            }
        }
        internal List<Schools> GetSchoolData()
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                List<Schools> list = new List<Schools>();
                string query = "SELECT * FROM tblschool";
                DataTable dt = dbHandler.ExecuteQuery(query);
                Schools def = new Schools
                {
                    Id = 0,
                    Desc = "--",
                };
                list.Add(def);
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
    }
}
