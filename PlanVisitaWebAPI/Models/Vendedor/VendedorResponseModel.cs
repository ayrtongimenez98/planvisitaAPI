using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models.Canal;

namespace PlanVisitaWebAPI.Models.Vendedor
{
    public class VendedorResponseModel
    {
        public DB.Vendedor Vendedor { get; set; }
    }
}