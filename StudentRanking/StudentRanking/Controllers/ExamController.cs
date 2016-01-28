using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentRanking.Models;
using StudentRanking.DataAccess;
using StudentRanking.Ranking;


namespace StudentRanking.Controllers
{
    [Authorize(Roles = "admin")]
    public class ExamController : Controller
    {
        QueryManager queryManager = QueryManager.getInstance();
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

            return PartialView("_StudentExamsGrades", queryManager.getExamsOfStudent(egn));
        }

        //
        // GET: /Exam/Details/5

        public ActionResult Details(string examName = null, string studentEGN = null)
        {
            Exam exam = queryManager.findExamOfStudent(examName, studentEGN);
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


            ViewData["currentExamName"] = new SelectList(queryManager.getExamNames());
            return View();
        }

        //
        // POST: /Exam/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Exam exam)
        {

            ViewData["currentExamName"] = new SelectList(queryManager.getExamNames());

            if (ModelState.IsValid)
            {
                queryManager.addExam(exam);
                return RedirectToAction("Create", "Exam");
            }

            return View(exam);
        }

        //
        // GET: /Exam/Edit/5

        public ActionResult Edit(string examName = null, string studentEGN = null)
        {
            Exam exam = queryManager.findExamOfStudent(examName, studentEGN);
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
                queryManager.setExamState(exam, EntityState.Modified);
                return RedirectToAction("Index");
            }
            return View(exam);
        }

        //
        // GET: /Exam/Delete/5

        public ActionResult Delete(string examName = null, string studentEGN = null)
        {
            Exam exam = queryManager.findExamOfStudent(examName,studentEGN);
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
            Exam exam = queryManager.findExamOfStudent(examName, studentEGN);
            queryManager.removeExam(exam);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            queryManager.refresh();
            base.Dispose(disposing);
        }

        public object egn { get; set; }
    }
}