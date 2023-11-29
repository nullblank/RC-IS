using MySql.Data.MySqlClient;
using RC_IS.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_IS.Classes
{
    public class Papers
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string ParsedYear { get; set; }
        public int SchoolID { get; set; }
        public string SchoolName { get; set; } // Add property for SchoolName
        public int ProgramID { get; set; }
        public string ProgramName { get; set; } // Add property for ProgramName
        public int AgendaID { get; set; }
        public string AgendaName { get; set; } // Add property for AgendaName
        public int AdviserID { get; set; }
        public string AdviserName { get; set; } // Add property for AdviserName
        public string Comments { get; set; }
        public List<object> Authors { get; set; }
        public List<Staff> Panelist { get; set; }
        public List<ResearchFiles> Files { get; set; }

        public void ParseYearInt()
        {
            string inputString = Year.ToString();

            if (inputString.Length == 8)
            {
                ParsedYear = $"{inputString.Substring(0, 4)} - {inputString.Substring(4)}";
            }
            else
            {
                ParsedYear = "Invalid input";
            }
        }
        internal int InsertPaper(Papers paper)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                string insertPaperQuery = "INSERT INTO tblpapers (paper_title, paper_year, school_id, program_id, agenda_id, isarchive) VALUES (@PaperTitle, @PaperYear, @SchoolID, @ProgramID, @AgendaID, 0)";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@PaperTitle", paper.Title),
                    new MySqlParameter("@PaperYear", paper.Year),
                    new MySqlParameter("@SchoolID", paper.SchoolID),
                    new MySqlParameter("@ProgramID", paper.ProgramID),
                    new MySqlParameter("@AgendaID", paper.AgendaID),
                };
                dbHandler.ExecuteNonQueryWithParameters(insertPaperQuery, parameters.ToArray());
                string getPaperIdQuery = "SELECT paper_id FROM tblpapers WHERE paper_title = @PaperTitle AND paper_year = @PaperYear AND school_id = @SchoolID AND program_id = @ProgramID AND agenda_id = @AgendaID";
                parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@PaperTitle", paper.Title),
                    new MySqlParameter("@PaperYear", paper.Year),
                    new MySqlParameter("@SchoolID", paper.SchoolID),
                    new MySqlParameter("@ProgramID", paper.ProgramID),
                    new MySqlParameter("@AgendaID", paper.AgendaID),
                };
                DataTable dt = dbHandler.ExecuteQueryWithParameters(getPaperIdQuery, parameters.ToArray());
                foreach (DataRow row in dt.Rows)
                {
                    return Convert.ToInt32(row["paper_id"]);
                }
                return -1;
            }
            catch (MySqlException ex)
            {
                Trace.WriteLine($"Error inserting data: {ex.Message}");
                return -1;
            }
        }
        internal List<Papers> GetPapers()
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                List<Papers> list = new List<Papers>();
                string query = "SELECT * FROM tblpapers";
                DataTable dt = dbHandler.ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(row["paper_title"].ToString()) && Convert.ToInt32(row["isarchive"]) != 1)
                    {
                        Papers paper = new Papers
                        {
                            Id = Convert.ToInt32(row["paper_id"]),
                            Title = row["paper_title"].ToString(),
                            Year = Convert.ToInt32(row["paper_year"]),
                            SchoolID = Convert.ToInt32(row["school_id"]),
                            ProgramID = Convert.ToInt32(row["program_id"]),
                            AgendaID = Convert.ToInt32(row["agenda_id"]),
                        };
                        Schools school = new Schools();  paper.SchoolName = school.GetSchoolData(paper);
                        Programs program = new Programs(); paper.ProgramName = program.GetProgramData(paper);
                        Agenda agenda = new Agenda();  paper.AgendaName = agenda.GetAgendaData(paper);
                        paper.ParseYearInt();
                        list.Add(paper);
                    }
                }
                return list;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"ERROR GETTING PAPERS DATA: {e.Message}");
                return null;
            }
        }
        internal List<Papers> GetPapers(string query, string title, int year, int school, int program, int agenda)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                List<Papers> list = new List<Papers>();
                List<MySqlParameter> parameters = new List<MySqlParameter> { new MySqlParameter("@PaperTitle", $"%{title}%") };
                if (year != 0) { parameters.Add(new MySqlParameter("@PaperYear", $"%{year}%")); }
                if (school != 0) { parameters.Add(new MySqlParameter("@PaperSchool", school)); }
                if (program != 0) { parameters.Add(new MySqlParameter("@PaperProgram", program)); }
                if (agenda != 0) { parameters.Add(new MySqlParameter("@PaperAgenda", agenda)); }
                DataTable dt = dbHandler.ExecuteQueryWithParameters(query, parameters.ToArray());
                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(row["paper_title"].ToString()) && Convert.ToInt32(row["isarchive"]) != 1)
                    {
                        Papers paper = new Papers
                        {
                            Id = Convert.ToInt32(row["paper_id"]),
                            Title = row["paper_title"].ToString(),
                            Year = Convert.ToInt32(row["paper_year"]),
                            SchoolID = Convert.ToInt32(row["school_id"]),
                            ProgramID = Convert.ToInt32(row["program_id"]),
                            AgendaID = Convert.ToInt32(row["agenda_id"]),
                        };
                        Schools s = new Schools(); paper.SchoolName = s.GetSchoolData(paper);
                        Programs p = new Programs(); paper.ProgramName = p.GetProgramData(paper);
                        Agenda a = new Agenda(); paper.AgendaName = a.GetAgendaData(paper);
                        paper.ParseYearInt();
                        list.Add(paper);
                    }
                }
                return list;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error getting papers! {e.Message}");
                return null;
            }
        }

        internal void UpdatePaper(Papers paper)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                string query = "UPDATE tblpapers SET paper_title = @PaperTitle, paper_year = @PaperYear, school_id = @SchoolID, program_id = @ProgramID, agenda_id = @AgendaID WHERE paper_id = @PaperID";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@PaperTitle", paper.Title),
                    new MySqlParameter("@PaperYear", paper.Year),
                    new MySqlParameter("@SchoolID", paper.SchoolID),
                    new MySqlParameter("@ProgramID", paper.ProgramID),
                    new MySqlParameter("@AgendaID", paper.AgendaID),
                    new MySqlParameter("@PaperID", paper.Id),
                };
                dbHandler.ExecuteNonQueryWithParameters(query, parameters.ToArray());
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"Error updating paper! {e.Message}");
            }
        }
    }
}
