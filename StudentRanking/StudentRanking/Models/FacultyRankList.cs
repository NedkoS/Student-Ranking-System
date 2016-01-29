using StudentRanking.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudentRanking.Models
{
    public class FacultyRankList
    {
        [Required]
        [Key]
        [Column(Order = 0)]
        [StudentRanking.Filters.Index("ProgrammeNameIndex", unique: false)]
        public String ProgrammeName { get; set; }

        [Required]
        [Key]
        [Column(Order = 1)]
        public String EGN { get; set; }

        [Required]
        public Double TotalGrade { get; set; }
    }
}