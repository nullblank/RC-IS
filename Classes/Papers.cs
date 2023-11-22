using RC_IS.Windows;
using System;
using System.Collections.Generic;
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

    }
}
