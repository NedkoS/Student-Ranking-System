using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudentRanking.DataAccess
{
    public class Preference
    {
        [Required]
        [Key, Column(Order = 0)]
        public string EGN{ get; set; }

        [Required]
        public string ProgrammeName{ get; set; }

        [Required]
        [Key, Column(Order = 1)]
        public int PrefNumber{ get; set; }

        [DefaultValue(0)]
        public double TotalGrade { get; set; }
    }
}