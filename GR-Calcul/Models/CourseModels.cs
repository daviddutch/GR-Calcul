using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GR_Calcul.Models
{
    public class CreateCourseModel
    {
        [Required]
        [Display(Name = "Nom")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Clef")]
        public string Key { get; set; }

        [Required]
        [Display(Name = "Actif")]
        public bool Email { get; set; }

        [Required]
        [Display(Name = "Responsable")]
        public string Responsible { get; set; }
    }
    public class CourseModels
    {
    }
}