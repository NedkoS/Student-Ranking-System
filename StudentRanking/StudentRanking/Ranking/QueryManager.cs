using StudentRanking.DataAccess;
using StudentRanking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentRanking.Ranking
{
    public class QueryManager
    {
        private RankingContext context;

        public QueryManager(RankingContext context)
        {
            this.context = context;
        }

        //Returns a list of preferences of a student by SSN
        public List<Preference> getStudentPreferences(String EGN)
        {
            List<Preference> preferences = new List<Preference>();

            var query = from pref in context.Preferences
                        where pref.EGN == EGN
                        select pref;

            preferences = query.ToList();

            return preferences;
        }

        //Returns a list of preferences of a student by SSN
        public List<Preference> getStudentPreferencesByFaculty(String EGN, String facultyName)
        {
            List<Preference> preferences = new List<Preference>();

            var query = from pref in context.Preferences
                        from faculty in context.Faculties
                        where pref.EGN == EGN && faculty.FacultyName == facultyName && faculty.ProgrammeName == pref.ProgrammeName
                        orderby pref.PrefNumber ascending
                        select pref;

            preferences = query.ToList();

            return preferences;
        }

        //get faculty by a programme name
        public String getFaculty(String programmeName)
        {
            var query = from ProgrammeToFaculty in context.Faculties
                        where ProgrammeToFaculty.ProgrammeName == programmeName
                        select ProgrammeToFaculty.FacultyName;

            return query.First();
        }

        //get male/female student count allowed for a programme
        public int getQuota(String programmeName, bool gender)
        {
            var query = from rules in context.ProgrammesRules
                        where rules.ProgrammeName == programmeName
                        select gender ? rules.MaleCount : rules.FemaleCount;

            return query.First();
        }

        //get formulas components organized in a list
        public List<List<String>> getFormulasComponents(String programmeName)
        {
            List<List<String>> allComponents = new List<List<string>>();
            List<String> components = new List<string>();

            var query = from formula in context.Formulas
                        where formula.ProgrammeName == programmeName
                        select formula;

            foreach (var formula in query)
            {
                if (formula.C1 > 0)
                {
                    components.Add(formula.C1.ToString());
                    components.Add(formula.X);
                }

                if (formula.C2 > 0)
                {
                    components.Add(formula.C2.ToString());
                    components.Add(formula.Y);
                }

                if (formula.C3 > 0)
                {
                    components.Add(formula.C3.ToString());
                    components.Add(formula.Z);
                }

                if (formula.C4 > 0)
                {
                    components.Add(formula.C4.ToString());
                    components.Add(formula.W);
                }

                allComponents.Add(components);
                components = new List<string>();
            }

            return allComponents;
        }

        //get the grades of a student wit a given SSN
        public List<Exam> getStudentGradesList(String studentEGN)
        {
            List<Exam> grades = new List<Exam>();

            var query = from grade in context.Exams
                        where grade.StudentEGN == studentEGN
                        select grade;

            foreach (Exam grade in query)
            {
                grades.Add(grade);
            }

            return grades;
        }

        //get a dictionary with keys - the exam names, and grades as values
        public Dictionary<String, double> getStudentGrades(String studentEGN)
        {
            Dictionary<String, double> grades = new Dictionary<String, double>();

            var query = from grade in context.Exams
                        where grade.StudentEGN == studentEGN
                        select grade;

            foreach (Exam grade in query)
            {
                grades.Add(grade.ExamName, grade.Grade);
            }

            return grades;
        }

        public Student getStudent(String EGN)
        {
            return context.Students.Where(student => student.EGN == EGN).First();
        }

        public List<FacultyRankList> getRankList(String programmeName)
        {
            List<FacultyRankList> rankList = new List<FacultyRankList>();
            var query = from rankEntry in context.FacultyRankLists
                        where rankEntry.ProgrammeName == programmeName
                        orderby rankEntry.TotalGrade ascending
                        select rankEntry;

            foreach (FacultyRankList entry in query)
                rankList.Add(entry);

            return rankList;
        }


        public List<FacultyRankList> getStudentRankList(String EGN)
        {
            List<FacultyRankList> rankList = new List<FacultyRankList>();
            var query = from rankEntry in context.FacultyRankLists
                        where rankEntry.EGN == EGN
                        select rankEntry;

            foreach (FacultyRankList entry in query)
                rankList.Add(entry);

            return rankList;
        }

        public List<FacultyRankList> getRankListData(String programmeName, Boolean gender)
        {
            List<FacultyRankList> result = new List<FacultyRankList>();

            var query = from rankEntry in context.FacultyRankLists
                        where rankEntry.ProgrammeName == programmeName
                        orderby rankEntry.TotalGrade ascending
                        select rankEntry;

            var genderCheck = from student in context.Students
                              where student.Gender == gender
                              select student.EGN;

            List<FacultyRankList> temp = query.ToList();

            foreach (FacultyRankList entry in temp)
            {
                if (genderCheck.Contains(entry.EGN))
                    result.Add(entry);
            }

            return result;
        }


        public RankingDates getCampaignDates()
        {
            if ( context.Dates.ToList().Count() != 0 )
            {
                return context.Dates.ToList().First();

            }

            return new RankingDates();

            

        }
    }
}