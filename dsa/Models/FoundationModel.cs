using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dsa.Models
{

    /*
     * 'Select ORG_ID, ORG_NAME, MEMSTATUS, ORG_COF_STATUS, ORG_CLASS, CONGRESSIONAL, ADDRESS_1, ADDRESS_2, ADDRESS_3, CITY, STATE, POSTAL_CODE, COUNTRY, ACCREDITED '

     */

    public class FoundationModel
    {
        [Key]
        public int Id { get; set; }
        public string OrgName { get; set; }
        public string OrgClass { get; set; }
        public string CouncilStatus { get; set; }
        public string CFNSB { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string District { get; set; }

    }
}