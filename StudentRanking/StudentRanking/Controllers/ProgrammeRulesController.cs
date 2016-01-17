﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentRanking.Models;
using StudentRanking.DataAccess;

namespace StudentRanking.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProgrammeRulesController : Controller
    {
        private RankingContext db = new RankingContext();
        private List<ProgrammeProperties> model = new List<ProgrammeProperties>();
        private Dictionary<String, List<String>> programmes = new Dictionary<String, List<String>>();

        //
        // GET: /ProgrammeRules/

        public ProgrammeRulesController()
            : base()
        {


            var faculties = db.Faculties.ToList();

            foreach (var faculty in faculties)
            {
                if (!programmes.ContainsKey(faculty.FacultyName))
                {
                    List<String> specialities = new List<String>();
                    programmes.Add(faculty.FacultyName, specialities);
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
            if (faculty != "Моля изберете")
                return Json(programmes[faculty]);
            else
                return Json(new List<String>());
        }

        [HttpPost]
        public ActionResult Index(string faculty, string programmeName)
        {
            List<String> l = programmes.Keys.ToList<string>();
            l.Insert(0, "Моля изберете");
            SelectList faculties = new SelectList(l);

            ViewData["faculties"] = faculties;

            model = getProgrammeRules(programmeName);

            return PartialView("_ProgrammePropertiesTable", model);
        }

        //[Authorize(Roles = "admin")]
        public ActionResult Index()
        {

            List<String> l = programmes.Keys.ToList<string>();
            l.Insert(0, "Моля изберете");
            SelectList faculties = new SelectList(l);

            ViewData["faculties"] = faculties;


            return View(model);
        }

        [HttpPost]
        public void SaveCounts(int maleCount, int femaleCount, string programmeName)
        {
            //ako crashnat int-ovete napravi si go sys string parametri
            var rule = db.ProgrammesRules.Find(programmeName);
            if (rule == null)
            {
                db.ProgrammesRules.Add(new ProgrammeRules()
                {
                    FemaleCount = femaleCount,
                    MaleCount = maleCount,
                    ProgrammeName = programmeName
                });
                db.SaveChanges();
                return;
            }
            rule.MaleCount = maleCount;
            rule.FemaleCount = femaleCount;
            db.Entry(rule).State = EntityState.Modified;
            db.SaveChanges();

        }


        private List<ProgrammeProperties> getProgrammeRules(String programmeName)
        {
            List<ProgrammeProperties> result = new List<ProgrammeProperties>();
            var query = from formula in db.Formulas
                        where formula.ProgrammeName == programmeName
                        select formula;

            ProgrammeRules pr = db.ProgrammesRules.Find(programmeName);


            foreach (var formula in query)
            {
                ProgrammeProperties rule = new ProgrammeProperties();
                rule.MaleCount = pr.MaleCount;
                rule.FemaleCount = pr.FemaleCount;
                rule.ProgrammeName = programmeName;
                if (formula.C1 > 0)
                {
                    rule.C1 = formula.C1;
                    rule.X = formula.X;

                }

                if (formula.C2 > 0)
                {
                    rule.C2 = formula.C2;
                    rule.Y = formula.Y;
                }

                if (formula.C3 > 0)
                {
                    rule.C3 = formula.C3;
                    rule.Z = formula.Z;
                }

                if (formula.C4 > 0)
                {
                    rule.C4 = formula.C4;
                    rule.W = formula.W;
                }

                result.Add(rule);
            }

            return result;
        }

    }


}