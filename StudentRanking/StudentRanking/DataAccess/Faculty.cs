using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentRanking.DataAccess
{
    public class Faculty
    {
        [Required]
        [Key]
        public String ProgrammeName{ get; set; }
        [Required]
        public String FacultyName{ get; set; }
    }
}