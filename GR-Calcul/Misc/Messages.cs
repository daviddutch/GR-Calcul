using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GR_Calcul.Misc
{
    public class Messages
    {
        public const String errProd = "une erreur s'est produite!";
        public const String retList = "Retour à la liste";
        public const String recommencerDelete = "Il se peut que ces données aient été modifiées entre temps. Veuillez contrôler les données et reessayer.";
        public const String recommencerEdit = "Il se peut que ces données aient été modifiées entre temps. Veuillez contrôler les données et recommencer.";
        public const String invalidData = "Vous avez envoyé des données invalides";
        public const String uniqueUserEmail = "Il y a eu un problème à l'insertion. Veuillez vérifier qu'aucun utilisateur n'existe avec le même nom d'utilisateur ou la même adresse email.";
        public const String slotReserved = "Ce slot est déjà reservé !";
    }
}