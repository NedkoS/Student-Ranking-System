using StudentRanking.DataAccess;
using StudentRanking.Ranking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentRanking.DataAccess
{
    public class RulesModel
    {
        QueryManager queryManager = QueryManager.getInstance();
        public List<String> Faculties()
        {
            return queryManager.getFacultyNames();
        }

        public List<String> Programmes(String facultyName)
        {
            return queryManager.getProgrammeNames(facultyName);
        }

        public int maleCount;
        public int femaleCount;
    }
}