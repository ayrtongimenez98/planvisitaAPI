﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.JefeVentas
{
    public class JefeVentasResponseModel
    {
        public int JefeVentas_Id { get; set; }
        public string JefeVentas_Nombre { get; set; }
        public int Division_Id { get; set; }
        public string JefeVentas_Mail { get; set; }
    }
}