using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;

namespace System.Web.Mvc.Html
{
    public class SelectListItemGroup : SelectListItem
    {
        public SelectListItemGroup()
        {
        }

        public string Class { get; set; }
        public string Group { get; set; }
        public string Style { get; set; }
        public bool Disabled { get; set; }
    }
}