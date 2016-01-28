
using StudentRanking.Models;
using StudentRanking.Ranking;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace StudentRanking.Controllers
{
    [Authorize(Roles = "student")]
    public class StudentPreferencesController : Controller
    {
        //
        // GET: /StudentPreferences/
        private QueryManager queryManager = QueryManager.getInstance();
        private Dictionary<String, List<String>> programmes = new Dictionary<String, List<String>>();
        private List<StudentPreferences> model = new List<StudentPreferences>();

        public StudentPreferencesController()
        {
            //List<String> p1 = new List<String>();
            //p1.Add("KN");
            //p1.Add("Info");
            //p1.Add("IS");

            //List<String> p2 = new List<String>();
            //p2.Add("ikonomika");
            //p2.Add("Selsko stopanstvo");

            //List<String> p3 = new List<String>();
            //p3.Add("Biologiq");
            //p3.Add("Biotehnologii");
            //p3.Add("Molekulqrna");

            //List<String> faculties = new List<String>();
            //faculties.Add("FMI");
            //faculties.Add("Stopanski");
            //faculties.Add("Bilogicheski");

            //programmes.Add(faculties[0], p1);
            //programmes.Add(faculties[1], p2);
            //programmes.Add(faculties[2], p3);

            //List<Faculty> faculties = new List<Faculty>();
            ////faculties = db.Faculties.ToList();

            var faculties = queryManager.getFaculties();

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

        [HttpGet]
        public ActionResult Index()
        {
            String user = User.Identity.Name;
            //user = "Evgeny";
            ViewData["userName"] = user;

            List<String> l = programmes.Keys.ToList<string>();
            l.Insert(0,"Please Select");
            SelectList faculties = new SelectList(l);
       
            ViewData["faculties"] = faculties;

            QueryManager mng = QueryManager.getInstance();

            DateTime end = Convert.ToDateTime(mng.getCampaignDates().PreferrencesLastDate);
            ViewData["isAddingPreferencesEnd"] = false;
            if (DateTime.Today > end)
            {
                ViewData["isAddingPreferencesEnd"] = true;
            }



            QueryManager queryManager = QueryManager.getInstance();

            List<Preference> studentPreferences = queryManager.getStudentPreferences(user);

            foreach (var preff in studentPreferences)
            {
                Faculty f = queryManager.getFaculty(preff.ProgrammeName);
                String fac = (f != null ) ? f.FacultyName : "";
                StudentPreferences pr = new StudentPreferences
                {
                    Faculty = fac,
                    ProgrammeName = preff.ProgrammeName,
                    PrefNumber = preff.PrefNumber
                };
                model.Add(pr);
            }
            ViewData["result"] = model;

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(String faculty, String programmeName)
        {
            String user = User.Identity.Name;
            ViewData["userName"] = user;

            QueryManager mng = QueryManager.getInstance();

            DateTime finale = Convert.ToDateTime(mng.getCampaignDates().PreferrencesLastDate); 
            

            ViewData["isAddingPreferencesEnd"] = true;

            if (DateTime.Today > finale)
            {
                ViewData["isAddingPreferencesEnd"] = true;
            }


            List<String> l = programmes.Keys.ToList<string>();
            l.Insert(0, "Please Select");
            SelectList faculties = new SelectList(l);

            ViewData["faculties"] = faculties;

            

            String egn = user;

            //int lastPreferenceNumber = db.Preferences.Where(t => t.EGN == egn )
            //                                         .OrderByDescending(t => t.PrefNumber)
            //                                         .FirstOrDefault().PrefNumber;

            QueryManager queryManager = QueryManager.getInstance();

            List<Preference> studentPreferences = queryManager.getStudentPreferences(egn);

            bool isPreferrenceRepeated = false;
            foreach (var preff in studentPreferences)
            {
                String fac = queryManager.getFaculty(preff.ProgrammeName).FacultyName;
                StudentPreferences pr = new StudentPreferences { Faculty = fac, ProgrammeName = preff.ProgrammeName,
                                                                 PrefNumber = preff.PrefNumber };
                if (faculty == fac && preff.ProgrammeName == programmeName)
                {
                    isPreferrenceRepeated = true;
                }

                model.Add(pr);
            }

            
           
            if (!isPreferrenceRepeated)
            {
                int nextPrefenceNumber = (studentPreferences.Count() != 0) ? studentPreferences.Max(t => t.PrefNumber) + 1 : 1;
                StudentPreferences pref = new StudentPreferences
                {
                    Faculty = faculty,
                    ProgrammeName = programmeName,
                    PrefNumber = nextPrefenceNumber
                };
                Preference p = new Preference
                {
                    EGN = egn,
                    PrefNumber = nextPrefenceNumber,
                    ProgrammeName = programmeName,
                    TotalGrade = 0
                };

                queryManager.addPreference(p);
                model.Add(pref);
            }
            
            ViewData["result"] = model;



            return PartialView("_StudentPreferencesTable", model);
            //return View("Index",model);
        }

        public ActionResult deleteLastPreference()
        {
            QueryManager queryManager = QueryManager.getInstance();

            List<Preference> studentPreferences = queryManager.getStudentPreferences(User.Identity.Name);
            if (studentPreferences.Count() > 0 )
            {
                Preference p = studentPreferences.Last();
                queryManager.removePreference(p);

            }

            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", "StudentPreferences");
            return Json(new { Url = redirectUrl });
        }


    }
}
