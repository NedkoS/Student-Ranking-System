
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

        private QueryManager queryManager;

        public Ranker()
        {
            queryManager = QueryManager.getInstance();
        }

        private void match(String facultyName, List<Preference> preferences, Student student)
        {
            //add student as rejected in the db at first
            FacultyRankList rejected = new FacultyRankList()
            {
                EGN = student.EGN,
                ProgrammeName = QueryManager.CONST_REJECTED + ' ' + facultyName,
                TotalGrade = 0
            };

            queryManager.addFacultyRankListItem(rejected);

            try
            {
                foreach (Preference preference in preferences)
                {
                    int quota;
                    List<FacultyRankList> rankList;

                    quota = queryManager.getQuota(preference.ProgrammeName, (bool)student.Gender);
                    rankList = queryManager.getRankListData(preference.ProgrammeName, (bool)student.Gender);

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


                        queryManager.removeFacultyRankListItems(entries);

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


                        queryManager.addFacultyRankListItem(entry);
                        queryManager.removeFacultyRankListItem(rejected);

                        break;
                    }

                }
            }
            catch (Exception e)
            {
                queryManager.removeFacultyRankListItem(rejected);

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
                String faculty = queryManager.getFaculty(preference.ProgrammeName).FacultyName;


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
            List<Preference> preferences;

            preferences = queryManager.getStudentPreferencesByFaculty(student.EGN, facultyName);


            //handle grading
            Grader grader = new Grader();
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

            List<String> facultyNames = queryManager.getFacultyNames();

            //if we try to rate for a second time we should clear the rejected students and
            //those who did not enroll
            queryManager.setupIgnoredStudents();
            queryManager.filterRankingData();
            List<String> ignoreList = queryManager.getIgnoredStudents();

            List<Task> tasks = new List<Task>();
            //iterate through every faculty
            foreach (String facultyName in facultyNames)
            {
                Task facultyTask = Task.Run(() => rankFaculty(facultyName, ignoreList));
                tasks.Add(facultyTask);
            }

            foreach (Task task in tasks)
            {
                await task;
            }

            onFinishListener();
        }


        private void rankFaculty(String facultyName, List<String> ignoreList)
        {

            List<String> studentEGNs;
            int count;
            studentEGNs = queryManager.getStudentEGNs(facultyName);

            //this while cycle is used to match any remaining students rejected
            //in previous iterations
            do
            {
                count = 0;
                //iterate through every student with a preference in this faculty
                foreach (String EGN in studentEGNs)
                {
                    Student student;
                    bool isApproved;

                    student = queryManager.getStudent(EGN);
                    isApproved = queryManager.getApprovedStudentsEGNs(facultyName).Contains(EGN);

                    //if the student is already approved for a programme, skip him/her
                    if (!isApproved && !ignoreList.Contains(EGN))
                    {
                        serve(facultyName, student);
                        count++;
                    }
                }

            }while (count > 0);

            onFinishListener();
        }
    }
}