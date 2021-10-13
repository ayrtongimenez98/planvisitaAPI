using PlanVisitaWebAPI.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models
{
    public class ClienteResponseModel
    {
        public Cliente Cliente { get; set; }
        public PaginationModel<DB.Sucursal> Sucursales { get; set; }

    }
}