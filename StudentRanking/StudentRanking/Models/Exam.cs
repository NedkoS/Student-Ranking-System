using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudentRanking.Models
{
    public class Exam
    {
        [Required]
        [Key][Column(Order=0)]
        public String ExamName{ get; set; }
        [Key][Column(Order = 1)]
        [Required]
        public String StudentEGN{ get; set; }

        [Required]
        public double Grade{ get; set; }
    }
}