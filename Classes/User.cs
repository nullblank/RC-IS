using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_IS.Classes
{
    internal class User
    {
        public string AccountName { get; set; }
        public string AccountPass { get; set; }
        public string AccountRole { get; set; }
        public User(string accountname, string accountpass, string accountrole)
        {
            AccountName = accountname;
            AccountPass = accountpass;
            AccountRole = accountrole;
        }
        public override string ToString()
        {
            return $"{AccountName} {AccountPass} {AccountRole}";
        }
    }
}
