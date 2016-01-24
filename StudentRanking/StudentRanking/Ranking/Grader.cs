using StudentRanking.DataAccess;
using StudentRanking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentRanking.Ranking
{
    public class Grader
    {
        private const String MATRICULARITY_EXAM = "матура";
        private const String DIPLOMA = "диплома";
        private const double MINIMAL_ALLOWED_GRADE = 3;

        private UsersContext context;
        private QueryManager queryManager;
        private Dictionary<string, double> grades;

        public Grader(UsersContext context)
        {
            this.context = context;
            queryManager = new QueryManager(context);
        }

        private List<String> getMatricularityExams(List<List<String>> formulas)
        {
            List<String> matricularityExams = new List<string>();
            double value;

            for (int formulaInd = 0; formulaInd < formulas.Count; formulaInd++)
            {
                for (int componentInd = 1; componentInd < formulas[formulaInd].Count; componentInd += 2)
                {
                    if (formulas[formulaInd][componentInd].ToLower().Contains(MATRICULARITY_EXAM) && 
                        grades.TryGetValue(formulas[formulaInd][componentInd], out value))
                    {
                        String matricularityExam = formulas[formulaInd][componentInd].ToLower().Replace(MATRICULARITY_EXAM, String.Empty);
                        matricularityExam.Trim();
                        matricularityExams.Add(matricularityExam);
                    }

                }
            }

            return matricularityExams;
        }

        private Boolean hasMatricularityGrade(String exam, List<String> matricularityExams)
        {
            String filteredExamName = exam.ToLower().Replace(DIPLOMA, String.Empty);
            filteredExamName.Trim();

            if (matricularityExams.Contains(filteredExamName))
                return true;

            return false;
        }

        private double calculateTotalGrade(String StudentEGN, String programmeName)
        {
            //Boolean useMatExam = false; //should the matricularity exam be used instead of the diploma grade
            Double value;
            //TODO: return formulas applicable for this student

            List<List<String>> formulas = queryManager.getFormulasComponents(programmeName);
            double totalGrade = 0;
            double maxGrade = 0;

            List<String> matricularityExams = getMatricularityExams(formulas);

            for (int formulaInd = 0; formulaInd < formulas.Count; formulaInd++)
            {

                for (int componentInd = 1; componentInd < formulas[formulaInd].Count; componentInd += 2)
                {
                    if (formulas[formulaInd][componentInd].ToLower().Contains(DIPLOMA) &&
                        hasMatricularityGrade(formulas[formulaInd][componentInd], matricularityExams))
                    {
                        //we shouldn't allow diploma grading if matricularity grade is available
                        totalGrade = 0;
                        break;
                    }

                    if (!grades.TryGetValue(formulas[formulaInd][componentInd], out value))
                    {
                        totalGrade = 0;
                        break;
                    }

                    int weight = Int32.Parse(formulas[formulaInd][componentInd - 1]);
                    double grade = grades[formulas[formulaInd][componentInd]];

                    if (grade < MINIMAL_ALLOWED_GRADE)
                    {
                        totalGrade = 0;
                        break;
                    }

                    totalGrade += weight * grade;
                }

                if (totalGrade > maxGrade)
                    maxGrade = totalGrade;

                totalGrade = 0;

            }

            return maxGrade;
        }


        public void grade(String studentEGN, List<Preference> preferences)
        {
            grades = queryManager.getStudentGrades(studentEGN);

            //TODO: Should we check for exams with the same name but different date?

            foreach (Preference preference in preferences)
            {
                preference.TotalGrade = calculateTotalGrade(studentEGN, preference.ProgrammeName);

                //add total grade in preference table
                context.Preferences.Attach(preference);
                context.Entry(preference).Property(x => x.TotalGrade).IsModified = true;
                context.SaveChanges();
            }
        }
    }
}