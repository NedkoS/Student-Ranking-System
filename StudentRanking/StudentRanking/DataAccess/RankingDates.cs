using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudentRanking.DataAccess
{
    public class RankingDates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public String FirstRankingDate { get; set; }
        public String SecondRankingDate { get; set; }
        public String ThirdRankingDate { get; set; }
        public String PreferrencesFirstDate { get; set; }
        public String PreferrencesLastDate { get; set; }
    }
}