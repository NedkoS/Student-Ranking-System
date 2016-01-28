using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentRanking.Models;
using WebMatrix.WebData;
using System.Web.Security;
using StudentRanking.Ranking;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;

namespace StudentRanking.Controllers
{
    [Authorize(Roles = "admin")]
    public class StudentBrowseController : Controller
    {
        private QueryManager queryManager = QueryManager.getInstance();

        //
        // GET: /StudentBrowse/

        public ActionResult Index(String egn = "")
        {

            //Ranker ranker = new Ranker(db);
            //ranker.start();


            //String text = String.Empty;

            //var roles = (SimpleRoleProvider)Roles.Provider;

            //foreach (String EGN in query)
            //{
            //    roles.AddUsersToRoles(new string[] { EGN }, new string[] { "student" });
            //}
            //foreach(String EGN in query)
            //{
            //    string newPassword = Membership.GeneratePassword(10, 0);
            //    Random rnd = new Random();
            //    newPassword = Regex.Replace(newPassword, @"[^a-zA-Z0-9]", m => rnd.Next(0, 10).ToString());
            //    ViewBag.Password = newPassword;
            //    WebSecurity.CreateUserAndAccount(EGN, ViewBag.Password);
            //    text += EGN + " " + newPassword + "\n";
            //}

            //System.IO.File.WriteAllText(@"D:\WriteText.txt", text);
            
            if (egn == "")
                return View(queryManager.getStudents());
            else
                return Details(egn);
        }

        //
        // GET: /StudentBrowse/

        //[HttpPost]
        //public ActionResult Index(String egn)
        //{

        //    return Details(egn);
        //}


        //
        // GET: /StudentBrowse/Details/5

        public ActionResult Details(string id = null)
        {
            Student student = queryManager.findStudent(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // GET: /StudentBrowse/Create

        public ActionResult Create()
        {
            ViewBag.Password = Membership.GeneratePassword(10, 0);
            String pass = ViewBag.Password;

            string newPassword = Membership.GeneratePassword(10, 0);
            Random rnd = new Random();
            newPassword = Regex.Replace(newPassword, @"[^a-zA-Z0-9]", m => rnd.Next(0, 10).ToString());
            ViewBag.Password = newPassword;

            return View();
        }

        //
        // POST: /StudentBrowse/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {

                queryManager.addStudent(student);
                string newPassword = Membership.GeneratePassword(10, 0);
                Random rnd = new Random();
                newPassword = Regex.Replace(newPassword, @"[^a-zA-Z0-9]", m => rnd.Next(0, 10).ToString());
                ViewBag.Password = newPassword;

                String pass = ViewBag.Password;
                WebSecurity.CreateUserAndAccount(student.EGN, ViewBag.Password);

                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                MailMessage mail = new MailMessage();
                mail.To.Add(new MailAddress(student.Email));
                mail.From = new MailAddress("mailsender39@gmail.com");
                mail.Subject = "Your email subject";
                mail.Body = string.Format(body, "admin",
                                                   "mailsender39@gmail.com", newPassword); ;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials =
                     new System.Net.NetworkCredential("mailsender39@gmail.com", "stefito15");
                smtp.Send(mail);

                MembershipUser user = Membership.GetUser(student.EGN);

                var roles = (SimpleRoleProvider)Roles.Provider;

                if (!roles.RoleExists("admin"))
                    roles.CreateRole("admin");

                if (!roles.RoleExists("student"))
                    roles.CreateRole("student");

                roles.AddUsersToRoles(new string[] { student.EGN }, new string[] { "student" });
            }

            return RedirectToAction("Create", "StudentBrowse");
            //return View(student);
        }

        //
        // GET: /StudentBrowse/Edit/5

        public ActionResult Edit(string id = null)
        {
            Student student = queryManager.findStudent(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // POST: /StudentBrowse/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                queryManager.setStudentState(student, EntityState.Modified);
                return RedirectToAction("Index");
            }
            return View(student);
        }

        //
        // GET: /StudentBrowse/Delete/5

        public ActionResult Delete(string id = null)
        {
            Student student = queryManager.findStudent(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            Roles.RemoveUserFromRole(student.EGN, "student");
            Membership.DeleteUser(student.EGN);

            return View(student);
        }

        //
        // POST: /StudentBrowse/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Student student = queryManager.findStudent(id);
            queryManager.removeStudent(student);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            queryManager.refresh();
            base.Dispose(disposing);
        }
    }
}