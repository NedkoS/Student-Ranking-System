using StudentRanking.DataAccess;
using StudentRanking.Models;
using StudentRanking.Ranking;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace StudentRanking.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProgrammeRankListController : Controller
    {
        private UsersContext db = new UsersContext();
        private Dictionary<String, List<String>> programmes = new Dictionary<String, List<String>>();
        private List<FacultyRankList> model = new List<FacultyRankList>();
        //


        public ProgrammeRankListController()
        {
            var faculties = db.Faculties.ToList();

            foreach (var faculty in faculties)
            {
                if (!programmes.ContainsKey(faculty.FacultyName))
                {
                    List<String> specialities = new List<String>();
                    programmes.Add(faculty.FacultyName, specialities);
                    //specialities.Add(faculty.ProgrammeName);
                    programmes[faculty.FacultyName].Add(faculty.ProgrammeName);
                }
                else
                {
                    programmes[faculty.FacultyName].Add(faculty.ProgrammeName);
                }
            }
        }

        public JsonResult GetProgrammes(string faculty)
        {
            if (faculty != "Please Select")
                return Json(programmes[faculty]);
            else
                return Json(new List<String>());
        }




        // GET: /ProgrammeRankList/
        
        [HttpGet]
        public ActionResult Index()
        {

            UsersContext db = new UsersContext();
            QueryManager mng = new QueryManager(db);

            



            String user = User.Identity.Name;

            String s = mng.getCampaignDates().FirstRankingDate;
            // класиране първи етап - дати
            DateTime first = Convert.ToDateTime(mng.getCampaignDates().FirstRankingDate);
            ViewData["isFirstRankingDate"] = false;
            if (DateTime.Today >= first)
            {
                ViewData["isFirstRankingDate"] = true;
            }

            ViewData["isFirstRankListPublished"] = false;
            if (db.Dates.ToList().Count != 0 && db.Dates.ToList().Last().FirstRankingDate == "true")
            {
                ViewData["isFirstRankListPublished"] = true;
            }


            // класиране втори етап - дати
            DateTime second = Convert.ToDateTime(mng.getCampaignDates().SecondRankingDate);
            ViewData["isSecondRankingDate"] = false;
            if (DateTime.Today >= second)
            {
                ViewData["isSecondRankingDate"] = true;
            }

            ViewData["isSecondRankListPublished"] = false;
            if (db.Dates.ToList().Count != 0 && db.Dates.ToList().Last().FirstRankingDate == "true")
            {
                ViewData["isSecondRankListPublished"] = true;
            }

            // класиране трети етап - дати
            DateTime third = Convert.ToDateTime(mng.getCampaignDates().ThirdRankingDate);
            ViewData["isThirdRankingDate"] = false;
            if (DateTime.Today >= third)
            {
                ViewData["isThirdRankingDate"] = true;
            }

            ViewData["isThirdRankListPublished"] = false;
            if (db.Dates.ToList().Count != 0 &&  db.Dates.ToList().Last().FirstRankingDate == "true")
            {
                ViewData["isThirdRankListPublished"] = true;
            }



            ViewData["mainAdmin"] = false;
            if (user == "Admin")
            {
                ViewData["mainAdmin"] = true;
            }

            ViewData["userName"] = user;



            List<String> l = programmes.Keys.ToList<string>();
            l.Insert(0, "Please Select");
            SelectList faculties = new SelectList(l);

            ViewData["faculties"] = faculties;

            ViewData["result"] = model;

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(String faculty, String programmeName)
        {
            //генериране на combobox
            List<String> l = programmes.Keys.ToList<string>();
            l.Insert(0, "Please Select");
            SelectList faculties = new SelectList(l);
            ViewData["faculties"] = faculties;
            
            //проверка дали е настъпила дата за обявяване на класиране

            UsersContext db = new UsersContext();
            QueryManager mng = new QueryManager(db);

            // класиране първи етап - дати
            DateTime first = Convert.ToDateTime(mng.getCampaignDates().FirstRankingDate);
            ViewData["isFirstRankingDate"] = false;
            if (DateTime.Today >= first)
            {
                ViewData["isFirstRankingDate"] = true;
            }

            ViewData["isFirstRankListPublished"] = false;
            if (db.Dates.ToList().Count != 0 && db.Dates.ToList().Last().FirstRankingDate == "true")
            {
                ViewData["isFirstRankListPublished"] = true;
            }


            // класиране втори етап - дати
            DateTime second = Convert.ToDateTime(mng.getCampaignDates().SecondRankingDate);
            ViewData["isSecondRankingDate"] = false;
            if (DateTime.Today >= second)
            {
                ViewData["isSecondRankingDate"] = true;
            }

            ViewData["isSecondRankListPublished"] = false;
            if (db.Dates.ToList().Count != 0 &&  db.Dates.ToList().Last().FirstRankingDate == "true")
            {
                ViewData["isSecondRankListPublished"] = true;
            }

            // класиране трети етап - дати
            DateTime third = Convert.ToDateTime(mng.getCampaignDates().ThirdRankingDate);
            ViewData["isThirdRankingDate"] = false;
            if (DateTime.Today >= third)
            {
                ViewData["isThirdRankingDate"] = true;
            }

            ViewData["isThirdRankListPublished"] = false;
            if (db.Dates.ToList().Count != 0 && db.Dates.ToList().Last().FirstRankingDate == "true")
            {
                ViewData["isThirdRankListPublished"] = true;
            }



            //вземане на потребителското име на потребителя
            String user = User.Identity.Name;
            ViewData["userName"] = user;

            //проверка кой администратор е влязъл
            ViewData["mainAdmin"] = false;
            if (user == "Admin")
            {
                ViewData["mainAdmin"] = true;
            }

            QueryManager queryManager = new QueryManager(db);

            List<FacultyRankList> rankList = queryManager.getRankList(programmeName);

            foreach (var item in rankList)
            {
                FacultyRankList rank = new FacultyRankList
                {
                    ProgrammeName = programmeName,
                    EGN = item.EGN,
                    TotalGrade = item.TotalGrade
                };
                model.Add(rank);
            }

            //FacultyRankList f = new FacultyRankList
            //{
            //    EGN = "12345678",
            //    ProgrammeName = programmeName,
            //    TotalGrade = 4.5
            //};
            //model.Add(f);

            ViewData["result"] = model;
            
            return PartialView("_ProgrammeRankListTable", model);
        }

        public void onAlgoFinished()
        {
            int a = 2;
        }

        [HttpPost]
        public async Task<ActionResult> algoStart()
        {
            //Algo start
            Ranker ranker = new Ranker(db);
            ranker.setOnFinishListener(onAlgoFinished);
            await ranker.start();

            RankingDates dates = db.Dates.ToList().Last();

            
            if (dates.FirstRankingDate == "false")
            {
                dates.FirstRankingDate = "true";
                db.Entry(dates).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                if (dates.FirstRankingDate == "true" && dates.SecondRankingDate == "false")
                {
                    dates.SecondRankingDate = "true";
                    db.Entry(dates).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    if (dates.SecondRankingDate == "true" && dates.ThirdRankingDate == "false")
                    {
                        dates.ThirdRankingDate = "true";
                        db.Entry(dates).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

            }
            

            //return RedirectToAction("Index", "StudentRankingInformation");
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", "ProgrammeRankList");
            return Json(new { Url = redirectUrl });
        }

    }
}
