using BigDataJSN7.Modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using static BigDataJSN7.Modelo.DatosFuncion;

namespace BigDataJSN7.Controllers
{
    [Route("mc_Funcion")]
    [ApiController]
    public class ControlFuncion : ControllerBase
    {
        static string NombreServicio = ServiciosMC.mc_Funcion.ToString();
        static string[] Directorio = new string[] { NombreServicio };

        #region "--------------+Get+-----------------"
        [HttpGet]
        public List<string[]> Get()
        {
            string ClaveServicio = DateTime.UtcNow.Ticks.ToString();
            List<string[]> Lista = new List<string[]>();
            try
            {
                Exception Ex = null;
                Lista.Add(new string[] { $"Modulo {NombreServicio} .Net7" });
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }
            catch { }
            return Lista;
        }
        #endregion

        #region "--------+NuevaFuncion+---------"
        [HttpPost("NuevaFuncion")]

        public RespuestaFuncion NuevaFuncion([FromBody] ParametrosFuncion Parametros)
        {
            RespuestaFuncion Respuesta = new RespuestaFuncion() { Resultado = 0, Mensaje = "Error de datos" };
            DatosFuncion Datos = new DatosFuncion();
            string ClaveServicio = DateTime.UtcNow.Ticks.ToString();

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
                                    Respuesta = Datos.NuevaFuncion(Parametros, ClaveServicio);
                                }
                                else
                                {
                                    Respuesta.Resultado = -1000;
                                    Respuesta.Mensaje = "Error de token";
                                }
                            }
                            else
                            {
                                Respuesta.Resultado = -1000;
                                Respuesta.Mensaje = "Error de parametros: El Token es requerido";
                            }
                        }
                        else
                        {
                            Respuesta.Resultado = -1001;
                            Respuesta.Mensaje = "Error de parametros: El IdUsuario debe ser mayor a 0";
                        }
                    }
                    else
                    {
                        Respuesta.Resultado = -1001;
                        Respuesta.Mensaje = "Error de parametros: los parametros son requeridos";
                    }
                }
                else
                {
                    Respuesta.Resultado = -1003;
                    Respuesta.Mensaje = "Servicio no habilitado";
                }

            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }
            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        Servicio = Parametros.IdServicio,
                        IdUsuarioRegistro = Parametros.IdUsuario,
                    };

                    object ObjDetalle = new
                    {
                        IdUsuario = Parametros.IdUsuario,
                        Token = Parametros.Token,
                    };
                    Datos.Utilidades.LogServicio(new List<string> { NombreServicio }, NombreServicio, MethodBase.GetCurrentMethod(), ClaveServicio, Respuesta, ObjParametros, ObjDetalle, Respuesta.Resultado);
                }
            }
            catch { }
            #endregion
            return Respuesta;
        }
        #endregion

        #region "--------+NuevoDetalle+---------"
        [HttpPost("NuevoDetalle")]

        public RespuestaFuncion NuevoDetalle([FromBody] ParametrosDetalle Parametros)
        {
            RespuestaFuncion Respuesta = new RespuestaFuncion() { Resultado = 0, Mensaje = "Error de datos" };
            DatosFuncion Datos = new DatosFuncion();
            string ClaveServicio = DateTime.UtcNow.Ticks.ToString();

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
                                    Respuesta = Datos.NuevoDetalle(Parametros, ClaveServicio);
                                }
                                else
                                {
                                    Respuesta.Resultado = -1000;
                                    Respuesta.Mensaje = "Error de token";
                                }
                            }
                            else
                            {
                                Respuesta.Resultado = -1000;
                                Respuesta.Mensaje = "Error de parametros: El Token es requerido";
                            }
                        }
                        else
                        {
                            Respuesta.Resultado = -1001;
                            Respuesta.Mensaje = "Error de parametros: El IdUsuario debe ser mayor a 0";
                        }
                    }
                    else
                    {
                        Respuesta.Resultado = -1001;
                        Respuesta.Mensaje = "Error de parametros: los parametros son requeridos";
                    }
                }
                else
                {
                    Respuesta.Resultado = -1003;
                    Respuesta.Mensaje = "Servicio no habilitado";
                }

            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }
            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        Servicio = Parametros.IdServicio,
                        IdUsuarioRegistro = Parametros.IdUsuario,
                    };

                    object ObjDetalle = new
                    {
                        IdUsuario = Parametros.IdUsuario,
                        Token = Parametros.Token,
                    };
                    Datos.Utilidades.LogServicio(new List<string> { NombreServicio }, NombreServicio, MethodBase.GetCurrentMethod(), ClaveServicio, Respuesta, ObjParametros, ObjDetalle, Respuesta.Resultado);
                }
            }
            catch { }
            #endregion
            return Respuesta;
        }
        #endregion

        #region "--------+EditarFuncion+---------"
        [HttpPost("EditarFuncion")]

        public RespuestaFuncion EditarFuncion([FromBody] ParametrosEditarFuncion Parametros)
        {
            RespuestaFuncion Respuesta = new RespuestaFuncion() { Resultado = 0, Mensaje = "Error de datos" };
            DatosFuncion Datos = new DatosFuncion();
            string ClaveServicio = DateTime.UtcNow.Ticks.ToString();

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
                                    Respuesta = Datos.EditarFuncion(Parametros, ClaveServicio);
                                }
                                else
                                {
                                    Respuesta.Resultado = -1000;
                                    Respuesta.Mensaje = "Error de token";
                                }
                            }
                            else
                            {
                                Respuesta.Resultado = -1000;
                                Respuesta.Mensaje = "Error de parametros: El Token es requerido";
                            }
                        }
                        else
                        {
                            Respuesta.Resultado = -1001;
                            Respuesta.Mensaje = "Error de parametros: El IdUsuario debe ser mayor a 0";
                        }
                    }
                    else
                    {
                        Respuesta.Resultado = -1001;
                        Respuesta.Mensaje = "Error de parametros: los parametros son requeridos";
                    }
                }
                else
                {
                    Respuesta.Resultado = -1003;
                    Respuesta.Mensaje = "Servicio no habilitado";
                }

            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }
            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        Servicio = Parametros.Nombre,
                        IdUsuarioRegistro = Parametros.IdUsuario,
                    };

                    object ObjDetalle = new
                    {
                        IdUsuario = Parametros.IdUsuario,
                        Token = Parametros.Token,
                    };
                    Datos.Utilidades.LogServicio(new List<string> { NombreServicio }, NombreServicio, MethodBase.GetCurrentMethod(), ClaveServicio, Respuesta, ObjParametros, ObjDetalle, Respuesta.Resultado);
                }
            }
            catch { }
            #endregion
            return Respuesta;
        }
        #endregion

        #region "--------+EditarDetalle+---------"
        [HttpPost("EditarDetalle")]

        public RespuestaFuncion EditarDetalle([FromBody] ParametrosEditarDetalle Parametros)
        {
            RespuestaFuncion Respuesta = new RespuestaFuncion() { Resultado = 0, Mensaje = "Error de datos" };
            DatosFuncion Datos = new DatosFuncion();
            string ClaveServicio = DateTime.UtcNow.Ticks.ToString();

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
                                    Respuesta = Datos.EditarDetalle(Parametros, ClaveServicio);
                                }
                                else
                                {
                                    Respuesta.Resultado = -1000;
                                    Respuesta.Mensaje = "Error de token";
                                }
                            }
                            else
                            {
                                Respuesta.Resultado = -1000;
                                Respuesta.Mensaje = "Error de parametros: El Token es requerido";
                            }
                        }
                        else
                        {
                            Respuesta.Resultado = -1001;
                            Respuesta.Mensaje = "Error de parametros: El IdUsuario debe ser mayor a 0";
                        }
                    }
                    else
                    {
                        Respuesta.Resultado = -1001;
                        Respuesta.Mensaje = "Error de parametros: los parametros son requeridos";
                    }
                }
                else
                {
                    Respuesta.Resultado = -1003;
                    Respuesta.Mensaje = "Servicio no habilitado";
                }

            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }
            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        DescripcionVersion = Parametros.DescripcionVersion,
                        IdUsuarioRegistro = Parametros.IdUsuario,
                    };

                    object ObjDetalle = new
                    {
                        IdUsuario = Parametros.IdUsuario,
                        Token = Parametros.Token,
                    };
                    Datos.Utilidades.LogServicio(new List<string> { NombreServicio }, NombreServicio, MethodBase.GetCurrentMethod(), ClaveServicio, Respuesta, ObjParametros, ObjDetalle, Respuesta.Resultado);
                }
            }
            catch { }
            #endregion
            return Respuesta;
        }
        #endregion

        #region "--------+EditarVersion+---------"
        [HttpPost("EditarVersion")]

        public RespuestaFuncion EditarVersion([FromBody] ParametrosEditarVersion Parametros)
        {
            RespuestaFuncion Respuesta = new RespuestaFuncion() { Resultado = 0, Mensaje = "Error de datos" };
            DatosFuncion Datos = new DatosFuncion();
            string ClaveServicio = DateTime.UtcNow.Ticks.ToString();

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
                                    Respuesta = Datos.EditarVersion(Parametros, ClaveServicio);
                                }
                                else
                                {
                                    Respuesta.Resultado = -1000;
                                    Respuesta.Mensaje = "Error de token";
                                }
                            }
                            else
                            {
                                Respuesta.Resultado = -1000;
                                Respuesta.Mensaje = "Error de parametros: El Token es requerido";
                            }
                        }
                        else
                        {
                            Respuesta.Resultado = -1001;
                            Respuesta.Mensaje = "Error de parametros: El IdUsuario debe ser mayor a 0";
                        }
                    }
                    else
                    {
                        Respuesta.Resultado = -1001;
                        Respuesta.Mensaje = "Error de parametros: los parametros son requeridos";
                    }
                }
                else
                {
                    Respuesta.Resultado = -1003;
                    Respuesta.Mensaje = "Servicio no habilitado";
                }

            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }
            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        IdVersion = Parametros.IdVersion,
                        IdUsuarioRegistro = Parametros.IdUsuario,
                    };

                    object ObjDetalle = new
                    {
                        IdUsuario = Parametros.IdUsuario,
                        Token = Parametros.Token,
                    };
                    Datos.Utilidades.LogServicio(new List<string> { NombreServicio }, NombreServicio, MethodBase.GetCurrentMethod(), ClaveServicio, Respuesta, ObjParametros, ObjDetalle, Respuesta.Resultado);
                }
            }
            catch { }
            #endregion
            return Respuesta;
        }
        #endregion

        #region "--------+ReactivarVersion+---------"
        [HttpPost("ReactivarVersion")]

        public RespuestaFuncion ReactivarVersion([FromBody] ParametrosReactivarVersion Parametros)
        {
            RespuestaFuncion Respuesta = new RespuestaFuncion() { Resultado = 0, Mensaje = "Error de datos" };
            DatosFuncion Datos = new DatosFuncion();
            string ClaveServicio = DateTime.UtcNow.Ticks.ToString();

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
                                    Respuesta = Datos.ReactivarVersion(Parametros, ClaveServicio);
                                }
                                else
                                {
                                    Respuesta.Resultado = -1000;
                                    Respuesta.Mensaje = "Error de token";
                                }
                            }
                            else
                            {
                                Respuesta.Resultado = -1000;
                                Respuesta.Mensaje = "Error de parametros: El Token es requerido";
                            }
                        }
                        else
                        {
                            Respuesta.Resultado = -1001;
                            Respuesta.Mensaje = "Error de parametros: El IdUsuario debe ser mayor a 0";
                        }
                    }
                    else
                    {
                        Respuesta.Resultado = -1001;
                        Respuesta.Mensaje = "Error de parametros: los parametros son requeridos";
                    }
                }
                else
                {
                    Respuesta.Resultado = -1003;
                    Respuesta.Mensaje = "Servicio no habilitado";
                }

            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }
            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        IdVersionFuncion= Parametros.IdVersionFuncion,
                        IdUsuarioRegistro = Parametros.IdUsuario,
                    };

                    object ObjDetalle = new
                    {
                        IdUsuario = Parametros.IdUsuario,
                        Token = Parametros.Token,
                    };
                    Datos.Utilidades.LogServicio(new List<string> { NombreServicio }, NombreServicio, MethodBase.GetCurrentMethod(), ClaveServicio, Respuesta, ObjParametros, ObjDetalle, Respuesta.Resultado);
                }
            }
            catch { }
            #endregion
            return Respuesta;
        }
        #endregion
    }
}
