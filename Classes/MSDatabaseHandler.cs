﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows;

namespace RC_IS.Classes
{
    internal class MSDatabaseHandler : IDisposable
    {
        private string connectionString;
        private SqlConnection connection;
        public MSDatabaseHandler()
        {
            try
            {
                this.connectionString = ConfigurationManager.ConnectionStrings["DbCICT"].ConnectionString;
                Trace.WriteLine("MsDatabaseHandler Initiated");
            }
            catch (TimeoutException e)
            {
                MessageBox.Show($"Cannot connect to reference database: {e.Message}");
            }
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
                CloseConnection();
            }
            catch (SqlException ex)
            {
                Trace.WriteLine("MsDatabaseHandler ExecuteQueryWithParameters Error: " + ex.Message);
                return null;
            }
            return dataTable;
        }

        internal async Task<List<Staff>> GetStaffAsync(string searchKeyword)
        {
            try
            {
                List<Staff> list = new List<Staff>();
                string query = "SELECT TOP 10 szEmpFirstName, szEmpLastName, szEmpMiddle, szEmpID " +
                                "FROM dbo.UVW_EMPLOYEES " +
                                "WHERE szEmpFirstName LIKE @KeyWord OR szEmpLastName LIKE @KeyWord OR szEmpMiddle LIKE @KeyWord OR szEmpID LIKE @KeyWord";
                SqlParameter parameter = new SqlParameter("@keyWord", "%" + searchKeyword + "%");
                DataTable dt = await Task.Run(() => ExecuteQueryWithParameters(query, parameter));// Staff Fill
                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(row["szEmpID"].ToString()))
                    {
                        Staff staff = new Staff
                        {
                            Id = Convert.ToInt32(row["szEmpID"]),
                            Name = row["szEmpFirstName"].ToString() + " " + row["szEmpMiddle"].ToString() + " " + row["szEmpLastName"].ToString(),
                        };
                        list.Add(staff);
                    }
                }
                return list;
            }
            catch (SqlException e)
            {
                Trace.WriteLine("ERROR GETTING STAFF DATA");
                return null;
            }
            finally
            {
                this.Dispose();
            }
        }

        internal async Task<List<Researcher>> GetResearchersAsync(string keyWord)
        {
            try
            {
                List<Researcher> list = new List<Researcher>();
                string query = "SELECT TOP 10 szFirstName, szLastName, szMiddleName, szIDNo " +
                                   "FROM DBO.UVW_STUDENTS " +
                                   "WHERE szLastName LIKE @KeyWord OR szFirstName LIKE @KeyWord OR szMiddleName LIKE @KeyWord OR szIDNo LIKE @KeyWord ";
                SqlParameter parameter = new SqlParameter("@KeyWord", "%" + keyWord + "%");
                DataTable dt = await Task.Run(() => ExecuteQueryWithParameters(query, parameter));// Student Fill
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(row["szIDNo"].ToString()))
                        {
                            Researcher researcher = new Researcher
                            {
                                Id = Convert.ToInt32(row["szIDNo"]),
                                Name = row["szFirstName"].ToString() + " " + row["szMiddleName"].ToString() + " " + row["szLastName"].ToString(),
                            };
                            list.Add(researcher);
                        }
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

        internal bool ValidateLocalDatabases()
        {
            try
            {
                OpenConnection();
                return true;
            }
            catch (SqlException e)
            {
                Trace.WriteLine("ERROR VALIDATING LOCAL DATABASES");
                return false;
            }
            finally
            {
                this.Dispose();
            }
        }
    }
}
