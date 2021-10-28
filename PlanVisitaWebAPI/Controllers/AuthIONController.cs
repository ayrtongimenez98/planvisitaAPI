using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models.Authentication;
using PlanVisitaWebAPI.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PlanVisitaWebAPI.Controllers.JefeVentas
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuthIONController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        public HttpResponseMessage Post([FromBody] LoginSearchModel model)
        {
            var response = new HttpResponseMessage();
            if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Username))
            {
                var validation = new SystemValidationModel()
                {
                    Success = false,
                    Message = "Credenciales incorrectas."
                };
                var json = JsonConvert.SerializeObject(validation);
                response = Request.CreateResponse(HttpStatusCode.NotFound);
                response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            } else
            {
                var user = db.Usuario.FirstOrDefault(x => x.Usuario1 == model.Username && x.Usuario_Pass == model.Password);
                
                if (user != null)
                {
                    var usuarioLogin = new LoginResponseModel();
                    if (user.Usuario_Vendedor_Id != null)
                    {
                        var vendedor = db.Vendedor.First(x => x.Vendedor_Id == user.Usuario_Vendedor_Id);
                        usuarioLogin.Es_Jefe = false;
                        usuarioLogin.Nombre = vendedor.Vendedor_Nombre;
                        usuarioLogin.Usuario_Id = vendedor.Vendedor_Id;
                        usuarioLogin.Usuario = user.Usuario1;
                        usuarioLogin.Email = vendedor.Vendedor_Mail;
                        usuarioLogin.Id = user.Usuario_Id.ToString();
                    }
                    else
                    {
                        var jefeVentas = db.JefeVentas.First(x => x.JefeVentas_Id == user.JefeVentas_Id);
                        usuarioLogin.Es_Jefe = true;
                        usuarioLogin.Nombre = jefeVentas.JefeVentas_Nombre;
                        usuarioLogin.Usuario_Id = jefeVentas.JefeVentas_Id;
                        usuarioLogin.Usuario = user.Usuario1;
                        usuarioLogin.Email = jefeVentas.JefeVentas_Mail;
                        usuarioLogin.Id = user.Usuario_Id.ToString();
                    }

                    var json = JsonConvert.SerializeObject(usuarioLogin);
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                } else
                {
                    var validation = new SystemValidationModel()
                    {
                        Success = false,
                        Message = "Credenciales incorrectas."
                    };
                    var json = JsonConvert.SerializeObject(validation);
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                    response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                }
            }
            return response;
        }
    }
}
