using System;
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
    public class ExamController : Controller
    {
        private RankingContext db = new RankingContext();

        //
        // GET: /Exam/

        [HttpGet]

        public ActionResult Index()
        {
            return View( new List<Exam>());
        }
        [HttpPost]
        public ActionResult Index(String egn)
        {
            var exams = from exam in db.Exams
                        where exam.StudentEGN == egn
                        select exam;
            return PartialView("_StudentExamsGrades", exams);
        }

        //
        // GET: /Exam/Details/5

        public ActionResult Details(string examName = null, string studentEGN = null)
        {
            Exam exam = db.Exams.Find(examName, studentEGN);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        //
        // GET: /Exam/Create

        public ActionResult Create()
        {
            //List<String> programmes = new List<String>();

            //programmes.Add("Математика Матура");
            //programmes.Add("Математика Диплома");
            //programmes.Add("Математика 1");
            //programmes.Add("Математика 2");

            var examsNames = from exam in db.ExamNames
                             select exam.Name;


            ViewData["currentExamName"] = new SelectList(examsNames.ToList());
            return View();
        }

        //
        // POST: /Exam/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Exam exam)
        {
            var examsNames = from exam2 in db.ExamNames
                             select exam2.Name;


            ViewData["currentExamName"] = new SelectList(examsNames.ToList());

            if (ModelState.IsValid)
            {
                db.Exams.Add(exam);
                db.SaveChanges();
                return RedirectToAction("Create", "Exam");
            }

            return View(exam);
        }

        //
        // GET: /Exam/Edit/5

        public ActionResult Edit(string examName = null, string studentEGN = null)
        {
            Exam exam = db.Exams.Find(examName, studentEGN);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        //
        // POST: /Exam/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Exam exam)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exam).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(exam);
        }

        //
        // GET: /Exam/Delete/5

        public ActionResult Delete(string examName = null, string studentEGN = null)
        {
            Exam exam = db.Exams.Find(examName,studentEGN);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        //
        // POST: /Exam/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string examName = null, string studentEGN = null)
        {
            Exam exam = db.Exams.Find(examName, studentEGN);
            db.Exams.Remove(exam);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public object egn { get; set; }
    }
}