using BigDataJSN7.Modelo;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using static BigDataJSN7.Modelo.DatosInteraccion;

namespace BigDataJSN7.Controllers
{
    [Route("mc_Interaccion")]
    [ApiController]
    public class ControlInteraccion : ControllerBase
    {
        static string NombreServicio = ServiciosMC.mc_Interaccion.ToString();
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

        #region "--------+NuevaInteraccion+---------"
        [HttpPost("NuevaInteraccion")]

        public RespuestaInteraccion NuevaInteraccion([FromBody] ParametrosInteraccion Parametros)
        {
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            DatosInteraccion Datos = new DatosInteraccion();
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
                                    Respuesta = Datos.NuevaInteraccion(Parametros, ClaveServicio);
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
                        NombrePlantilla = Parametros.NombrePlantilla,
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

        #region "--------+EditarInteraccion+---------"
        [HttpPost("EditarInteraccion")]

        public RespuestaInteraccion EditarInteraccion([FromBody] ParametrosEditarInteraccion Parametros)
        {
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            DatosInteraccion Datos = new DatosInteraccion();
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
                                    Respuesta = Datos.EditarInteraccion(Parametros, ClaveServicio);
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
                        IdInteraccion = Parametros.IdInteraccion,
                        NombreInteraccion = Parametros.Nombre,
                        IdUsuarioModifico = Parametros.IdUsuario,
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

        #region "--------+EditarPlantilla+---------"
        [HttpPost("EditarPlantilla")]

        public RespuestaInteraccion EditarPlantilla([FromBody] ParametrosEditarPlantilla Parametros)
        {
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            DatosInteraccion Datos = new DatosInteraccion();
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
                                    Respuesta = Datos.EditarPlantilla(Parametros, ClaveServicio);
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
                        IdPlantilla = Parametros.IdPlantilla,
                        NombreInteraccion = Parametros.NombrePlantilla,
                        IdUsuarioModifico = Parametros.IdUsuario,
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

        #region "--------+MigrarInteraccion+---------"
        [HttpPost("MigrarInteraccion")]

        public RespuestaInteraccion MigrarInteraccion([FromBody] ParametrosMigrarInteraccion Parametros)
        {
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            DatosInteraccion Datos = new DatosInteraccion();
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
                                    Respuesta = Datos.MigrarInteraccion(Parametros, ClaveServicio);
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
                        NombrePlantilla = Parametros.NombrePlantilla,
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
 
