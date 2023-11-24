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
    public class Researcher
    {
        public int Id { get; set; }
        public string Name { get; set; }

        internal List<Researcher> GetResearchers(string researcherName)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                List<Researcher> list = new List<Researcher>();
                string query = "SELECT * FROM tblres WHERE res_fname LIKE @FirstName OR res_lname LIKE @FirstName OR res_mi LIKE @FirstName LIMIT 10";
                MySqlParameter parameter = new MySqlParameter("@FirstName", "%" + researcherName + "%");
                DataTable dt = dbHandler.ExecuteQueryWithParameters(query, parameter);
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
        }
    }
}
