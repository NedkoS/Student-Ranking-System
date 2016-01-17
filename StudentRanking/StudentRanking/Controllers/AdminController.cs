using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentRanking.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        //
        // GET: /AdminMenu/

        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult ShowStudents()
        {
            return View();
        }

        public ActionResult ShowRanking()
        {
            return View();
        }

        public ActionResult SetRules()
        {
            return View();
        }
    }
}
