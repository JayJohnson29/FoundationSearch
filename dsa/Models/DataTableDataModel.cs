using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dsa.Models
{
    public class DataTableDataModel
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<FoundationModel> data { get; set; }
    }
}