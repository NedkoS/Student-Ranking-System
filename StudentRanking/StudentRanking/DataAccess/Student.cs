using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentRanking.DataAccess
{
    public class Student
    {
        [Required]
        public String FirstName { get; set; }

        [Required]
        public String LastName { get; set; }

        [Required]
        [Key]
        public String EGN { get; set; }

        [Required]
        public String Email { get; set; }

        [Required]
        public bool? Gender { get; set; }

        public Boolean IsEnrolled{ get; set; }
    }
}