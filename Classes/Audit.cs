using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_IS.Classes
{
    public class Audit
    {
        private User _user;
        public Audit(User user)
        {
            _user = user;
        }
        internal void LogUserAction(User user, string action, string table, string row_id, string column)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                string query = "INSERT INTO tblaudit (acc_id, action, `table`, row_id, `column`) " +
                               "VALUES (@accountId, @action, @table, @rowId, @column)";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@accountId", user.UserId),
                    new MySqlParameter("@action", action),
                    new MySqlParameter("@table", table),
                    new MySqlParameter("@rowId", row_id),
                    new MySqlParameter("@column", column)
                };
                dbHandler.ExecuteNonQueryWithParameters(query, parameters.ToArray());
            }
            catch (MySqlException ex)
            {
                Trace.WriteLine("Error logging user action!: " + ex.Message);
            }
        }

    }
}
