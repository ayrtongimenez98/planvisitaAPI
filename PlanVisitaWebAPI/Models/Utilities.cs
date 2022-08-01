using System;
using Newtonsoft.Json;
using PlanVisitaWebAPI.Models.Sucursal;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace PlanVisitaWebAPI.Models
{
    // Modelos usados como parámetros para las acciones de AccountController.

    public static class Utilities
    {

        public static List<SucursalVendedorResponseModel> CompararSeparar(List<SucursalVendedorResponseModel> listaActual) {
            var list = new List<SucursalVendedorResponseModel>();
            var pivot = new SucursalVendedorResponseModel();
            string path = "C:\\Users\\admin\\Desktop\\log.txt";
            StreamWriter sw = new StreamWriter(path);
            //Write a line of text
            
            //Close the file
            
            foreach (var sucursalVendedor in listaActual) {
                pivot = sucursalVendedor;
                if (list.Count == 0)
                {
                    if (pivot.Sucursal_Direccion != null)
                        list.Add(pivot);
                }
                else
                {
                    if (pivot.Sucursal_Direccion != null) {
                        if (!list.Any(x => x.Cliente_Cod == pivot.Cliente_Cod)) {
                            list.Add(pivot);
                        } else
                        {
                            if (!ExisteEnLaLista(list, pivot.Sucursal_Direccion))
                            {
                                list.Add(pivot);
                            } else
                            {
                                sw.WriteLine($"Direccion: {pivot.Sucursal_Direccion}. Id: {pivot.Sucursal_Id}. Cod: {pivot.Cliente_Cod}");
                            }
                        }
                    }
                }
            }
            sw.Close();
            return list;
        }

        public static bool ExisteEnLaLista(List<SucursalVendedorResponseModel> listaActual, string sucursal) {
            var existe = false;
            var pivot = new SucursalVendedorResponseModel();
            foreach (var sucursalVendedor in listaActual)
            {
                var sucursalActual = sucursalVendedor.Sucursal_Direccion;
                sucursalActual = string.Join("", sucursalActual.Split('@', ',', '.', ';', '\'', '(', ')', '/'));
                var palabrasActual = sucursalActual.Split(' ').Where(x => x.Count() > 1);
                var sucursalComparar = string.Join("", sucursal.Split('@', ',', '.', ';', '\'', '(', ')', '/'));
                var palabrasComparar = sucursalComparar.Split(' ').Where(x => x.Count() > 1);
                var cantidadPalabrasComunes = palabrasActual.Intersect(palabrasComparar).Count();
                if(cantidadPalabrasComunes >= 3)
                {
                    existe = true;
                    
                }
            }
            return existe;
        }
    }

}
