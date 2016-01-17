using StudentRanking.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentRanking.DataAccess
{
    public class RulesModel
    {
        public List<String> Faculties()
        {
            
            RankingContext context = new RankingContext();

            var f = from b in context.Faculties
                    select b.FacultyName;

            return f.ToList<String>();
        }

        public List<String> Programmes(String facultyName)
        {
            
            RankingContext context = new RankingContext();

            var f = from b in context.Faculties
                    where b.FacultyName == facultyName
                    select b.ProgrammeName;

            return f.ToList<String>();
        }

        public int maleCount;
        public int femaleCount;
    }
}