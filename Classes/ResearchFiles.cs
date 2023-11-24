using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace RC_IS.Classes
{
    public class ResearchFiles
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }

        internal List<ResearchFiles> GetFiles(int id)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                List<ResearchFiles> list = new List<ResearchFiles>();
                string query = "SELECT * FROM tblfiles WHERE paper_id = @PaperId";
                MySqlParameter parameter = new MySqlParameter("@PaperId", id);
                DataTable dt = dbHandler.ExecuteQueryWithParameters(query, parameter);
                foreach (DataRow row in dt.Rows)
                {
                    ResearchFiles researchFiles = new ResearchFiles
                    {
                        FileName = row["files_name"].ToString(),
                        FileData = (byte[])row["files_content"],
                    };
                    Trace.WriteLine($"GET: [TYPE]FILES [FILENAME]{researchFiles.FileName} [BLOB]{researchFiles.FileData.ToString()}");
                    list.Add(researchFiles);
                }
                return list;
            }
            catch (MySqlException e)
            {
                Trace.WriteLine($"GetFiles Error: {e.Message}");
                Trace.WriteLine($"Error Code: {e.ErrorCode}");
                Trace.WriteLine($"SQL State: {e.SqlState}");
                return null;
            }
        }
        public void InsertDocuments(Papers paper)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                string query = "INSERT INTO tblfiles (paper_id, files_content, files_name) VALUES (@PaperID, @DocumentData, @DocumentName)";
                foreach (ResearchFiles file in paper.Files)
                {
                    byte[] documentBytes = File.ReadAllBytes(file.FilePath);
                    List<MySqlParameter> parameters = new List<MySqlParameter>
                    {
                        new MySqlParameter("@PaperID", paper.Id),
                        new MySqlParameter("@DocumentName", file.FileName),
                        new MySqlParameter("@DocumentData", documentBytes),
                    };
                    dbHandler.ExecuteQueryWithParameters(query, parameters.ToArray());
                }
                Trace.WriteLine("Document stored successfully!");
            }
            catch (MySqlException ex)
            {
                Trace.WriteLine($"Error storing document: {ex.Message}");
            }
        }
    }
}
