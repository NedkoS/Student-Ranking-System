
using StudentRanking.Ranking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentRanking.Models
{
    [Authorize(Roles = "student")]
    public class StudentRankingInformationController : Controller
    {
        //
        // GET: /StudentRankingInformation/
        private QueryManager queryManager = QueryManager.getInstance();
        private Dictionary<String, List<String>> programmes = new Dictionary<String, List<String>>();
        private List<StudentRankingInformation> model = new List<StudentRankingInformation>();

        private List<String> getProgrammes(List<FacultyRankList> l)
        {
            List<String> result = new List<String>();
            foreach (var item in l)
            {
                result.Add(item.ProgrammeName);
            }
            return result;
        }

        //I found that User works, ie. User.Identity.Name or User.IsInRole("Administrator")...
        public ActionResult Index()
        {

            
            String user = User.Identity.Name;
            ViewData["userName"] = user;
            QueryManager manager = QueryManager.getInstance();

            bool isEnrolled = false;
            Student st = manager.findStudent(user);
            if ( st != null )
            {
                isEnrolled = st.IsEnrolled;
            }

            List<FacultyRankList> rankList = manager.getStudentRankList(user);

            ViewData["enrolledProgramme"] = "";
            ViewData["faculty"] = "";
            if (isEnrolled)
            {
                ViewData["isRankListPublished"] = true;
                ViewData["isEnrolled"] = true;
                if (rankList.Count() == 1)
                {
                    ViewData["enrolledProgramme"] = rankList.First().ProgrammeName;
                    Faculty f = manager.getFaculty((String)ViewData["enrolledProgramme"]);
                    ViewData["faculty"] = f.FacultyName;
                }
                return View(model);
            }

            ViewData["isEnrolled"] = false;

            QueryManager queryManager = QueryManager.getInstance();

            List<FacultyRankList> studentRankList = queryManager.getStudentRankList(user);

            List<String> l = getProgrammes(studentRankList);
            l.Insert(0, "Please Select");
            SelectList pr = new SelectList(l);

            ViewData["programmes"] = pr;

            //bool hasFacultyRankListEntries = (db.FacultyRankLists.ToList().Count() != 0 ) ? true : false;

            //ViewData["isRankListPublished"] = false;
            //if (hasFacultyRankListEntries)
            //{
            //    ViewData["isRankListPublished"] = true;
            //}



            QueryManager mng = QueryManager.getInstance();

            // класиране първи етап - дати
            ViewData["isFirstRankListPublished"] = false;
            if (mng.getRankingDatesContent().Count != 0 && mng.getRankingDatesContent().Last().FirstRankingDate == "true")
            {
                ViewData["isFirstRankListPublished"] = true;
            }


            // класиране втори етап - дати
            ViewData["isSecondRankListPublished"] = false;
            if (mng.getRankingDatesContent().Count != 0 && mng.getRankingDatesContent().Last().FirstRankingDate == "true")
            {
                ViewData["isSecondRankListPublished"] = true;
            }

            // класиране трети етап - дати
            ViewData["isThirdRankListPublished"] = false;
            if (mng.getRankingDatesContent().Count != 0 && mng.getRankingDatesContent().Last().FirstRankingDate == "true")
            {
                ViewData["isThirdRankListPublished"] = true;
            }

            List<String> studentProgrammes = new List<String>();
            foreach (var item in studentRankList)
            {
                studentProgrammes.Add(item.ProgrammeName);
            }

            Dictionary<String, int> prefNumbers = new Dictionary<String, int>();
            foreach (var item in studentProgrammes)
            {
                int number = queryManager.getPrefNumber(user, item);
                prefNumbers.Add(item, number);
            }

            foreach (var item in studentRankList)
            {
                String faculty = queryManager.getFaculty(item.ProgrammeName).FacultyName;
                StudentRankingInformation r = new StudentRankingInformation
                {
                    FacultyName = faculty,
                    FinalResult = item.TotalGrade,
                    ProgrammeName = item.ProgrammeName,
                    PrefNumber = prefNumbers[item.ProgrammeName]
                };
                model.Add(r);
            }
            ViewData["result"] = model;

           // model.Add(new StudentRankingInformation { FacultyName = "FMI", FinalResult = 22.4, PrefNumber = 1, ProgrammeName = "KN" });
            return View(model);
        }

        [HttpPost]
        public ActionResult EnrollStudent(String programmeName)
        {
            String user = User.Identity.Name;
            ViewData["userName"] = user;
            Student st = queryManager.findStudent(user);

            st.IsEnrolled = true;

            queryManager.setStudentState(st, EntityState.Modified);
            ViewData["isEnrolled"] = true;

            QueryManager manager = QueryManager.getInstance();
            List<FacultyRankList> rankList = manager.getStudentRankList(user);

            foreach (var item in rankList)
            {
                if (item.ProgrammeName != programmeName)
                {
                    queryManager.removeFacultyRankListItem(item);
                }
            }

            
            //return RedirectToAction("Index", "StudentRankingInformation");
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", "StudentRankingInformation");
            return Json(new { Url = redirectUrl });
        }

    }
}
