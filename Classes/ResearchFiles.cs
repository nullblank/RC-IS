using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Windows;

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

        public void DownloadDocument(Papers paper, string filename)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                string query = "SELECT * FROM tblfiles WHERE paper_id = @PaperID AND files_name = @DocumentName";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@PaperID", paper.Id),
                    new MySqlParameter("@DocumentName", filename),
                };
                DataTable dt = dbHandler.ExecuteQueryWithParameters(query, parameters.ToArray());

                if (dt.Rows.Count > 0)
                {
                    // Use SaveFileDialog to get the destination path and file name
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = dt.Rows[0]["files_name"].ToString();
                    while (saveFileDialog.ShowDialog() == true)
                    {
                        // Get the selected file name and path
                        string destinationPath = saveFileDialog.FileName;
                        if (File.Exists(destinationPath))
                        {
                            // File already exists, prompt user for confirmation or choose a different file name
                            MessageBoxResult result = MessageBox.Show("The file already exists. Do you want to overwrite it?", "File Exists", MessageBoxButton.YesNoCancel);

                            if (result == MessageBoxResult.Yes)
                            {
                                // User chose to overwrite the file
                                MessageBox.Show("Document overwritten successfully!", "Done!", MessageBoxButton.OK, MessageBoxImage.Information);
                                break;
                            }
                            else if (result == MessageBoxResult.No)
                            {
                                // User chose not to overwrite, continue the loop to prompt for a different name
                            }
                            else
                            {
                                // User canceled, exit the method
                                return;
                            }
                        }
                        else
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                byte[] documentBytes = (byte[])row["files_content"];
                                File.WriteAllBytes(destinationPath, documentBytes);
                            }

                            Trace.WriteLine("Document downloaded successfully!");
                            MessageBox.Show("Document downloaded successfully!", "Done!", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        }
                    }
                }
                else
                {
                    Trace.WriteLine("No document found for download.");
                }
            }
            catch (MySqlException ex)
            {
                Trace.WriteLine($"Error downloading document: {ex.Message}");
            }
        }


    }
}
