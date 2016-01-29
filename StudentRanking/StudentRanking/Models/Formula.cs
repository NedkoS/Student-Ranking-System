using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudentRanking.Models
{
    public class Formula
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public String ProgrammeName{ get; set; }

        public double C1{ get; set; }
        public String X { get; set; }
        public double C2 { get; set; }
        public String Y { get; set; }
        public double C3 { get; set; }
        public String Z { get; set; }
        public double C4 { get; set; }
        public String W { get; set; }
    }
}