using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using System.Diagnostics;

namespace RC_IS.Classes
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public int Role { get; set; }
        public User(string username)
        {
            Username = username;
        }
        public bool Authenticate(string password)
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                return dbHandler.AuthenticateUser(Username, password, this);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error authenticating user: " + ex.Message);
                return false;
            }
        }
        public bool InsertUser()
        {
            try
            {
                DatabaseHandler dbHandler = new DatabaseHandler();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting user: " + ex.Message);
                return false;
            }
        }

    }
}
