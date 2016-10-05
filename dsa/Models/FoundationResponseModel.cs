using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dsa.Models
{
    /* 'Select ORG_ID, ORG_NAME, MEMSTATUS, ORG_COF_STATUS, ORG_CLASS, CONGRESSIONAL, ADDRESS_1, ADDRESS_2, ADDRESS_3, CITY, STATE, POSTAL_CODE, COUNTRY, ACCREDITED '     */

    public class FoundationResponseModel
    {
        [Key]
        public int ORG_ID { get; set; }
        public string ORG_NAME { get; set; }
        public string MEMSTATUS { get; set; }
        public string ORG_COF_STATUS { get; set; }
        public string ORG_CLASS { get; set; }
        public string CONGRESSIONAL { get; set; }
        public string ADDRESS_1 { get; set; }
        public string ADDRESS_2 { get; set; }
        public string ADDRESS_3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string POSTAL_CODE { get; set; }
        public string COUNTRY { get; set; }
        public string ACCREDITED { get; set; }

    }
}