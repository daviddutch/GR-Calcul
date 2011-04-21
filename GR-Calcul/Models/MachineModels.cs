using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace GR_Calcul.Models
{   
    public class MachineModel
    {
        // name
        [Required]
        [Display(Name = "Nom de la machine")]
        public string Name { get; set; }

        // IP
        [Required]
        [RegularExpression(@"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)",
           ErrorMessage = "Donnez une adresse IPv4 valide.")]
        [Display(Name = "Adresse IP de la machine")]
        public string IP { get; set; }
    }

    //public class DeleteMachineModel
    //{

    //}

    // These next lines don't work...

    //public class MachineDbContext : DbContext
    //{
    //    public DbSet<MachineModel> Machines { get; set; }
    //}
}