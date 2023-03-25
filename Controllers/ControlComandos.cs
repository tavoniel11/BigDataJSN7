using BigDataJSN7.Modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using static BigDataJSN7.Modelo.DatosComandos;

namespace BigDataJSN7.Controllers
{
    [Route("mc_Comandos")]
    [ApiController]
    public class ControlComandos : ControllerBase
    {

        static string NombreServicio = ServiciosMC.mc_Comandos.ToString();
        static string[] Directorio = new string[] { NombreServicio };

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
        #endregion
        
        #region "--------+InsertarComando+----------"

        [HttpPost]
        [Route("InsertarComando")]
        public RespuestaComandos InsertarComando([FromBody] ParametrosComando Parametros)
        {
            RespuestaComandos Objeto = new RespuestaComandos() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };
            DatosComandos Datos = new DatosComandos();
            string ClaveServicio = DateTime.UtcNow.Ticks.ToString();
            string Token = Security.TacoSecurity.GeneraToken(1,0);

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
                                    Objeto = Datos.InsertarComando(Parametros, ClaveServicio);
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
                Utilidades.RegistrarError(DatosComandos.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }

            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        Etiqueta = Parametros.Etiqueta
                    };

                    object ObjDetalle = new
                    {
                        IdUsuario = Parametros.IdUsuario,
                        Token = Parametros.Token,
                    };
                    Datos.Utilidades.LogServicio(new List<string> { NombreServicio }, NombreServicio, MethodBase.GetCurrentMethod(), ClaveServicio, Objeto, ObjParametros, ObjDetalle, Objeto.Estado);
                }
            }
            catch { }
            #endregion

            return Objeto;
        }

        #endregion

        #region "-------+ModificarComando+----------"

        [HttpPost]
        [Route("ModificarComando")]
        public RespuestaComandos ModificarComando([FromBody] ParametroEditarComando Parametros)
        {
            RespuestaComandos Objeto = new RespuestaComandos() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };
            DatosComandos Datos = new DatosComandos();
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
                                    Objeto = Datos.ModificarComando(Parametros, ClaveServicio);
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
                Utilidades.RegistrarError(DatosComandos.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }

            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        Id = Parametros.IdComando,
                        Etiqueta = Parametros.Etiqueta,
                        IdUsuarioModifico = Parametros.IdUsuario,
                    };

                    object ObjDetalle = new
                    {
                        IdUsuario = Parametros.IdUsuario,
                        Token = Parametros.Token,
                    };
                    Datos.Utilidades.LogServicio(new List<string> { NombreServicio }, NombreServicio, MethodBase.GetCurrentMethod(), ClaveServicio, Objeto, ObjParametros, ObjDetalle, Objeto.Estado);
                }
            }
            catch { }
            #endregion

            return Objeto;
        }

        #endregion

        #region "---------+MigrarComando+-----------"

        [HttpPost]
        [Route("MigrarComando")]
        public RespuestaComandos MigrarComando([FromBody] ParametrosMigrarComando Parametros)
        {
            RespuestaComandos Objeto = new RespuestaComandos() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };
            DatosComandos Datos = new DatosComandos();
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
                                    Objeto = Datos.MigrarComando(Parametros, ClaveServicio);
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
                Utilidades.RegistrarError(DatosComandos.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);
            }

            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        IdComando = Parametros.IdComando,
                        IdEmpresa = Parametros.IdEmpresa,
                        BoolPublico = Parametros.BoolPublico,
                        BoolDistribuidor = Parametros.BoolDistribuidor,
                    };

                    object ObjDetalle = new
                    {
                        IdUsuario = Parametros.IdUsuario,
                        Token = Parametros.Token,
                    };
                    Datos.Utilidades.LogServicio(new List<string> { NombreServicio }, NombreServicio, MethodBase.GetCurrentMethod(), ClaveServicio, Objeto, ObjParametros, ObjDetalle, Objeto.Estado);
                }
            }
            catch { }
            #endregion

            return Objeto;
        }

        #endregion



    }
}
