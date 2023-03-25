using Backend.Datos;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Office2016.Excel;
using Mono.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Reflection;
using System.Text.Json.Nodes;
using static BigDataJSN7.Modelo.DatosInteraccion;

namespace BigDataJSN7.Modelo
{
    public class DatosFuncion
    {
        #region "------+Variables/Propiedades+------"

        public Utilidades Utilidades;
        public ServicioLog ObjServicio;

        private string NombreServicio = ServiciosMC.mc_Funcion.ToString();
        public static readonly string[] Directorio = { ServiciosMC.mc_Funcion.ToString() };

        #endregion

        #region "-----------+Constructor+-----------"
        public DatosFuncion()
        {
            Utilidades = new Utilidades();
            ObjServicio = new ServicioLog();

            if (Utilidades.DicServicios.ContainsKey(NombreServicio))
                ObjServicio = Utilidades.DicServicios[NombreServicio];
        }
        #endregion

        #region "-----------+NuevaFuncion+-----------"
        public RespuestaFuncion NuevaFuncion(ParametrosFuncion _Parametros, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Funcion, 1);
            RespuestaFuncion Respuesta = new RespuestaFuncion() { Resultado = 0, Mensaje = "Error de datos" };

            string JsonInstalacion = JsonConvert.SerializeObject(_Parametros.Instalacion);
            string JsonParametros = JsonConvert.SerializeObject(_Parametros.Parametros);
            string JsonRespuesta = JsonConvert.SerializeObject(_Parametros.Respuesta);
            string JsonDatos = JsonConvert.SerializeObject(_Parametros.Datos);

            try
            {
                int ResultadoProceso = ExisteProceso(_Parametros.IdServicio, _ClaveServicio);

                if (ResultadoProceso > 0)
                {
                    Query = "INSERT INTO tbl.funcion(" +
                           "int_idservicio, int_idproyecto, int_idempresa_embe, var_nombre, int_idusuario_registro, int_idusuario_modifico, bol_enuso, dt_modificacion, dt_registro)" +
                           $"VALUES({_Parametros.IdServicio},{_Parametros.IdProyecto},{_Parametros.IdEmpresaEmbe},@Nombre,{_Parametros.IdUsuario},{_Parametros.IdUsuario},TRUE," +
                           $"CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC',CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC'); " +
                           $"SELECT CURRVAL('tbl.funcion_int_id_seq');";

                    Conexion.Open();
                    var Transaccion = Conexion.BeginTransaction();
                    try
                    {
                        NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                        Cmd1.Parameters.AddWithValue("@Nombre", NpgsqlDbType.Varchar, _Parametros.Nombre);
                        int IdResultado = int.Parse(Cmd1.ExecuteScalar().ToString());
                        if (IdResultado > 0)
                        {
                            int ResultadoServicio = ExisteServicio(_Parametros.IdServicio, _Parametros.Nombre, _ClaveServicio);
                            if (ResultadoServicio == 0)
                            {
                                Respuesta.Resultado = IdResultado;
                                Query = "INSERT INTO tbl.funcion_detalle(" +
                                                "int_idfuncion, int_idservicio, int_idempresa_embe, int_idambiente, json_instalacion, json_parametros, json_respuesta," +
                                                "bol_enuso, int_idusuario_registro,int_idusuario_modifico, dt_modificacion, dt_registro) " +
                                                $"VALUES ({IdResultado},{_Parametros.IdServicio},{_Parametros.IdEmpresaEmbe},{_Parametros.IdAmbiente},@Instalacion,@Parametros,@Respuesta, " +
                                                $"TRUE,{_Parametros.IdUsuario},{_Parametros.IdUsuario}, CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC',CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC');" +
                                                $"SELECT CURRVAL('tbl.funcion_detalle_int_id_seq');";

                                NpgsqlCommand Cmd2 = new NpgsqlCommand(Query, Conexion);

                                Cmd2.Parameters.AddWithValue("@Instalacion", NpgsqlTypes.NpgsqlDbType.Json, JsonInstalacion);
                                Cmd2.Parameters.AddWithValue("@Parametros", NpgsqlTypes.NpgsqlDbType.Json, JsonParametros);
                                Cmd2.Parameters.AddWithValue("@Respuesta", NpgsqlTypes.NpgsqlDbType.Json, JsonRespuesta);

                                int IdResultadoFuncion = int.Parse(Cmd2.ExecuteScalar().ToString());

                                if (IdResultadoFuncion > 0)
                                {
                                    int ResultadoDetalle = ExisteServicio(_Parametros.IdServicio, "", _ClaveServicio);
                                    if (ResultadoDetalle == 0)
                                    {

                                        Query = "INSERT INTO tbl.version_funcion( " +
                                                "int_idservicio, int_idfuncion_detalle, var_descripcion_version, json_datos, var_funcion_datos, bol_reactivo, var_motivo, " +
                                                "bol_activo, int_idusuario_registro, int_idusuario_modifico, dt_publicado, dt_modificacion, dt_registro) " +
                                                $"VALUES ({_Parametros.IdServicio},{IdResultadoFuncion},@DescripcionVersion,@Datos,@FuncionDatos,TRUE,NULL, " +
                                                $"TRUE,{_Parametros.IdUsuario},{_Parametros.IdUsuario}, CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC', CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC', CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC');";

                                        NpgsqlCommand Cmd3 = new NpgsqlCommand(Query, Conexion, Transaccion);
                                        Cmd3.Parameters.AddWithValue("@DescripcionVersion", NpgsqlDbType.Varchar, _Parametros.DescripcionVersion);
                                        Cmd3.Parameters.AddWithValue("@Datos", NpgsqlTypes.NpgsqlDbType.Json, JsonDatos);
                                        Cmd3.Parameters.AddWithValue("@FuncionDatos", NpgsqlDbType.Varchar, _Parametros.FuncionDatos);

                                        Cmd3.ExecuteNonQuery();
                                        Transaccion.Commit();
                                        Respuesta.Resultado = IdResultado;
                                        Respuesta.Mensaje = "Ok";
                                    }
                                    else
                                    {
                                        Respuesta.Resultado = -4;
                                        Respuesta.Mensaje = "Error: El nombre del servicio ya esta registrado";
                                    }
                                }
                            }
                            else
                            {
                                Respuesta.Resultado = -4;
                                Respuesta.Mensaje = "Error: El nombre del servicio ya esta registrado";
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        Transaccion.Rollback();
                        Respuesta.Resultado = -2;
                        Respuesta.Mensaje = "Error: No se pudo completar el registro";
                        Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
                    }
                }
                else
                {
                    Respuesta.Resultado = -3;
                    Respuesta.Mensaje = "Errror: No existe un servicio";
                }
            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }
            return Respuesta;
        }
        #endregion

        #region "---------+NuevoDetalle+---------"
        public RespuestaFuncion NuevoDetalle(ParametrosDetalle _Parametros, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Funcion, 2);
            RespuestaFuncion Respuesta = new RespuestaFuncion() { Resultado = 0, Mensaje = "Error de datos" };

            string JsonInstalacion = JsonConvert.SerializeObject(_Parametros.Instalacion);
            string JsonParametros = JsonConvert.SerializeObject(_Parametros.Parametros);
            string JsonRespuesta = JsonConvert.SerializeObject(_Parametros.Respuesta);
            string JsonDatos = JsonConvert.SerializeObject(_Parametros.Datos);

            try
            {
                int ResultadoDetalle = ExisteDetalle(_Parametros.IdServicio, _Parametros.IdAmbiente, _Parametros.IdFuncion, _ClaveServicio);

                if (ResultadoDetalle == 0)
                {

                    Query = "INSERT INTO tbl.funcion_detalle( " +
                            "int_idfuncion, int_idservicio, int_idempresa_embe, int_idambiente, json_instalacion, json_parametros, json_respuesta," +
                            "bol_enuso, int_idusuario_registro,int_idusuario_modifico, dt_modificacion, dt_registro) " +
                             $"VALUES ({_Parametros.IdFuncion},{_Parametros.IdServicio},{_Parametros.IdEmpresaEmbe},{_Parametros.IdAmbiente},@Instalacion,@Parametros,@Respuesta, " +
                             $"TRUE,{_Parametros.IdUsuario},{_Parametros.IdUsuario}, CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC',CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC');" +
                             $"SELECT CURRVAL('tbl.funcion_detalle_int_id_seq');";

                    Conexion.Open();
                    var Transaccion = Conexion.BeginTransaction();
                    try
                    {
                        NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);

                        Cmd1.Parameters.AddWithValue("@Instalacion", NpgsqlTypes.NpgsqlDbType.Json, JsonInstalacion);
                        Cmd1.Parameters.AddWithValue("@Parametros", NpgsqlTypes.NpgsqlDbType.Json, JsonParametros);
                        Cmd1.Parameters.AddWithValue("@Respuesta", NpgsqlTypes.NpgsqlDbType.Json, JsonRespuesta);

                        int IdResultadoFuncion = int.Parse(Cmd1.ExecuteScalar().ToString());
                        if (IdResultadoFuncion > 0)
                        {
                            Respuesta.Resultado = IdResultadoFuncion;

                            Query = "INSERT INTO tbl.version_funcion( " +
                                    "int_idservicio, int_idfuncion_detalle, var_descripcion_version, json_datos, var_funcion_datos, bol_reactivo, var_motivo, " +
                                    "bol_activo, int_idusuario_registro, int_idusuario_modifico, dt_publicado, dt_modificacion, dt_registro) " +
                                    $"VALUES ({_Parametros.IdServicio},{IdResultadoFuncion},@DescripcionVersion,@Datos,@FuncionDatos,TRUE,NULL, " +
                                    $"TRUE,{_Parametros.IdUsuario},{_Parametros.IdUsuario}, CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC', CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC', CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC');";

                            NpgsqlCommand Cmd2 = new NpgsqlCommand(Query, Conexion, Transaccion);
                            Cmd2.Parameters.AddWithValue("@DescripcionVersion", NpgsqlDbType.Varchar, _Parametros.DescripcionVersion);
                            Cmd2.Parameters.AddWithValue("@Datos", NpgsqlTypes.NpgsqlDbType.Json, JsonDatos);
                            Cmd2.Parameters.AddWithValue("@FuncionDatos", NpgsqlDbType.Varchar, _Parametros.FuncionDatos);

                            Cmd2.ExecuteNonQuery();
                            Transaccion.Commit();
                            Respuesta.Resultado = IdResultadoFuncion;
                            Respuesta.Mensaje = "Ok";
                        }
                    }
                    catch (Exception Ex)
                    {
                        Transaccion.Rollback();
                        Respuesta.Resultado = -2;
                        Respuesta.Mensaje = "Error: No se pudo completar el registro";
                        Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
                    }
                }
                else
                {
                    Respuesta.Resultado = -3;
                    Respuesta.Mensaje = "Errro : Los datos ya existen";
                }
            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }
            return Respuesta;
        }
        #endregion

        #region "---------+EditarFuncion+---------"
        public RespuestaFuncion EditarFuncion(ParametrosEditarFuncion _Parametros, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Funcion, 3);
            RespuestaFuncion Respuesta = new RespuestaFuncion() { Resultado = 0, Mensaje = "Error de datos" };
            try
            {
                if (!string.IsNullOrEmpty(_Parametros.Nombre) || !string.IsNullOrWhiteSpace(_Parametros.Nombre))
                {
                    if (_Parametros.IdFuncion > 0)
                    {
                        int ResultadoFuncion = ExisteFuncion(_Parametros.IdFuncion, _ClaveServicio);

                        if (ResultadoFuncion > 0)
                        {
                            if (ResultadoFuncion == 1)
                            {
                                int ResultadoServicio = ExisteServicio(_Parametros.IdFuncion, _Parametros.Nombre, _ClaveServicio);

                                if (ResultadoServicio == 0)
                                {
                                    Query = "UPDATE tbl.funcion " +
                                        "SET var_nombre=@Nombre, int_idusuario_modifico=@IdUsuario,dt_modificacion=Current_timestamp(3) AT TIME ZONE 'UTC' " +
                                        "WHERE int_id=@IdFuncion";

                                    Conexion.Open();
                                    NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                                    Cmd1.Parameters.AddWithValue("@Nombre", NpgsqlDbType.Varchar, _Parametros.Nombre);
                                    Cmd1.Parameters.AddWithValue("@IdFuncion", NpgsqlDbType.Integer, _Parametros.IdFuncion);
                                    Cmd1.Parameters.Add(new NpgsqlParameter("@IdUsuario", NpgsqlDbType.Integer)).Value = _Parametros.IdUsuario;
                                    int IdResultado = Cmd1.ExecuteNonQuery();

                                    if (IdResultado > 0)
                                    {
                                        Respuesta.Mensaje = "Ok";
                                        Respuesta.Resultado = 1;
                                    }
                                    else
                                    {

                                        Respuesta.Mensaje = "Error: No se completo la modificacion";
                                        Respuesta.Resultado = -2;
                                    }
                                }
                                else
                                {
                                    Respuesta.Resultado = -3;
                                    Respuesta.Mensaje = "Error: El servicio ya esta registrado";
                                }
                            }
                            else
                            {
                                Respuesta.Resultado = -4;
                                Respuesta.Mensaje = "Error: El id ya tiene mas de 1 registro";
                            }
                        }
                        else
                        {
                            Respuesta.Resultado = -5;
                            Respuesta.Mensaje = "Error: No existe Funcion";
                        }
                    }
                    else
                    {
                        Respuesta.Resultado = -6;
                        Respuesta.Mensaje = "Error: El IdFuncion debe ser mayor a 0";
                    }
                }
                else
                {
                    Respuesta.Resultado = -7;
                    Respuesta.Mensaje = "Error: Nombre de servicio no valido";
                }
            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }
            return Respuesta;
        }
        #endregion

        #region "---------+EditarDetalle+---------"
        public RespuestaFuncion EditarDetalle(ParametrosEditarDetalle _Parametros, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Funcion, 4);
            RespuestaFuncion Respuesta = new RespuestaFuncion() { Resultado = 0, Mensaje = "Error de datos" };

            string JsonInstalacion = JsonConvert.SerializeObject(_Parametros.Instalacion);
            string JsonParametros = JsonConvert.SerializeObject(_Parametros.Parametros);
            string JsonRespuesta = JsonConvert.SerializeObject(_Parametros.Respuesta);
            string JsonDatos = JsonConvert.SerializeObject(_Parametros.Datos);

            try
            {
                if (_Parametros.Id > 0)
                {
                    Query = "UPDATE tbl.funcion_detalle " +
                        "SET json_instalacion=@Instalacion, json_parametros=@Parametros, json_respuesta=@Respuesta, " +
                        "int_idusuario_modifico=@IdUsuario, dt_modificacion=Current_timestamp(3) AT TIME ZONE 'UTC' " +
                        "WHERE int_id=@Id";

                    Conexion.Open();
                    var Transaccion = Conexion.BeginTransaction();
                    try
                    {
                        NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                        Cmd1.Parameters.AddWithValue("@Instalacion", NpgsqlTypes.NpgsqlDbType.Json, JsonInstalacion);
                        Cmd1.Parameters.AddWithValue("@Parametros", NpgsqlTypes.NpgsqlDbType.Json, JsonParametros);
                        Cmd1.Parameters.AddWithValue("@Respuesta", NpgsqlTypes.NpgsqlDbType.Json, JsonRespuesta);
                        Cmd1.Parameters.AddWithValue("@IdUsuario", NpgsqlDbType.Integer, _Parametros.IdUsuario);
                        Cmd1.Parameters.AddWithValue("@Id", NpgsqlDbType.Integer, _Parametros.Id);
                        int IdResultadoDetalle = Cmd1.ExecuteNonQuery();

                        if (IdResultadoDetalle > 0)
                        {
                            int ResultadoActivo = ObtenerEstadoActivo(_Parametros.Id, _ClaveServicio);

                            if (ResultadoActivo > 1)
                            {

                                Query = "UPDATE tbl.version_funcion SET " +
                                    "bol_activo=false " +
                                    "WHERE bol_activo=true AND int_idfuncion_detalle=@Id";
                                NpgsqlCommand Cmd2 = new NpgsqlCommand(Query, Conexion);
                                Cmd2.Parameters.AddWithValue("@Id", NpgsqlDbType.Integer, _Parametros.Id);
                                int IdResultadoFuncionDetalle = Cmd2.ExecuteNonQuery();

                                if (IdResultadoFuncionDetalle > 0)
                                {
                                    Query = "INSERT INTO tbl.version_funcion " +
                                     "(int_idservicio, int_idfuncion_detalle, var_descripcion_version, json_datos, var_funcion_datos, bol_reactivo, " +
                                     "var_motivo, bol_activo, int_idusuario_registro, int_idusuario_modifico, dt_publicado, dt_modificacion, dt_registro) " +
                                     $"VALUES (@IdServicio, @IdFuncionDetalle, @DescripcionVersion, @Datos, @FuncionDatos, false, null, true,{_Parametros.IdUsuario},{_Parametros.IdUsuario}," +
                                     $" CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC', CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC', CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC')";
                                    NpgsqlCommand Cmd3 = new NpgsqlCommand(Query, Conexion, Transaccion);
                                    Cmd3.Parameters.AddWithValue("@IdServicio", NpgsqlDbType.Integer, _Parametros.IdServicio);
                                    Cmd3.Parameters.AddWithValue("@IdFuncionDetalle", NpgsqlDbType.Integer, _Parametros.Id);
                                    Cmd3.Parameters.AddWithValue("@DescripcionVersion", NpgsqlDbType.Varchar, _Parametros.DescripcionVersion);
                                    Cmd3.Parameters.AddWithValue("@Datos", NpgsqlTypes.NpgsqlDbType.Json, JsonDatos);
                                    Cmd3.Parameters.AddWithValue("@FuncionDatos", NpgsqlDbType.Varchar, _Parametros.FuncionDatos);
                                    Cmd3.ExecuteNonQuery();

                                    Transaccion.Commit();
                                    Respuesta.Resultado = 1;
                                    Respuesta.Mensaje = "Datos actualizados correctamente";
                                }
                            }

                        }
                    }
                    catch (Exception Ex)
                    {
                        Transaccion.Rollback();
                        Respuesta.Resultado = -2;
                        Respuesta.Mensaje = "Error: No se pudo completar el registro";
                        Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
                    }
                }
                else
                {
                    Respuesta.Resultado = -3;
                    Respuesta.Mensaje = "Error: El Id debe ser mayor a 0";
                }
            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }
            return Respuesta;
        }
        #endregion

        #region "---------+EditarVersion---------"
        public RespuestaFuncion EditarVersion(ParametrosEditarVersion _Parametros, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Funcion, 5);
            RespuestaFuncion Respuesta = new RespuestaFuncion() { Resultado = 0, Mensaje = "Error de datos" };

            string JsonDatos = JsonConvert.SerializeObject(_Parametros.Datos);

            try
            {
                Query = "UPDATE tbl.version_funcion " +
                    "SET var_descripcion_version=@DescripcionVersion, json_datos=@Datos, var_funcion_datos=@FuncionDatos, " +
                    "int_idusuario_modifico=@IdUsuario, dt_publicado=Current_timestamp(3) AT TIME ZONE 'UTC' " +
                    "WHERE int_id=@IdVersion AND bol_activo";

                Conexion.Open();
                NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                Cmd1.Parameters.AddWithValue("@IdVersion", NpgsqlDbType.Integer, _Parametros.IdVersion);
                Cmd1.Parameters.AddWithValue("@DescripcionVersion", NpgsqlDbType.Varchar, _Parametros.DescripcionVersion);
                Cmd1.Parameters.AddWithValue("@Datos", NpgsqlTypes.NpgsqlDbType.Json, JsonDatos);
                Cmd1.Parameters.AddWithValue("@FuncionDatos", NpgsqlDbType.Varchar, _Parametros.FuncionDatos);
                Cmd1.Parameters.AddWithValue("@IdUsuario", NpgsqlDbType.Integer, _Parametros.IdUsuario);
                int ResultadoVersion = Cmd1.ExecuteNonQuery();

                if (ResultadoVersion > 0)
                {
                    Respuesta.Resultado = 1;
                    Respuesta.Mensaje = "Ok";
                }
                else
                {

                    Respuesta.Mensaje = "Error: No se completo la modificacion";
                    Respuesta.Resultado = -2;
                }
            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }
            return Respuesta;
        }
        #endregion

        #region "-----------+ReactivarVersion+-----------"
        public RespuestaFuncion ReactivarVersion(ParametrosReactivarVersion _Parametros, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Funcion, 6);
            RespuestaFuncion Respuesta = new RespuestaFuncion() { Resultado = 0, Mensaje = "Error de datos" };

            try
            {
                int ResultadoVersionFuncion = ExisteVersion(_Parametros.IdVersionFuncion, _ClaveServicio);

                if (ResultadoVersionFuncion > 0)
                {
                    int ResultadoReactivo = ExisteReactivo(ResultadoVersionFuncion, _ClaveServicio);

                    Query = "UPDATE tbl.version_funcion " +
                        "SET bol_activo=true, bol_reactivo=true ,var_motivo=@Motivo " +
                        "WHERE int_id=@IdVersionFuncion AND bol_activo= false AND bol_reactivo = false;";

                    Conexion.Open();
                    var Transaccion = Conexion.BeginTransaction();
                    try
                    {
                        NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                        Cmd1.Parameters.AddWithValue("@IdVersionFuncion", NpgsqlDbType.Integer, _Parametros.IdVersionFuncion);
                        Cmd1.Parameters.AddWithValue("@IdUsuario", NpgsqlDbType.Integer, _Parametros.IdUsuario);
                        Cmd1.Parameters.AddWithValue("@Motivo", NpgsqlDbType.Varchar, _Parametros.Motivo);
                        int IdResultado = Cmd1.ExecuteNonQuery();

                        if (IdResultado > 0)
                        {
                            if (ResultadoReactivo > 0)
                            {
                                Query = "UPDATE tbl.version_funcion " +
                               "SET bol_activo = false, bol_reactivo = false, int_idusuario_modifico=@IdUsuario, dt_publicado=Current_timestamp(3) AT TIME ZONE 'UTC' " +
                               "WHERE int_id = @IdVersionFuncion AND bol_activo AND bol_reactivo;";

                                NpgsqlCommand Cmd2 = new NpgsqlCommand(Query, Conexion, Transaccion);
                                Cmd2.Parameters.AddWithValue("@IdVersionFuncion", NpgsqlDbType.Integer, ResultadoReactivo);
                                Cmd2.Parameters.AddWithValue("@IdUsuario", NpgsqlDbType.Integer, _Parametros.IdUsuario);

                                Cmd2.ExecuteNonQuery();
                                Transaccion.Commit();
                                Respuesta.Resultado = 1;
                                Respuesta.Mensaje = "Ok";
                            }
                            else
                            {
                                Respuesta.Resultado = -2;
                                Respuesta.Mensaje = "Error: No existe el id funcion ingresado";
                            }
                        }
                        else
                        {

                            Respuesta.Mensaje = "Error: No se completo la modificacion";
                            Respuesta.Resultado = -3;
                        }
                    }
                    catch (Exception Ex)
                    {
                        Transaccion.Rollback();
                        Respuesta.Resultado = -4;
                        Respuesta.Mensaje = "Error: No se pudo completar el registro";
                        Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
                    }
                }
                else
                {
                    Respuesta.Resultado = -5;
                    Respuesta.Mensaje = "Error: Ingresar un Id existente";
                }

            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }
            return Respuesta;
        }
        #endregion

        #region "-----------+FuncionesPrivadas+-----------"

        #region "-----------+ObtenerEstadoActivo+-----------"
        private int ObtenerEstadoActivo(int _IdFuncionDetalle, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Funcion, 7);
            RespuestaFuncion Respuesta = new RespuestaFuncion() { Resultado = 0, Mensaje = "Error de datos" };
            int Resultado = 0;

            try
            {
                Query = "SELECT COUNT(1) " +
                    "FROM tbl.funcion_detalle " +
                    "WHERE  int_idfuncion_detalle =" + _IdFuncionDetalle + " AND bol_activo;";

                Conexion.Open();
                NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                Cmd1.Parameters.AddWithValue("@Id", NpgsqlDbType.Integer, _IdFuncionDetalle);
                int CantidadActivo = Convert.ToInt32(Cmd1.ExecuteScalar());

                if (CantidadActivo > 0)
                {
                    Resultado = CantidadActivo;
                }
            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }

            return Resultado;
        }

        #endregion

        #region "-----------+ExisteProceso+-----------"
        private int ExisteProceso(int _IdServcio, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Funcion, 8);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            int Resultado = 0;

            try
            {
                Query = "SELECT COUNT(1) " +
                    "FROM tmp.particion_funcion_proceso " +
                    "WHERE int_idservicio = " + _IdServcio;

                Conexion.Open();
                NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                int CantidadFuncion = Convert.ToInt32(Cmd1.ExecuteScalar());

                if (CantidadFuncion > 0)
                {
                    string Json_base_datos = null;
                    Query = "SELECT json_procesos " +
                        "FROM tmp.particion_funcion_proceso " +
                        "WHERE int_idservicio = @Id_servicio;";
                    NpgsqlCommand Cmd2 = new NpgsqlCommand(Query, Conexion);
                    Cmd2.Parameters.AddWithValue("@Id_servicio", NpgsqlDbType.Integer, _IdServcio);
                    NpgsqlDataReader leer = Cmd2.ExecuteReader();

                    if (leer.Read())
                    {
                        Json_base_datos = leer.GetString(0);
                    }
                    leer.Close();

                    if (!string.IsNullOrEmpty(Json_base_datos))
                    {
                        JObject jObject = JObject.Parse(Json_base_datos);
                        int EstadoProcesoFuncion = (int)jObject.SelectToken("ProcesoFuncion.Estado");
                        int EstadoProcesoFuncionDetalle = (int)jObject.SelectToken("ProcesoFuncionDetalle.Estado");
                        int EstadoProcesoVersionFuncion = (int)jObject.SelectToken("ProcesoVersionFuncion.Estado");

                        if (EstadoProcesoFuncion == 1 && EstadoProcesoFuncionDetalle == 1 && EstadoProcesoVersionFuncion == 1)
                        {
                            Resultado = CantidadFuncion;
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }

            return Resultado;
        }
        #endregion

        #region "-----------+ExisteServicio+-----------"
        private int ExisteServicio(int _IdServcio, string _Nombre, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Funcion, 9);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            int Resultado = 0;

            try
            {
                Query = string.Format("SELECT COUNT(1) " +
                    "FROM tbl.funcion " +
                    "WHERE TRIM(LOWER(var_nombre)) = '{0}'", _Nombre.Trim().ToLower());

                Conexion.Open();
                if (_IdServcio > 0)
                {
                    Query += " AND int_id != " + _IdServcio;
                }

                NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                int CantidadServicio = Convert.ToInt32(Cmd1.ExecuteScalar());

                if (CantidadServicio > 0)
                {
                    Resultado = CantidadServicio;
                }
            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }

            return Resultado;
        }
        #endregion

        #region "-----------+ExisteDetalle+-----------"
        private int ExisteDetalle(int _IdServcio, int _IdAmbiente, int _IdFuncion, string _ClaveServicio)
        {

            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Funcion, 10);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            int Resultado = 0;

            try
            {
                Query = "SELECT COUNT(1) FROM tbl.funcion_detalle " +
                      "WHERE int_idfuncion = " + _IdFuncion + " AND int_idservicio = " + _IdServcio + " AND int_idambiente = " + _IdAmbiente;

                Conexion.Open();
                NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                int CantidadDetalle = Convert.ToInt32(Cmd1.ExecuteScalar());

                if (CantidadDetalle > 0)
                {
                    Resultado = CantidadDetalle;
                }
            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }

            return Resultado;
        }
        #endregion

        #region "-----------+ExisteFuncion+-----------"
        private int ExisteFuncion(int _IdFuncion, string _ClaveServicio)
        {

            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Funcion, 11);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            int Resultado = 0;

            try
            {
                Query = "SELECT COUNT(1) FROM tbl.funcion_detalle " +
                   "WHERE int_idfuncion = " + _IdFuncion;

                Conexion.Open();
                NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                int CantidadFuncion = Convert.ToInt32(Cmd1.ExecuteScalar());

                if (CantidadFuncion > 0)
                {
                    Resultado = CantidadFuncion;
                }
            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }

            return Resultado;
        }
        #endregion

        #region "-----------+ExisteVersion+-----------"
        private int ExisteVersion(int _IdVersionFuncion, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Funcion, 12);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            int Resultado = 0;

            try
            {
                Query = "SELECT int_idfuncion_detalle FROM tbl.version_funcion " +
                    "WHERE int_id =@IdVersionFuncion AND bol_activo = false AND bol_reactivo = false;";

                Conexion.Open();
                NpgsqlCommand Cmd2 = new NpgsqlCommand(Query, Conexion);
                Cmd2.Parameters.AddWithValue("@IdVersionFuncion", NpgsqlDbType.Integer, _IdVersionFuncion);
                int IdResultado = int.Parse(Cmd2.ExecuteScalar().ToString());
                if (IdResultado > 0)
                {
                    Resultado = IdResultado;
                }
            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }

            return Resultado;
        }
        #endregion

        #region "-----------+ExisteReactivo+-----------"
        private int ExisteReactivo(int _IdFuncionDetalle, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Funcion, 13);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            int Resultado = 0;

            try
            {
                Query = "SELECT int_id FROM tbl.version_funcion " +
                        "WHERE int_idfuncion_detalle = @IdFuncionDetalle AND bol_activo AND bol_reactivo;";

                Conexion.Open();
                NpgsqlCommand Cmd = new NpgsqlCommand(Query, Conexion);
                Cmd.Parameters.AddWithValue("@IdFuncionDetalle", NpgsqlDbType.Integer, _IdFuncionDetalle);
                int IdResultado = int.Parse(Cmd.ExecuteScalar().ToString());
                if (IdResultado > 0)
                {
                    Resultado = IdResultado;
                }
            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }

            return Resultado;
        }

        #endregion
        #endregion

        public class ParametrosFuncion : ParametroGeneral
        {
            public int IdServicio { get; set; }
            public int IdProyecto { get; set; }
            public int IdEmpresaEmbe { get; set; }
            public int IdAmbiente { get; set; }
            public string Nombre { get; set; }
            public List<int> Instalacion { get; set; }
            public string Parametros { get; set; }
            public string Respuesta { get; set; }
            public string DescripcionVersion { get; set; }
            public List<string> Datos { get; set; }
            public string FuncionDatos { get; set; }
        }
        public class ParametrosEditarFuncion : ParametroGeneral
        {
            public int IdFuncion { get; set; }
            public string Nombre { get; set; }

        }
        public class ParametrosDetalle : ParametroGeneral
        {
            public int IdFuncion { get; set; }
            public int IdServicio { get; set; }
            public int IdAmbiente { get; set; }
            public int IdEmpresaEmbe { get; set; }
            public List<int> Instalacion { get; set; }
            public string Parametros { get; set; }
            public string Respuesta { get; set; }
            public string DescripcionVersion { get; set; }
            public List<string> Datos { get; set; }
            public string FuncionDatos { get; set; }

        }


        public class ParametrosEditarDetalle : ParametroGeneral
        {
            public int Id { get; set; }
            public int IdServicio { get; set; }
            public List<int> Instalacion { get; set; }
            public string Parametros { get; set; }
            public string Respuesta { get; set; }
            public string DescripcionVersion { get; set; }
            public List<string> Datos { get; set; }
            public string FuncionDatos { get; set; }
        }

        public class ParametrosEditarVersion : ParametroGeneral
        {
            public int IdVersion { get; set; }
            public string DescripcionVersion { get; set; }
            public List<string> Datos { get; set; }
            public string FuncionDatos { get; set; }
        }
        public class ParametrosReactivarVersion : ParametroGeneral
        {
            public int IdVersionFuncion { get; set; }
            public string Motivo { get; set; }
        }

        public class ParametroGeneral
        {
            public int IdUsuario { get; set; }
            public string Token { get; set; }
        }
    }
}
