using Microsoft.AspNetCore.Mvc;
using BigDataJSN7.Modelo;
using System.Reflection;
using static BigDataJSN7.Modelo.DatosTablas;

namespace BigDataJSN7.Controllers
{
    [Route("mc_Tablas")]
    [ApiController]
    public class ControlTablas : ControllerBase
    {

        static string NombreServicio = ServiciosMC.mc_Sitios.ToString();
        static string[] Directorio = new string[] { NombreServicio };


        public DatosTablas DatosTablas;
        public Utilidades Utilidad;
        public ControlTablas()
        {
            DatosTablas = new DatosTablas();
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

        #region "--------+InsertarTabla+----------"

        [HttpPost]
        [Route("InsertarTabla")]
        public RespuestaTablas InsertarTabla([FromBody] ParametrosTablas Parametros)
        {
            RespuestaTablas Objeto = new RespuestaTablas() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };
            DatosTablas Datos = new DatosTablas();
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
                                    Objeto = Datos.InsertarTabla(Parametros, ClaveServicio);
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
                Utilidades.RegistrarError(DatosTablas.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }

            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        Etiqueta = Parametros.var_nombre
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

        #region "-------+ModificarTabla+----------"
        [HttpPost]
        [Route("ModificarTabla")]
        public RespuestaTablas ModificarTabla([FromBody] ParametrosEditarTablas Parametros)
        {
            RespuestaTablas Objeto = new RespuestaTablas() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };
            DatosTablas Datos = new DatosTablas();
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
                                    Objeto = Datos.ModificarTabla(Parametros, ClaveServicio);
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
                Utilidades.RegistrarError(DatosTablas.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }

            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        Etiqueta = Parametros.var_nombre
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

        #region "-------+DesactivarTabla+----------"
        [HttpPost]
        [Route("DesactivarTabla")]
        public RespuestaTablas DesactivarTabla([FromBody] ParametrosDesactivarTablas Parametros)
        {
            RespuestaTablas Objeto = new RespuestaTablas() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };
            DatosTablas Datos = new DatosTablas();
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
                                    Objeto = Datos.DesactivarTabla(Parametros, ClaveServicio);
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
                Utilidades.RegistrarError(DatosTablas.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
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

