using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentRanking.DataAccess
{
    public class ExamName
    {
        [Key]
        public String Name { get; set; }
    }
}