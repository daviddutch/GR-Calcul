using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GR_Calcul.Models
{

    public class SlotRangeModels
    {

        public static IEnumerable<SelectListItem> blabla = new[] { 
                new SelectListItem() { Value = "1", Text = "1" },
                new SelectListItem() { Value = "2", Text = "2" }, 
                new SelectListItem() { Value = "3", Text = "3" }, 
                new SelectListItem() { Value = "4", Text = "4" }, 
                new SelectListItem() { Value = "6", Text = "6" }, 
                new SelectListItem() { Value = "8", Text = "8" }
            };
                
        public SlotRangeModels()
        {
            StartRes = DateTime.Now;
            EndRes = DateTime.Now;
            Beginning = DateTime.Now;
            SlotDuration = blabla;
        }
        
    
        //step1 data
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

        public List<Machine> Machines { get; set; }

        //step2 data
        [Required]
        [Display(Name = "Durée d'un créneau")]
        public IEnumerable<SelectListItem> SlotDuration { get; set; }


        [Required]
        [Display(Name = "Nombre de créneaux total")]
        public short NumberOfSlots { get; set; }

        [Required]
        [Display(Name = "Date de départ")]
        [DataType(DataType.Date)]
        public DateTime Beginning { get; set; }

        //step3 data
        public List<SlotModel> Slots { get; set; }

    }

    public class SlotModel
    {
        [Required]
        [Display(Name = "Début du créneau")]
        [DataType(DataType.Date)]
        public DateTime Start { get; set; }

        [Required]
        [Display(Name = "Fin du créneau")]
        [DataType(DataType.Date)]
        public DateTime End { get; set; }
    }


}