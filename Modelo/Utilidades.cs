using Backend.Datos;
using Newtonsoft.Json;
using Npgsql;
using System.Collections.Concurrent;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;

namespace BigDataJSN7.Modelo
{
    public class Utilidades
    {

        #region "----+Variables/Propiedades+----"
        public static string NombreServicio = "BigDataJSN7";
        public static GlobalTrack.Principal ObjBackend { get; set; }
        public static svc_EnviarCorreo.svc_ISendEmailClient ClienteCorreo { get; set; }
        public static ConcurrentDictionary<string, Backend.Datos.ServicioLog> DicServicios { get; set; }// Configuración servicios y logs

        #endregion


        #region "---------+Constructor+---------"

        public Utilidades()
        {
            try
            {
                ClienteCorreo = new svc_EnviarCorreo.svc_ISendEmailClient();

                ObjBackend = new GlobalTrack.Principal(GlobalTrack.Datos.EProyecto.BigDataTM, TipoServicio.Web, NombreServicio, false);

                DicServicios = new ConcurrentDictionary<string, ServicioLog>();
                if (ObjBackend != null)
                {
                    foreach (var item in GlobalTrack.Principal.DicPropiedades)
                    {
                        #region "--- + Habilitar servicios + ---"
                        if (item.Key.StartsWith("mc_"))
                        {
                            string[] Key = item.Key.Split('_');
                            ServicioLog ObjServicio = null;
                            if (!DicServicios.ContainsKey(item.Key.Trim()))
                            {
                                ObjServicio = new ServicioLog
                                {
                                    HabilitarServicio = false,
                                    IdsEmpresa = new int[] { },
                                    IdsFlotilla = new int[] { }
                                };

                                Utilidades.DicServicios.TryAdd(Key[1], ObjServicio);
                            }
                            else ObjServicio = Utilidades.DicServicios[item.Key.Trim()];

                            if (string.IsNullOrEmpty(item.Value)) continue;
                            string[] ArrayVal = item.Value.Split('|');

                            if (ArrayVal.Length > 0)
                                ObjServicio.HabilitarServicio = Convert.ToBoolean(ArrayVal[0]);

                            if (ArrayVal.Length > 1)
                                ObjServicio.HabilitarLogServicio = Convert.ToBoolean(ArrayVal[1]);

                            if (ArrayVal.Length > 2)
                                ObjServicio.HabilitarLogsDatos = Convert.ToBoolean(ArrayVal[2]);

                            DicServicios[item.Key.Trim()] = ObjServicio;
                        }
                        #endregion
                    }
                }

            }
            catch
            {

            }
        }

        #endregion


        #region "-----------+Conexion+----------"
        public static NpgsqlConnection ObtenerConexion(ServiciosMC _EServicio, int _Funcion, int? _MaxPoolSize = 30, int? _Timeout = 180)
        {
            NpgsqlConnection Conexion = null;

            int ServidorDefault = 1;//ConexionPrincipal (Default)
            int ServidorAsignado = -1;
            ServidorBD ObjConexion = null;
            string _NombreServicio = $"{_Funcion}_{_EServicio}";
            string KeyServicio = $"*{_EServicio}";//*svc_Servicio_D(Identificador)

            try
            {
                ConcurrentDictionary<int, ServidorBD> DicConexiones = GlobalTrack.Principal.DicConexiones;//
                if (GlobalTrack.Principal.DicPropiedades != null && GlobalTrack.Principal.DicPropiedades.Count > 0)//"*svc_Alertas": "D2:1,3,4,5,6,7,8|D2:10,11"
                {
                    if (GlobalTrack.Principal.DicPropiedades.ContainsKey(KeyServicio))
                    {
                        string Valores = GlobalTrack.Principal.DicPropiedades[KeyServicio];

                        string[] Configuraciones = Valores.Split('|');

                        for (int i = 0; i < Configuraciones.Length; i++)
                        {
                            string Configuracion = Configuraciones[i];
                            string[] Datos = Configuracion.Split(':');

                            if (Datos.Length > 1)
                            {
                                string Funciones = Datos[1];
                                if (Funciones.Contains(_Funcion.ToString()))
                                {
                                    if (int.TryParse(Datos[0].Replace("D", ""), out int ServidorActivo))

                                        ServidorAsignado = ServidorActivo;
                                    break;
                                }
                            }
                        }
                    }

                    if (DicConexiones != null)
                    {
                        if (DicConexiones.ContainsKey(ServidorAsignado))
                            ObjConexion = DicConexiones[ServidorAsignado];

                        if (ObjConexion == null)
                            ObjConexion = DicConexiones[ServidorDefault];

                        Conexion = new NpgsqlConnection(CadenaConexion(ObjConexion.DireccionIP.ToString(), ObjConexion.Puerto,
                        _Timeout.Value, _NombreServicio, ObjConexion.NombreBD, ObjConexion.Usuario, ObjConexion.Contrasenia, ObjConexion.ETipoServidor));
                    }
                    else
                    {
                        throw new ArgumentNullException(paramName: MethodBase.GetCurrentMethod().Name, message: $"Cadenas de Conexion no configuradas");
                    }
                }

                if (Conexion == null)
                    throw new ArgumentNullException(paramName: MethodBase.GetCurrentMethod().Name, message: $"Conexion Nula, Servicio: {_EServicio} Funcion: {_Funcion}");
            }
            catch
            {
                Conexion = null;
                throw;
            }

            return Conexion;
        }
        private static string CadenaConexion(string _DireccionIP, int _Puerto, int _Timeout, string _Nombre, string _BD, string _Usuario, string _Contrasenia, TipoServidor _TipoServidor)
        {
            string Conexion = $"Host={_DireccionIP}; ";
            Conexion += $"Port={_Puerto}; ";
            Conexion += $"Database={_BD}; ";
            Conexion += $"Username={_Usuario}; ";
            Conexion += $"Password={_Contrasenia}; ";
            Conexion += "Pooling=true; ";
            Conexion += "Minimum Pool Size=0; ";
            Conexion += $"Maximum Pool Size=10;";
            Conexion += "Connection Idle Lifetime=10; ";
            Conexion += "Connection Pruning Interval=1; ";
            Conexion += $"Timeout={_Timeout}; ";
            Conexion += "Command Timeout=180; ";

            if (_TipoServidor == TipoServidor.Replica)
                Conexion += "Server Compatibility Mode=NoTypeLoading; ";

            Conexion += $"APPLICATIONNAME={_Nombre};";

            return Conexion;
        }
        #endregion


        #region "------------+Logs+-------------"

        public static void RegistrarError(string[] _Directorio, string _NombreServicio, Exception _ObjException, MethodBase _ObjMethodBase, string _NombreArchivo, string _Query = "", NpgsqlConnection _Conexion = null, bool _Enviar = true)
        {
            List<string> Directorio = new List<string>() {
                _NombreServicio,
                GlobalTrack.Principal.FechaUniversal.ToString("yyyyMMdd"),
                "Errores",
                _ObjMethodBase.Name
            };

            Directorio = Directorio.Union(_Directorio).ToList();

            try
            {
                string CadenaConexion = string.Empty;
                if (_Conexion != null)
                    CadenaConexion = _Conexion.ConnectionString;

                if (_Enviar)
                {
                    try
                    {
                        string Funcion = _ObjMethodBase.Name;
                        string Clase = _ObjMethodBase.ReflectedType.Name;

                        string Asunto = string.Format("{0} {1} {2}", NombreServicio, ObjBackend.ObjAplicacion.ObjAmbiente.NombreAmbiente.Trim(), Funcion);
                        StringBuilder Mensaje = GlobalTrack.Principal.HTMLExcepcion(ObjBackend.ObjAplicacion, _ObjMethodBase, _ObjException, _NombreArchivo, _Query);

                        ClienteCorreo.EnviarCorreoErrorAsync(Asunto, Mensaje.ToString(), ObjBackend.ObjAplicacion.CorreoErrores, "",
                            ObjBackend.ObjAplicacion.CorreoQueEnviaErroresServicio.Trim(), ObjBackend.ObjAplicacion.NombreCorreoInterno);
                    }
                    catch { }

                    string Archivo = string.Empty;
                    try
                    {
                        Archivo = GlobalTrack.Principal.LogArchivoCsvExcepcionWeb(ObjBackend.ObjAplicacion, Directorio.ToArray(), _ObjException, _ObjMethodBase, _NombreArchivo, _Query, CadenaConexion, false, false);
                    }
                    catch { }

                    try
                    {
                        if (ObjBackend.ObjAplicacion.TelegramErrores)
                        {
                            GlobalTrack.Principal.TelegramMensajeError(_ObjMethodBase, _ObjException, ObjBackend.ObjAplicacion.ObjAmbiente, ObjBackend.ObjAplicacion.IdServicio, NombreServicio, ObjBackend.DicBotCanales, GlobalTrack.Principal.DicAplicacionEmpresa);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
        public void LogServicio(List<string> _Directorio, string _NombreServicio, MethodBase _ObjMethodBase, string _NombreArchivo, dynamic _Resultado, object _Parametros, object _Detalle = null, dynamic _Estado = null)
        {
            try
            {
                List<string> Directorio = new List<string> {
                _NombreServicio,
                GlobalTrack.Principal.FechaUniversal.ToString("yyyyMMdd") ,
                _ObjMethodBase.Name,
                "Peticiones",
                _NombreArchivo
            };
                _NombreArchivo = $"Servicio_{_NombreArchivo}";

                Directorio = Directorio.Union(_Directorio).ToList();


                #region "+--- Valor columnas ---+"
                string StrEstado = string.Empty;
                string StrParametros = string.Empty;
                string StrDetalle = string.Empty;
                string StrResultado = string.Empty;

                #region "Serealizacionn con newtonsoft"
                if (_Estado != null)
                    StrEstado = JsonConvert.SerializeObject(_Estado);

                if (_Parametros != null)
                    StrParametros = JsonConvert.SerializeObject(_Parametros);

                if (_Detalle != null)
                    StrDetalle = JsonConvert.SerializeObject(_Detalle);

                if (_Resultado != null)
                    StrResultado = JsonConvert.SerializeObject(_Resultado);
                #endregion

                #endregion

                string[] ListaEncabezado = new string[] { "Estado", "Parametros", "Detalle", "Resultado" };
                List<string[]> ListaContenido = new List<string[]> { new string[] { StrEstado, StrParametros, StrDetalle, StrResultado } };
                GlobalTrack.Principal.LogArchivoCsvWeb(Utilidades.ObjBackend.ObjAplicacion, Directorio.ToArray(), _NombreArchivo, ListaEncabezado, ListaContenido, false, true, false, false);
            }
            catch { }
        }
        public void LogDatos(List<string> _Directorio, string _NombreServicio, MethodBase _ObjMethodBase, string _NombreArchivo, dynamic _Resultado)
        {

            List<string> Directorio = new List<string> {
                _NombreServicio,
                GlobalTrack.Principal.FechaUniversal.ToString("yyyyMMdd") ,
                 _ObjMethodBase.Name,
                "Peticiones",
                _NombreArchivo
            };
            _NombreArchivo = $"Datos_{_NombreArchivo}";
            try
            {
                Directorio = Directorio.Union(_Directorio).ToList();

                #region "+--- Valor columnas ---+"           
                string StrResultado = string.Empty;


                if (_Resultado != null)
                    StrResultado = JsonConvert.SerializeObject(_Resultado);
                #endregion

                string[] ListaEncabezado = new string[] { "Estado", "Parametros", "Detalle", "Resultado" };
                List<string[]> ListaContenido = new List<string[]> { new string[] { StrResultado } };
                var algo = GlobalTrack.Principal.LogArchivoCsvWeb(Utilidades.ObjBackend.ObjAplicacion, Directorio.ToArray(), _NombreArchivo, ListaEncabezado, ListaContenido, false, true, false, false);
            }
            catch { }
        }

        #endregion
        

    }

}
