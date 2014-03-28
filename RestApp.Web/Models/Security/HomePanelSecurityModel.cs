using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestApp.Web.Models.Security
{
    public class HomePanelSecurityModel
    {
        public bool PanelSedronar { get; set; }

        public bool PanelRenar { get; set; }

        public bool PanelBank { get; set; }

        public bool PanelAdministration { get; set; }
    }
}