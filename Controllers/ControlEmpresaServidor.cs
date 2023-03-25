using Microsoft.AspNetCore.Mvc;
using BigDataJSN7.Modelo;
using System.Reflection;
using static BigDataJSN7.Modelo.DatosEmpresaServidor;

namespace BigDataJSN7.Controllers
{
    [Route("mc_EmpresaServidor")]
    [ApiController]
    public class ControlEmpresaServidor : ControllerBase
    {

        static string NombreServicio = ServiciosMC.mc_Sitios.ToString();
        static string[] Directorio = new string[] { NombreServicio };


        public DatosEmpresaServidor DatosEmpresaServidor;
        public Utilidades Utilidad;
        public ControlEmpresaServidor()
        {
            DatosEmpresaServidor = new DatosEmpresaServidor();
            Utilidad = new Utilidades();
        }

        #region "--------------+Get+-----------------"
        [HttpGet]
        public List<string[]> Get()
        {
            List<string[]> Lista = new List<string[]>();
            try
            {
                Lista.Add(new string[] { $"Modulo {NombreServicio} .Net7" });
            }
            catch { }
            return Lista;
        }
        #endregion+

        #region "--------+InsertarEmpresaServidor+----------"

        [HttpPost]
        [Route("InsertarEmpresaServidor")]
        public RespuestaEmpresaServidor InsertarEmpresaServidor([FromBody] ParametrosEmpresas Parametros)
        {
            RespuestaEmpresaServidor Objeto = new RespuestaEmpresaServidor() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };
            DatosEmpresaServidor Datos = new DatosEmpresaServidor();
            string ClaveServicio = DateTime.UtcNow.Ticks.ToString();
            string Token = Security.TacoSecurity.GeneraToken(1, 0);

            try
            {
                if (Datos.ObjServicio.HabilitarServicio)
                {
                    if (Parametros != null)
                    {
                        if (Parametros.IdUsuario > 0)
                        {
                            if (!string.IsNullOrEmpty(Parametros.Token))
                            {
                                if (Security.TacoSecurity.ValidarToken(Parametros.Token, Parametros.IdUsuario, 0))
                                {
                                    Objeto = Datos.InsertarEmpresaServidor(Parametros, ClaveServicio);
                                }
                                else
                                {
                                    Objeto.Estado = -1000;
                                    Objeto.Mensaje = "Error de token";
                                }
                            }
                            else
                            {
                                Objeto.Estado = -1000;
                                Objeto.Mensaje = "Error de parametros: El Token es requerido";
                            }
                        }
                        else
                        {
                            Objeto.Estado = -1001;
                            Objeto.Mensaje = "Error de parametros: El IdUsuario debe ser mayor a 0";
                        }
                    }
                    else
                    {
                        Objeto.Estado = -1001;
                        Objeto.Mensaje = "Error de parametros: los parametros son requeridos";
                    }
                }
                else
                {
                    Objeto.Estado = -1003;
                    Objeto.Mensaje = "Servicio no habilitado";
                }

            }
            catch (Exception Ex)
            {
                Objeto.Estado = -1;
                Objeto.Mensaje = "Error general Control";
                Utilidades.RegistrarError(DatosEmpresaServidor.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }

            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        Etiqueta = Parametros.int_idtabla
                    };

                    object ObjTabla = new
                    {
                        IdUsuario = Parametros.IdUsuario,
                        Token = Parametros.Token,
                    };
                    Datos.Utilidades.LogServicio(new List<string> { NombreServicio }, NombreServicio, MethodBase.GetCurrentMethod(), ClaveServicio, Objeto, ObjParametros, ObjTabla, Objeto.Estado);
                }
            }
            catch { }
            #endregion

            return Objeto;
        }

        #endregion

        #region "-------+EliminarEmpresaServidor+----------"
        [HttpPost]
        [Route("EliminarEmpresaServidor")]
        public RespuestaEmpresaServidor EliminarEmpresaServidor([FromBody] ParametrosDesactivarEmpresas Parametros)
        {
            RespuestaEmpresaServidor Objeto = new RespuestaEmpresaServidor() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };
            DatosEmpresaServidor Datos = new DatosEmpresaServidor();
            string ClaveServicio = DateTime.UtcNow.Ticks.ToString();
            string Token = Security.TacoSecurity.GeneraToken(1, 0);

            try
            {
                if (Datos.ObjServicio.HabilitarServicio)
                {
                    if (Parametros != null)
                    {
                        if (Parametros.IdUsuario > 0)
                        {
                            if (!string.IsNullOrEmpty(Parametros.Token))
                            {
                                if (Security.TacoSecurity.ValidarToken(Parametros.Token, Parametros.IdUsuario, 0))
                                {
                                    Objeto = Datos.EliminarEmpresaServidor(Parametros, ClaveServicio);
                                }
                                else
                                {
                                    Objeto.Estado = -1000;
                                    Objeto.Mensaje = "Error de token";
                                }
                            }
                            else
                            {
                                Objeto.Estado = -1000;
                                Objeto.Mensaje = "Error de parametros: El Token es requerido";
                            }
                        }
                        else
                        {
                            Objeto.Estado = -1001;
                            Objeto.Mensaje = "Error de parametros: El IdUsuario debe ser mayor a 0";
                        }
                    }
                    else
                    {
                        Objeto.Estado = -1001;
                        Objeto.Mensaje = "Error de parametros: los parametros son requeridos";
                    }
                }
                else
                {
                    Objeto.Estado = -1003;
                    Objeto.Mensaje = "Servicio no habilitado";
                }

            }
            catch (Exception Ex)
            {
                Objeto.Estado = -1;
                Objeto.Mensaje = "Error general Control";
                Utilidades.RegistrarError(DatosEmpresaServidor.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }

            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        Etiqueta = Parametros.int_id
                    };

                    object ObjTabla = new
                    {
                        IdUsuario = Parametros.IdUsuario,
                        Token = Parametros.Token,
                    };
                    Datos.Utilidades.LogServicio(new List<string> { NombreServicio }, NombreServicio, MethodBase.GetCurrentMethod(), ClaveServicio, Objeto, ObjParametros, ObjTabla, Objeto.Estado);
                }
            }
            catch { }
            #endregion

            return Objeto;
        }

        #endregion
    }

}

