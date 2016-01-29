
using StudentRanking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;

namespace StudentRanking.Ranking
{
    public class QueryManager
    {
        public const String CONST_REJECTED = "rejected";
        public const String CONST_IGNORED = "ignored";
        private UsersContext context;
        private static QueryManager queryManager = new QueryManager();

        private QueryManager()
        {
            context = new UsersContext();
        }

        private void lockManager()
        {
            Monitor.Enter(this);
        }

        private void unlockManager()
        {
            refresh();
            Monitor.Exit(this);
            
        }

        public void setContext(UsersContext context)
        {
            this.context = context;
        }

        public static QueryManager getInstance()
        {
            return queryManager;
        }

        public void setRankListState(FacultyRankList entry, EntityState state)
        {
            lockManager();
            context.Entry(entry).State = state;
            context.SaveChanges();
            unlockManager();
        }

        public List<String> getIgnoredStudents()
        {
            lockManager();
            var firstPrefAccepted = (
                                     from entry in context.FacultyRankLists
                                     where entry.ProgrammeName.StartsWith(CONST_IGNORED)
                                     select entry.EGN).Distinct();

            List<String> result = firstPrefAccepted.ToList();
            unlockManager();
            return result;
        }

        //Used get a list of unenrolled students who were accepted for their first preference
        public void setupIgnoredStudents()
        {
            lockManager();


            var firstPrefAccepted = (from student in context.Students
                                     where student.IsEnrolled == false
                                     from preference in context.Preferences
                                     where preference.EGN == student.EGN && preference.PrefNumber == 1
                                     from entry in context.FacultyRankLists
                                     where entry.EGN == student.EGN && entry.ProgrammeName == preference.ProgrammeName
                                     select entry).Distinct();

            foreach (FacultyRankList entry in firstPrefAccepted)
            {
                FacultyRankList entry2 = new FacultyRankList();
                entry2.EGN = entry.EGN;
                entry2.ProgrammeName = CONST_IGNORED + " " + entry.ProgrammeName;
                entry2.TotalGrade = 0;
                //entry.ProgrammeName = CONST_IGNORED + " " + entry.ProgrammeName;
                
                context.FacultyRankLists.Attach(entry);
                context.FacultyRankLists.Remove(entry);
                context.FacultyRankLists.Add(entry2);

                //setRankListState(entry2, EntityState.Modified);

            }

            context.SaveChanges();

            //var firstPrefAccepted = (from student in context.Students
            //                        where student.IsEnrolled == false
            //                        from preference in context.Preferences
            //                        where preference.EGN == student.EGN && preference.PrefNumber == 1
            //                        from entry in context.FacultyRankLists
            //                        where entry.EGN == student.EGN && entry.ProgrammeName == preference.ProgrammeName
            //                        select entry.EGN).Distinct();
            //List<String> result = firstPrefAccepted.ToList();
            unlockManager();
        }

        //Clearing the rejected students and
        //those who did not enroll
        public void filterRankingData()
        {
            lockManager();

            var entriesToDelete = from student in context.Students
                                  from entry in context.FacultyRankLists
                                  where entry.EGN == student.EGN
                                  where (student.IsEnrolled == false && !entry.ProgrammeName.StartsWith(CONST_IGNORED)) || (entry.ProgrammeName.StartsWith(QueryManager.CONST_REJECTED + " "))
                                  select entry;

            foreach (FacultyRankList entry in entriesToDelete)
            {
                context.FacultyRankLists.Attach(entry);
                context.FacultyRankLists.Remove(entry);
            }

            context.SaveChanges();
            unlockManager();
        }

        public void addUser(String userName)
        {
            lockManager();
            context.UserProfiles.Add(new UserProfile { UserName = userName });
            context.SaveChanges();
            unlockManager();
        }

        public UserProfile getUser(String userName)
        {
            lockManager();
            UserProfile result = context.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());
            unlockManager();
            return result;
        }

        public List<RankingDates> getRankingDatesContent()
        {
            lockManager();
            List<RankingDates> result = context.Dates.ToList();
            unlockManager();

            return result;
        }

        public void addRankingDatesContent(RankingDates content)
        {
            lockManager();
            context.Dates.Add(content);
            context.SaveChanges();
            unlockManager();
        }

        public void removeRankingDates(RankingDates dates)
        {
            lockManager();
            context.Dates.Attach(dates);
            context.Dates.Remove(dates);
            context.SaveChanges();
            unlockManager();
        }

        //Returns a list of preferences of a student by SSN
        public List<Preference> getStudentPreferences(String EGN)
        {
            lockManager();
            List<Preference> preferences = new List<Preference>();

            var query = from pref in context.Preferences
                        where pref.EGN == EGN
                        select pref;

            preferences = query.ToList();
            unlockManager();
            return preferences;
        }

        public List<String> getApprovedStudentsEGNs(String facultyName)
        {
            lockManager();
            var getApprovedStudentsEGNQuery = (from entry in context.FacultyRankLists
                                               from faculty in context.Faculties
                                               where faculty.FacultyName == facultyName
                                               where entry.ProgrammeName == faculty.ProgrammeName || (entry.ProgrammeName.Equals(CONST_REJECTED + " " + faculty.FacultyName))
                                               select entry.EGN).Distinct();

            List<String> result =  getApprovedStudentsEGNQuery.ToList();
            unlockManager();

            return result;
        }

        public List<String> getStudentEGNs(String facultyName)
        {
            lockManager();
            var getStudentsEGNQuery = (from student in context.Students
                                       from preference in context.Preferences
                                       from faculty in context.Faculties
                                       where student.IsEnrolled == false
                                       where faculty.FacultyName == facultyName &&
                                            preference.ProgrammeName == faculty.ProgrammeName &&
                                            preference.EGN == student.EGN
                                       select student.EGN).Distinct();

            List<String> result = getStudentsEGNQuery.ToList();
            unlockManager();
            return result;
        }

        //Returns a list of preferences of a student by SSN
        public List<Preference> getStudentPreferencesByFaculty(String EGN, String facultyName)
        {
            lockManager();
            List<Preference> preferences = new List<Preference>();

            var query = from pref in context.Preferences
                        from faculty in context.Faculties
                        where pref.EGN == EGN && faculty.FacultyName == facultyName && faculty.ProgrammeName == pref.ProgrammeName
                        orderby pref.PrefNumber ascending
                        select pref;

            preferences = query.ToList();
            unlockManager();
            return preferences;
        }

        public List<String> getFacultyNames()
        {
            lockManager();
            var getFacultyNames = (from faculty in context.Faculties
                                   select faculty.FacultyName).Distinct();

            List<String> result = getFacultyNames.ToList();
            unlockManager();
            return result;
        }

        public void removeFacultyRankListItem(FacultyRankList item)
        {
            lockManager();
            context.FacultyRankLists.Attach(item);
            context.FacultyRankLists.Remove(item);
            context.SaveChanges();
            unlockManager();
        }

        public void removeFacultyRankListItems(IEnumerable<FacultyRankList> entries)
        {
            lockManager();
            foreach (FacultyRankList entry in entries)
            {
                context.FacultyRankLists.Attach(entry);
                context.FacultyRankLists.Remove(entry);
            }
            context.SaveChanges();
            unlockManager();
        }

        //get male/female student count allowed for a programme
        public int getQuota(String programmeName, bool gender)
        {
            lockManager();
            var query = from rules in context.ProgrammesRules
                        where rules.ProgrammeName == programmeName
                        select gender ? rules.MaleCount : rules.FemaleCount;

            int result = query.First();
            unlockManager();
            return result;
        }

        //get formulas components organized in a list
        public List<List<String>> getFormulasComponents(String programmeName)
        {
            lockManager();
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

            List<List<String>> result = allComponents;
            unlockManager();
            return result;
        }

        //get the grades of a student wit a given SSN
        public List<Exam> getStudentGradesList(String studentEGN)
        {
            lockManager();
            List<Exam> grades = new List<Exam>();

            var query = from grade in context.Exams
                        where grade.StudentEGN == studentEGN
                        select grade;

            foreach (Exam grade in query)
            {
                grades.Add(grade);
            }

            List<Exam> result = grades;
            unlockManager();
            return result;
        }

        //get a dictionary with keys - the exam names, and grades as values
        public Dictionary<String, double> getStudentGrades(String studentEGN)
        {
            lockManager();
            Dictionary<String, double> grades = new Dictionary<String, double>();

            var query = from grade in context.Exams
                        where grade.StudentEGN == studentEGN
                        select grade;

            foreach (Exam grade in query)
            {
                grades.Add(grade.ExamName, grade.Grade);
            }

            Dictionary<String, double> result = grades;
            unlockManager();
            return result;
        }

        public void addPreference(Preference preference)
        {
            lockManager();
            if (context.Preferences.Any(pref => pref.EGN.Equals(preference.EGN) && pref.ProgrammeName.Equals(preference.ProgrammeName)))
            {
                unlockManager();
                return;
            }
            context.Preferences.Add(preference);
            context.SaveChanges();
            unlockManager();
        }

        public void editPreference(Preference preference)
        {
            lockManager();
            context.Preferences.Attach(preference);
            context.Entry(preference).Property(x => x.TotalGrade).IsModified = true;
            context.SaveChanges();
            unlockManager();
        }

        public void addFacultyRankListItem(FacultyRankList item)
        {
            lockManager();
            context.FacultyRankLists.Add(item);
            context.SaveChanges();
            unlockManager();
        }
        public Student getStudent(String EGN)
        {
            lockManager();
            Student result =  context.Students.Where(student => student.EGN == EGN).First();
            unlockManager();
            return result;
        }

        public List<FacultyRankList> getRankList(String programmeName)
        {
            lockManager();
            List<FacultyRankList> rankList = new List<FacultyRankList>();
            var query = from rankEntry in context.FacultyRankLists
                        where rankEntry.ProgrammeName == programmeName
                        orderby rankEntry.TotalGrade ascending
                        select rankEntry;

            foreach (FacultyRankList entry in query)
                rankList.Add(entry);

            List<FacultyRankList> result =  rankList;
            unlockManager();
            return result;
        }


        public List<FacultyRankList> getStudentRankList(String EGN)
        {
            lockManager();
            List<FacultyRankList> rankList = new List<FacultyRankList>();
            var query = from rankEntry in context.FacultyRankLists
                        where rankEntry.EGN == EGN
                        select rankEntry;

            foreach (FacultyRankList entry in query)
                rankList.Add(entry);

            List<FacultyRankList> result =  rankList;
            unlockManager();
            return result;
        }

        public List<FacultyRankList> getUnenrolledRankListData(String programmeName, Boolean gender)
        {
            lockManager();
            List<FacultyRankList> result = new List<FacultyRankList>();

            var query = from rankEntry in context.FacultyRankLists
                        where rankEntry.ProgrammeName == programmeName
                        orderby rankEntry.TotalGrade ascending
                        select rankEntry;

            var genderCheck = from student in context.Students
                              where student.Gender == gender && student.IsEnrolled == true
                              select student.EGN;

            List<FacultyRankList> temp = query.ToList();

            foreach (FacultyRankList entry in temp)
            {
                if (genderCheck.Contains(entry.EGN))
                    result.Add(entry);
            }
            unlockManager();
            return result;
        }

        public List<FacultyRankList> getRankListData(String programmeName, Boolean gender)
        {
            lockManager();
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
            unlockManager();
            return result;
        }

        //public String getStudentName(String EGN)
        //{
        //    lockManager();

        //    var student = from s in context.Students
        //                  where s.EGN == EGN
        //                  select s;
        //    var result = student.ToList().First();
        //    unlockManager();
        //    return result.FirstName + " " + result.LastName;

        //}

        public RankingDates getCampaignDates()
        {
            lockManager();
            if ( context.Dates.ToList().Count() != 0 )
            {
                RankingDates result = context.Dates.ToList().First();
                unlockManager();
                return result;

            }
            unlockManager();
            return new RankingDates();
        }

        public List<Exam> getExamsOfStudent(String EGN)
        {
            lockManager();
            var exams = from exam in context.Exams
                        where exam.StudentEGN == EGN
                        select exam;

            List<Exam> result = exams.ToList();
            unlockManager();
            return result;
        }

        public Exam findExamOfStudent(String examName, String EGN)
        {
            lockManager();
            Exam result = context.Exams.Find(examName, EGN);
            unlockManager();
            return result;
        }

        public List<String> getExamNames()
        {
            lockManager();
            var examsNames = from exam in context.ExamNames
                             select exam.Name;

            List<String> result = examsNames.ToList();
            unlockManager();
            return result;
        }

        public void addExam(Exam exam)
        {
            lockManager();
            context.Exams.Add(exam);
            context.SaveChanges();
            unlockManager();
        }

        public void setExamState(Exam exam, EntityState state)
        {
            lockManager();
            context.Entry(exam).State = state;
            context.SaveChanges();
            unlockManager();
        }

        public void removeExam(Exam exam)
        {
            lockManager();
            context.Exams.Remove(exam);
            context.SaveChanges();
            unlockManager();
        }

        public List<Faculty> getFaculties()
        {
            lockManager();
            List<Faculty> result =  context.Faculties.ToList();
            unlockManager();
            return result;
        }

        public void setDatesState(RankingDates dates, EntityState state)
        {
            lockManager();
            context.Entry(dates).State = state;
            context.SaveChanges();
            unlockManager();
        }

        public ProgrammeRules findProgrammeRule(String programmeName)
        {
            lockManager();
            ProgrammeRules result = context.ProgrammesRules.Find(programmeName);
            unlockManager();
            return result;
        }

        public void addProgrammeRule(ProgrammeRules rule)
        {
            lockManager();
            context.ProgrammesRules.Add(rule);
            context.SaveChanges();
            unlockManager();
        }

        public void setProgrammeRuleState(ProgrammeRules rule, EntityState state)
        {
            lockManager();
            context.Entry(rule).State = state;
            context.SaveChanges();
            unlockManager();
        }

        public List<Formula> getProgrammeFormulae(String programmeName)
        {
            lockManager();
            var query = from formula in context.Formulas
                        where formula.ProgrammeName == programmeName
                        select formula;

             List<Formula> result = query.ToList();
             unlockManager();
             return result;
        }

        public List<Student> getStudents()
        {
            lockManager();
            List<Student> result = context.Students.ToList();
            unlockManager();
            return result;
        }

        public void addStudent(Student student)
        {
            lockManager();
            context.Students.Add(student);
            context.SaveChanges();
            unlockManager();

        }

        public void setStudentState(Student student, EntityState state)
        {
            lockManager();
            context.Entry(student).State = state;
            context.SaveChanges();
            unlockManager();
        }

        public void removeStudent(Student student)
        {
            lockManager();
            context.Students.Attach(student);
            context.Students.Remove(student);
            context.SaveChanges();
            unlockManager();
        }

        public Student findStudent(String id)
        {
            lockManager();
            Student result= context.Students.Find(id);
            unlockManager();
            return result;
        }

        public Faculty getFaculty(String programmeName)
        {
            lockManager();
            Faculty result = context.Faculties.Find(programmeName);
            unlockManager();
            return result;
        }

        public void removePreference(Preference preference)
        {
            lockManager();
            context.Preferences.Attach(preference);
            context.Preferences.Remove(preference);
            context.SaveChanges();
            unlockManager();
        }

        public List<String> getProgrammeNames(String facultyName)
        {
            lockManager();
            var query = from b in context.Faculties
                    where b.FacultyName == facultyName
                    select b.ProgrammeName;

            List<String> result = query.ToList();
            unlockManager();
            return result;
        }

        public int getPrefNumber(String EGN, String programmeName)
        {
            lockManager();
             var prefNumber = from pref in context.Preferences
                where pref.EGN == EGN && pref.ProgrammeName == programmeName
                select pref.PrefNumber;

            int result = prefNumber.First();
            unlockManager();
            return result;
        }

        //public bool isRankingReady(String ranking)
        //{
        //    lockManager();
        //    var rankings = from data in context.Dates
        //                   select data;
        //    bool result = false;

        //    var rankingsList = rankings.ToList();
        //    switch (ranking)
        //    {
        //        case "first": result = rankingsList.Last().FirstRankingDate == "true"; break;
        //        case "second": result = rankingsList.Last().SecondRankingDate == "true"; break;
        //        case "third": result = rankingsList.Last().ThirdRankingDate == "true"; break;
        //    }

        //    unlockManager();
        //    return result;
        //}

        public void refresh()
        {
            context.Dispose();
            context = new UsersContext();
        }

    }
}