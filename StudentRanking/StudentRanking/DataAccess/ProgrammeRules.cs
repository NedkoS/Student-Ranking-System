using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentRanking.DataAccess
{
    public class ProgrammeRules
    {
        [Required]
        [Key]
        public String ProgrammeName{ get; set; }

        [Required]
        public int MaleCount{ get; set; }

        [Required]
        public int FemaleCount{ get; set; }
    }
}