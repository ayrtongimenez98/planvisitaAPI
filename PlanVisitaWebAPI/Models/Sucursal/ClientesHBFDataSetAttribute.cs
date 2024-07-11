using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Sucursal
{
    public class ClientesHBFDataSetAttribute
    {
        public string cardcode { get; set; }
        public string cardfname { get; set; }
        public string Address { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public Nullable<int> GroupCode { get; set; }
        public string GroupName { get; set; }
        public string Address2 { get; set; }
        public string Estado { get; set; }
    }
}