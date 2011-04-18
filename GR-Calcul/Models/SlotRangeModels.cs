using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GR_Calcul.Models
{
    public class SlotRangeModels
    {
        [Required]
        [Display(Name = "Nom")]
        [StringLength(20)]
        public string Name { get; set; }

        
        [Required]
        [Display(Name = "Début de Réservation")]
        [DataType(DataType.Date)]
        public DateTime StartRes { get; set; }
        
        [Required]
        [Display(Name = "Fin de Réservation")]
        [DataType(DataType.Date)]        
        public DateTime EndRes { get; set; }

    }


}