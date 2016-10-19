using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dsa.Models
{
    public class CouncilFoundationModel
    {
        //public string OrgName { get; set; }
        public string CouncilStatus { get; set; }
        //public string CFNSB { get; set; }
        //public string Address1 { get; set; }
        //public string Address2 { get; set; }
        //public string City { get; set; }
        public string State { get; set; }
        public string District { get; set; }

        public List<SelectListItem> AllStates { get; set; }
        public List<SelectListItem> AllStatus { get; set; }
        public List<FoundationModel> Foundations { get; set; }
    }
}
