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
    public class Agenda
    {
        public int Id { get; set; }
        public string Desc { get; set; }

        internal string GetAgendaData(Papers paper)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                string query = "SELECT * FROM tblagenda WHERE agenda_id = @AgendaID";
                MySqlParameter parameter = new MySqlParameter("@AgendaID", paper.AgendaID);
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
                Trace.WriteLine($"ERROR GETTING AGENDA DATA: {e.Message}");
                return null;
            }
        }

        internal List<Agenda> GetAgendaData() // Get all agenda data from database
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                List<Agenda> list = new List<Agenda>();
                Agenda def = new Agenda
                {
                    Id = 0,
                    Desc = "--",
                };
                list.Add(def);
                string query = "SELECT * FROM tblagenda";
                DataTable dt = dbHandler.ExecuteQuery(query);
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

    }
}
