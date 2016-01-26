using StudentRanking.DataAccess;
using StudentRanking.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace StudentRanking.Ranking
{
    public class Ranker
    {
        private const String CONST_REJECTED = "rejected";

        private UsersContext context;
        private QueryManager queryManager;

        struct RankListEntry
        {
            public RankListEntry(String studentEGN, double totalGrade)
            {
                this.studentEGN = studentEGN;
                this.totalGrade = totalGrade;
            }

            public String studentEGN;
            public double totalGrade;
        }

        public Ranker(UsersContext context)
        {
            this.context = context;
            queryManager = new QueryManager(context);
        }

        //Returns a list of student SSNs currently in a Programme by Programme
        private List<RankListEntry> getProgrammeStudents(String ProgrammeName)
        {
            List<RankListEntry> students = new List<RankListEntry>();

            var query = from student in context.FacultyRankLists
                        where student.ProgrammeName == ProgrammeName
                        select student;

            foreach (var student in query)
            {
                students.Add(new RankListEntry(student.EGN, student.TotalGrade));
            }

            return students;
        }

        private void match(String facultyName, List<Preference> preferences, Student student)
        {
            //add student as rejected in the db at first
            FacultyRankList rejected = new FacultyRankList()
            {
                EGN = student.EGN,
                ProgrammeName = CONST_REJECTED + ' ' + facultyName,
                TotalGrade = 0
            };

            context.FacultyRankLists.Add(rejected);
            context.SaveChanges();
            try
            {
            foreach (Preference preference in preferences)
            {
                int quota = queryManager.getQuota(preference.ProgrammeName, (bool)student.Gender);
                List<FacultyRankList> rankList = queryManager.getRankListData(preference.ProgrammeName, (bool)student.Gender);
                double minimalGrade = 0;
                double studentCount = rankList.Count;

                if (rankList.Count != 0)
                {
                    minimalGrade = rankList.Min(list => list.TotalGrade);
                }


                if (preference.TotalGrade > minimalGrade &&
                    studentCount >= quota &&
                    ((quota >= 2 &&
                    rankList[quota - 2].TotalGrade > minimalGrade) ||
                    (quota == 1 &&
                    rankList[quota - 1].TotalGrade > minimalGrade)
                    ))
                {

                    var entries = rankList.Where(entry => entry.TotalGrade == minimalGrade);

                    foreach (FacultyRankList entry in entries)
                    {
                        context.FacultyRankLists.Attach(entry);
                        context.FacultyRankLists.Remove(entry);

                    }
                    context.SaveChanges();
                }

                if ((preference.TotalGrade > 0 &&
                    preference.TotalGrade >= minimalGrade) ||
                    (preference.TotalGrade < minimalGrade &&
                    studentCount < quota))
                {
                    FacultyRankList entry = new FacultyRankList()
                        {
                            EGN = preference.EGN,
                            ProgrammeName = preference.ProgrammeName,
                            TotalGrade = preference.TotalGrade
                        };

                    context.FacultyRankLists.Add(entry);
                    context.FacultyRankLists.Attach(rejected);
                    context.FacultyRankLists.Remove(rejected);
                    context.SaveChanges();

                    break;
                }

            }
            }
            catch (Exception e)
            {
                context.FacultyRankLists.Attach(rejected);
                context.FacultyRankLists.Remove(rejected);
                context.SaveChanges();
                throw e;
            }
        }

        //not used anymore
        private Dictionary<String, List<Preference>> splitPreferencesByFaculty(List<Preference> preferences)
        {
            Dictionary<String, List<Preference>> splittedPreferences = new Dictionary<String, List<Preference>>();
            List<Preference> value;

            foreach (Preference preference in preferences)
            {
                String faculty = queryManager.getFaculty(preference.ProgrammeName);


                if (!splittedPreferences.TryGetValue(faculty, out value))
                {
                    splittedPreferences.Add(faculty, new List<Preference>());
                }

                splittedPreferences[faculty].Add(preference);
            }

            return splittedPreferences;
        }

        private void serve(String facultyName, Student student)
        {
            //handle preferences
            List<Preference> preferences = queryManager.getStudentPreferencesByFaculty(student.EGN, facultyName);

            //handle grading
            Grader grader = new Grader(context);
            grader.grade(student.EGN, preferences);

            //handle ranking
            match(facultyName, preferences, student);
        }

        public delegate void OnFinishListener();
        private OnFinishListener onFinishListener;

        public void setOnFinishListener(OnFinishListener onFinish)
        {
            onFinishListener += onFinish;
        }

        //public async Task start()
        //{
        //    await Task.Run(new Action(startRanker));
        //}

        public async Task start()
        {
            List<Student> students = new List<Student>();

            var getFacultyNames = (from faculty in context.Faculties
                                   select faculty.FacultyName).Distinct();

            List<String> facultyNames = getFacultyNames.ToList();
            List<String> studentEGNs;

            //if we try to rate for a second time we should clear the rejected students and
            //those who did not enroll

            var entriesToDelete = from student in context.Students
                                  from entry in context.FacultyRankLists
                                  where entry.EGN == student.EGN
                                  where student.IsEnrolled == false || (entry.ProgrammeName.StartsWith(CONST_REJECTED + " "))
                                  select entry;

            foreach (FacultyRankList entry in entriesToDelete)
            {
                context.FacultyRankLists.Attach(entry);
                context.FacultyRankLists.Remove(entry);
            }
            context.SaveChanges();

            //iterate through every faculty
            foreach (String facultyName in facultyNames)
            {
                var getStudentsEGNQuery = (from student in context.Students
                                           from preference in context.Preferences
                                           from faculty in context.Faculties
                                           where student.IsEnrolled == false
                                           where faculty.FacultyName == facultyName &&
                                                preference.ProgrammeName == faculty.ProgrammeName &&
                                                preference.EGN == student.EGN
                                           select student.EGN).Distinct();

                var getApprovedStudentsEGNQuery = (from entry in context.FacultyRankLists
                                                   from faculty in context.Faculties
                                                   where faculty.FacultyName == facultyName
                                                   where entry.ProgrammeName == faculty.ProgrammeName || (entry.ProgrammeName.Equals(CONST_REJECTED + " " + faculty.FacultyName))
                                                   select entry.EGN).Distinct();

                int count;
                studentEGNs = getStudentsEGNQuery.ToList();

                //this while cycle is used to match any remaining students rejected
                //in previous iterations
                do
                {
                    count = 0;
                    //iterate through every student with a preference in this faculty
                    foreach (String EGN in studentEGNs)
                    {
                        Student student;
                        student = queryManager.getStudent(EGN);

                        //TODO: This row is for debugging only
                        List<String> lst = getApprovedStudentsEGNQuery.ToList();

                        //if the student is already approved for a programme, skip him/her
                        if (!getApprovedStudentsEGNQuery.Contains(EGN))
                        {
                            serve(facultyName, student);
                            count++;
                        }
                    }

                }
                while (count > 0);
            }

            onFinishListener();
        }

    }
}