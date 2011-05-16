using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GR_Calcul.Misc
{
    public class GrException : Exception
    {
        public string UserMessage { get; set; }

        public GrException(string userMessage)
        {
            this.UserMessage = userMessage;
        }

        public GrException(Exception e, string userMessage) : base("", e)
        {
            this.UserMessage = userMessage;
        }
    }
}