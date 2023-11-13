using System;
using System.Collections.Generic;
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
        public void Log(string action, string table, string row_id, string column)
        {
            DatabaseHandler dbHandler = new DatabaseHandler();
            dbHandler.LogUserAction(_user, action, table, row_id, column);
        }
    }
}
