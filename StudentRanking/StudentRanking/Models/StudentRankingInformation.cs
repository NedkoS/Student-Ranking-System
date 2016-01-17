using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentRanking.Models
{
    public class StudentRankingInformation
    {
        public int PrefNumber { get; set; }
        public String ProgrammeName { get; set; }
        public String FacultyName { get; set; }
        public double FinalResult { get; set; }
    }
}