using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace RC_IS.Classes
{
    internal class MSDatabaseHandler : IDisposable
    {
        private string connectionString;
        private SqlConnection connection;
        public MSDatabaseHandler()
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["DbCICT"].ConnectionString;
            Trace.WriteLine("MsDatabaseHandler Initiated");
        }

        private void OpenConnection()
        {
            if (connection == null)
            {
                connection = new SqlConnection(connectionString);
                Trace.WriteLine("MsDatabaseHandler Opened");
            }

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                Trace.WriteLine("MsDatabaseHandler Opened");
            }
        }

        private void CloseConnection()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                Trace.WriteLine("MsDatabaseHandler Closed");
            }
        }

        public void Dispose()
        {
            CloseConnection();
            connection?.Dispose();
            Trace.WriteLine("DatabaseHandler Closed and Disposed");
        }

        public DataTable ExecuteQuery(string query)
        {
            try
            {
                OpenConnection();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (SqlException e)
            {
                Trace.WriteLine("MsDatabaseHandler ExecuteQuery Error: " + e.Message);
                return null;
            }
            finally
            {
                Dispose();
            }
        }

        public DataTable ExecuteQueryWithParameters(string query, params SqlParameter[] parameters)
        {
            DataTable dataTable = new DataTable();
            try
            {
                OpenConnection();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (SqlException ex)
            {
                Trace.WriteLine("MsDatabaseHandler ExecuteQueryWithParameters Error: " + ex.Message);
                return null;
            }
            finally
            {
                Dispose();
            }

            return dataTable;
        }

        internal async Task<List<Researcher>> GetResearchersAsync(string keyWord)
        {
            try
            {
                OpenConnection();
                List<Researcher> list = new List<Researcher>();
                string query = "SELECT TOP 10 * FROM dbo.UVW_CURRENT_ENROLLED_STUDENTS WHERE STDN_FNM LIKE @KeyWord OR STDN_LNM LIKE @KeyWord OR STDN_MID LIKE @KeyWord OR STDN_IDN LIKE @KeyWord";
                SqlParameter parameter = new SqlParameter("@KeyWord", "%" + keyWord + "%");
                DataTable dt = await Task.Run(() => ExecuteQueryWithParameters(query, parameter));

                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(row["STDN_IDN"].ToString()))
                    {
                        Researcher researcher = new Researcher
                        {
                            Id = Convert.ToInt32(row["STDN_IDN"]),
                            Name = row["STDN_FNM"].ToString() + " " + row["STDN_MID"].ToString() + " " + row["STDN_LNM"].ToString(),
                        };
                        list.Add(researcher);
                    }
                }
                return list;
            }
            catch (SqlException e)
            {
                Trace.WriteLine("ERROR GETTING RESEARCHERDATA");
                return null;
            }
            finally
            {
                this.Dispose();
            }
        }

        /*
        internal List<Researcher> GetResearchers(string researcherName)
        {
            try
            {
                OpenConnection();
                List<Researcher> list = new List<Researcher>();
                string query = "SELECT TOP 10 * FROM dbo.UVW_CURRENT_ENROLLED_STUDENTS WHERE STDN_FNM LIKE @FirstName OR STDN_LNM LIKE @FirstName OR STDN_MID LIKE @FirstName OR STDN_IDN LIKE @FirstName";
                SqlParameter parameter = new SqlParameter("@FirstName", "%" + researcherName + "%");
                DataTable dt = ExecuteQueryWithParameters(query, parameter);
                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(row["STDN_IDN"].ToString()))
                    {
                        Researcher researcher = new Researcher
                        {
                            Id = Convert.ToInt32(row["STDN_IDN"]),
                            Name = row["STDN_FNM"].ToString() + " " + row["STDN_MID"].ToString() + " " + row["STDN_LNM"].ToString(),
                        };
                        list.Add(researcher);
                    }
                }
                return list;
            }
            catch (SqlException e)
            {
                Trace.WriteLine("ERROR GETTING RESEARCHERDATA");
                return null;
            }
            finally
            {
                this.Dispose();
            }
        }
        */

    }
}
