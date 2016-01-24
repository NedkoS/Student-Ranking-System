using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentRanking.Models;
using StudentRanking.DataAccess;
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
        private UsersContext db = new UsersContext();

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
                return View(db.Students.ToList());
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
            Student student = db.Students.Find(id);
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
                
                db.Students.Add(student);
                db.SaveChanges();

                string newPassword = Membership.GeneratePassword(10, 0);
                Random rnd = new Random();
                newPassword = Regex.Replace(newPassword, @"[^a-zA-Z0-9]", m => rnd.Next(0, 10).ToString());
                ViewBag.Password = newPassword;

                String pass = ViewBag.Password;
                WebSecurity.CreateUserAndAccount(student.EGN, ViewBag.Password);
                //WebSecurity.Login(student.EGN, ViewBag.Password);

                //var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                //var message = new MailMessage();
                //message.To.Add(new MailAddress("evgenistefchov@abv.bg")); //replace with valid value
                //message.From = new MailAddress("flood1@abv.bg");
                //message.Subject = "Your email subject";
                //message.Body = string.Format(body, "admin",
                //                                   "flood1@abv.bg", newPassword);
                //message.IsBodyHtml = true;
                //var smtp = new SmtpClient();

                //var credential = new NetworkCredential
                //{
                //    UserName = "flood1@abv.bg",  // replace with valid value
                //    Password = "123456789"  // replace with valid value
                //};
                //smtp.Credentials = credential;
                //smtp.Host = "smtp.abv.bg";
                //smtp.Port = 587;
                //smtp.EnableSsl = true;

                ////smtp.SendMailAsync(message);
                //smtp.Send(message);
//this
                //var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                //MailMessage mail = new MailMessage();
                //mail.To.Add(new MailAddress("evgenistefchov@abv.bg"));
                //mail.From = new MailAddress("flood1@abv.bg");
                //mail.Subject = "Your email subject";
                //mail.Body = string.Format(body, "admin",
                //                                   "flood1@abv.bg", newPassword); ;
                //mail.IsBodyHtml = true;
                //SmtpClient smtp = new SmtpClient("smtp.abv.bg", 587);
                //smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                //smtp.Credentials =
                //     new System.Net.NetworkCredential("flood1@abv.bg", "123456789");
                //smtp.Send(mail);



                //using (var smtp = new SmtpClient())
                //{
                //    await smtp.SendMailAsync(message);
                //    //return RedirectToAction("Sent");
                //}

                MembershipUser user = Membership.GetUser(student.EGN);
                //user.GetPassword();

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
            Student student = db.Students.Find(id);
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
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        //
        // GET: /StudentBrowse/Delete/5

        public ActionResult Delete(string id = null)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            Membership.DeleteUser(student.EGN);

            return View(student);
        }

        //
        // POST: /StudentBrowse/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}