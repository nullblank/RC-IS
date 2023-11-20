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
        public int SchoolID { get; set; }
        public int ProgramID { get; set; }
        public int AgendaID { get; set; }
        public int AdviserID { get; set; }
        public string Comments { get; set; }
        
        public List<Researcher> Authors { get; set; }
        public List<Staff> Panelist { get; set; }
        public List<ResearchFiles> Files { get; set; }

    }
}
