using StudentRanking.DataAccess;
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
    [Authorize(Roles = "admin")]
    public class CampaignRankingDatesController : Controller
    {
        //
        // GET: /RankingDates/

        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CampaignRankingDates ranking)
        {
            
            RankingDates dates = new RankingDates
            {
                FirstRankingDate = ranking.FirstRankingDate.ToString(),
                PreferrencesFirstDate = ranking.StudentPreferenceFirstDate.ToString(),
                PreferrencesLastDate = ranking.StudentPreferenceLastDate.ToString(),
                SecondRankingDate = ranking.SecondRankingDate.ToString(),
                ThirdRankingDate = ranking.ThirdRankingDate.ToString()
            };

            RankingDates flags = new RankingDates
            {
                FirstRankingDate = "false",
                PreferrencesFirstDate = "false",
                PreferrencesLastDate = "false",
                SecondRankingDate = "false",
                ThirdRankingDate = "false"
            };

            QueryManager queryManager = QueryManager.getInstance();

            List<RankingDates> rankingDates = queryManager.getRankingDatesContent();
            if (rankingDates.Count() != 0)
            {

                RankingDates p = rankingDates.First();
                RankingDates q = rankingDates.Last();

                queryManager.removeRankingDates(p);
                queryManager.removeRankingDates(q);
            }


            queryManager.addRankingDatesContent(dates);

            queryManager.addRankingDatesContent(flags);

            return RedirectToAction("Menu", "Admin");  
        }
    }
}
