using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentRanking.Models
{
    public class ExamName
    {
        [Key]
        public String Name { get; set; }
    }
}