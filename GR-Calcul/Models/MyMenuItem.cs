using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class MyMenuItem
{
    public string LinkText { get; set; }
    public string ActionName { get; set; }
    public string ControllerName { get; set; }

    public MyMenuItem(string link, string action, string controller)
    {
        LinkText = link;
        ActionName = action;
        ControllerName = controller;
    }
}
