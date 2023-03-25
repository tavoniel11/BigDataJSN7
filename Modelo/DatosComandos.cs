using Backend.Datos;
using BigDataJSN7.Controllers;
using NHibernate.Criterion;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Reflection;

namespace BigDataJSN7.Modelo
{
    public class DatosComandos
    {
        #region "------+Variables/Propiedades+------"

        public Utilidades Utilidades;
        public ServicioLog ObjServicio;

        private string NombreServicio = ServiciosMC.mc_Comandos.ToString();
        public static readonly string[] Directorio = { ServiciosMC.mc_Comandos.ToString() };

        #endregion

        #region "-----------+Constructor+-----------"
        public DatosComandos()
        {
            //Inicializando 
            Utilidades = new Utilidades();
            ObjServicio = new ServicioLog();

            if (Utilidades.DicServicios.ContainsKey(NombreServicio))
                ObjServicio = Utilidades.DicServicios[NombreServicio];
        }
        #endregion

        #region "---------+InsertarComando+---------"
        public RespuestaComandos InsertarComando(ParametrosComando _Parametros, string _ClaveServicio)
        {
            int Resultado = 0;
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Comandos, 1);
            RespuestaComandos Objeto = new RespuestaComandos() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };

            try
            {
                if (!string.IsNullOrEmpty(_Parametros.Etiqueta))
                {
                    Conexion.Open();
                    Query = string.Format("select count(1) from bigdata.comando_dispositivo where trim(lower(var_etiqueta)) = '{0}'", _Parametros.Etiqueta.Trim().ToLower());

                    NpgsqlCommand Comm = new NpgsqlCommand(Query, Conexion);
                    int Name = Convert.ToInt32(Comm.ExecuteScalar());

                    if (Name == 0)
                    {
                        Query = "INSERT INTO bigdata.comando_dispositivo(var_etiqueta,int_usuario_modifico,int_usuario_registro,dt_modificacion,dt_registro)" +
                        "VALUES (@Etiqueta,@UserMod,@UserReg,Current_timestamp(3) AT TIME ZONE 'UTC', Current_timestamp(3) AT TIME ZONE 'UTC');" +
                        "SELECT currval('bigdata.comando_dispositivo_int_id_seq');";

                        NpgsqlCommand Comm0 = new NpgsqlCommand(Query, Conexion);
                        Comm0.CommandType = CommandType.Text;
                        Comm0.Parameters.Add(new NpgsqlParameter("@Etiqueta", NpgsqlDbType.Varchar)).Value = _Parametros.Etiqueta;
                        Comm0.Parameters.Add(new NpgsqlParameter("@UserMod", NpgsqlDbType.Integer)).Value = _Parametros.IdUsuario;
                        Comm0.Parameters.Add(new NpgsqlParameter("@UserReg", NpgsqlDbType.Integer)).Value = _Parametros.IdUsuario;
                        Resultado = Convert.ToInt32(Comm0.ExecuteScalar());

                        if (Resultado > 0)
                        {
                            Objeto.Datos = Resultado;
                            Objeto.Estado = 1;
                            Objeto.Mensaje = "Ok";
                        }
                        else
                        {
                            Objeto.Datos = -2;
                            Objeto.Estado = -2;
                            Objeto.Mensaje = "Error: No se pudo completar el registro";
                        }
                    }
                    else
                    {
                        Objeto.Datos = -3;
                        Objeto.Estado = -3;
                        Objeto.Mensaje = "Error: Nombre de la etiqueta existente";
                    }
                }
                else
                {
                    Objeto.Datos = -4;
                    Objeto.Estado = -4;
                    Objeto.Mensaje = "Error: Etiqueta no valido";
                }
            }
            catch (Exception Ex)
            {
                Objeto.Datos = -1;
                Objeto.Estado = -1;
                Objeto.Mensaje = "Error general";

                Utilidades.RegistrarError(DatosComandos.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }

            #region "----+Logs+----"
            try
            {
                if (ObjServicio.HabilitarLogServicio)
                {
                    Utilidades.LogDatos(new List<string> { "Peticiones" }, NombreServicio, MethodBase.GetCurrentMethod(), _ClaveServicio, Objeto);
                }
            }
            catch { }

            #endregion

            return Objeto;
        }

        #endregion

        #region "---------+ModificarComando+--------"
        public RespuestaComandos ModificarComando(ParametroEditarComando _Parametros, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Comandos, 2);
            RespuestaComandos Objeto = new RespuestaComandos() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };

            try
            {
                if (!string.IsNullOrEmpty(_Parametros.Etiqueta))
                {
                    if (_Parametros.IdComando > 0)
                    {
                        Conexion.Open();
                        Query = string.Format("select count(1) from bigdata.comando_dispositivo where int_id != {0} and trim(lower(var_etiqueta)) = '{1}';", _Parametros.IdComando, _Parametros.Etiqueta.ToLower().Trim());
                        NpgsqlCommand Comm = new NpgsqlCommand(Query, Conexion);
                        int Update = Convert.ToInt32(Comm.ExecuteScalar());

                        if (Update == 0)
                        {
                            Query = "UPDATE bigdata.comando_dispositivo SET var_etiqueta=@Etiqueta,int_usuario_modifico = @UserMod," +
                                    "dt_modificacion=Current_timestamp(3) AT TIME ZONE 'UTC' WHERE int_id=@intId";

                            NpgsqlCommand Comm0 = new NpgsqlCommand(Query, Conexion);
                            Comm0.CommandType = CommandType.Text;
                            Comm0.Parameters.Add(new NpgsqlParameter("@Etiqueta", NpgsqlDbType.Varchar)).Value = _Parametros.Etiqueta;
                            Comm0.Parameters.Add(new NpgsqlParameter("@UserMod", NpgsqlDbType.Integer)).Value = _Parametros.IdUsuario;
                            Comm0.Parameters.Add(new NpgsqlParameter("@intId", NpgsqlDbType.Integer)).Value = _Parametros.IdComando;
                            int Rows = Comm0.ExecuteNonQuery();
                            if (Rows > 0)
                            {
                                Objeto.Estado = 1;
                                Objeto.Mensaje = "Ok";
                                Objeto.Datos = 1;
                            }
                            else
                            {
                                Objeto.Estado = -2;
                                Objeto.Mensaje = "Error: No se completo la modificacion";
                                Objeto.Datos = -2;
                            }
                        }
                        else
                        {
                            Objeto.Datos = -3;
                            Objeto.Estado = -3;
                            Objeto.Mensaje = "Error: El nombre de la etiqueta ya se encuentra registrado";
                        }
                        Conexion.Close();
                    }
                    else
                    {
                        Objeto.Datos = -4;
                        Objeto.Estado = -4;
                        Objeto.Mensaje = "Error: El IdComando debe ser mayor a 0";
                    }
                }
                else
                {
                    Objeto.Datos = -5;
                    Objeto.Estado = -5;
                    Objeto.Mensaje = "Error: Etiqueta no valido";
                }
            }
            catch (Exception Ex)
            {
                Objeto.Datos = -1;
                Objeto.Estado = -1;
                Objeto.Mensaje = "Error general";
                Utilidades.RegistrarError(DatosComandos.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
            }

            #region "----+Logs+----"
            try
            {
                if (ObjServicio.HabilitarLogServicio)
                {
                    Utilidades.LogDatos(new List<string> { "Peticiones" }, NombreServicio, MethodBase.GetCurrentMethod(), _ClaveServicio, Objeto);
                }
            }
            catch { }

            #endregion

            return Objeto;
        }

        #endregion

        #region "----------+MigrarComando+----------"
        public RespuestaComandos MigrarComando(ParametrosMigrarComando _Parametros, string _ClaveServicio)
        {
            int Resultado = 0;
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Comandos, 3);
            NpgsqlConnection ConexionEtiqueta = Utilidades.ObtenerConexion(ServiciosMC.mc_Comandos, 4);
            RespuestaComandos Objeto = new RespuestaComandos() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };

            try
            {
                if (_Parametros.IdComando > 0)
                {
                    ConexionEtiqueta.Open();

                    Query = string.Format("select count(1) from catalogos.tbl_etiqueta_comando where int_idcomando = {0}", _Parametros.IdComando);

                    NpgsqlCommand Comm = new NpgsqlCommand(Query, ConexionEtiqueta);
                    int Name = Convert.ToInt32(Comm.ExecuteScalar());

                    if (Name == 0)
                    {
                        Conexion.Open();
                        string Etiqueta = string.Empty;
                        string Query1 = string.Format("select var_etiqueta from bigdata.comando_dispositivo where int_id = {0}", _Parametros.IdComando);

                        NpgsqlCommand Comm0 = new NpgsqlCommand(Query1, Conexion);
                        NpgsqlDataReader dr = Comm0.ExecuteReader();
                        while (dr.Read())
                        {
                            Etiqueta = dr["var_etiqueta"].ToString();
                        }
                        dr.Close();

                        if (!string.IsNullOrEmpty(Etiqueta))
                        {
                            Query = "INSERT INTO catalogos.tbl_etiqueta_comando(int_idempresa,int_idcomando,var_etiqueta,bol_publico,bol_distribuidor,int_usuario_modifico,int_usuario_registro,dt_modificacion,dt_registro)" +
                            "VALUES (@IdEmpresa,@IdComando,@Etiqueta,@BolPublico,@Bol_Distribuidor,@IdUsuarioMod,@IdUsuarioReg,Current_timestamp(3) AT TIME ZONE 'UTC', Current_timestamp(3) AT TIME ZONE 'UTC');" +
                            "SELECT currval('catalogos.tbl_etiqueta_comando_int_id_seq');";

                            NpgsqlCommand Comm1 = new NpgsqlCommand(Query, ConexionEtiqueta);
                            Comm1.CommandType = CommandType.Text;
                            Comm1.Parameters.Add(new NpgsqlParameter("@IdEmpresa", NpgsqlDbType.Integer)).Value = _Parametros.IdEmpresa;
                            Comm1.Parameters.Add(new NpgsqlParameter("@IdComando", NpgsqlDbType.Integer)).Value = _Parametros.IdComando;
                            Comm1.Parameters.Add(new NpgsqlParameter("@Etiqueta", NpgsqlDbType.Varchar)).Value = Etiqueta;
                            Comm1.Parameters.Add(new NpgsqlParameter("@BolPublico", NpgsqlDbType.Boolean)).Value = _Parametros.BoolPublico;
                            Comm1.Parameters.Add(new NpgsqlParameter("@Bol_Distribuidor", NpgsqlDbType.Boolean)).Value = _Parametros.BoolDistribuidor;
                            Comm1.Parameters.Add(new NpgsqlParameter("@IdUsuarioMod", NpgsqlDbType.Integer)).Value = _Parametros.IdUsuario;
                            Comm1.Parameters.Add(new NpgsqlParameter("@IdUsuarioReg", NpgsqlDbType.Integer)).Value = _Parametros.IdUsuario;
                            Resultado = Convert.ToInt32(Comm1.ExecuteScalar());

                            if (Resultado > 0)
                            {
                                Objeto.Datos = Resultado;
                                Objeto.Estado = 1;
                                Objeto.Mensaje = "Ok";
                            }
                            else
                            {
                                Objeto.Datos = -2;
                                Objeto.Estado = -2;
                                Objeto.Mensaje = "Error: No se pudo completar el registro";
                            }
                        }
                        else
                        {
                            Objeto.Datos = -3;
                            Objeto.Estado = -3;
                            Objeto.Mensaje = "Error: Etiqueta no valido";
                        }
                        Conexion.Close();
                    }
                    else
                    {
                        Objeto.Datos = -4;
                        Objeto.Estado = -4;
                        Objeto.Mensaje = "Error: Ya se encuentra registrado";
                    }
                    ConexionEtiqueta.Close();
                }
                else
                {
                    Objeto.Datos = -5;
                    Objeto.Estado = -5;
                    Objeto.Mensaje = "Error: El IdComando debe ser mayor a 0";
                }
            }
            catch (Exception Ex)
            {
                Objeto.Datos = -1;
                Objeto.Estado = -1;
                Objeto.Mensaje = "Error general";
                Utilidades.RegistrarError(DatosComandos.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion.State != ConnectionState.Closed)
                    Conexion.Close();
                if (ConexionEtiqueta.State != ConnectionState.Closed)
                    ConexionEtiqueta.Close();
            }

            #region "----+Logs+----"
            try
            {
                if (ObjServicio.HabilitarLogServicio)
                {
                    Utilidades.LogDatos(new List<string> { "Peticiones" }, NombreServicio, MethodBase.GetCurrentMethod(), _ClaveServicio, Objeto);
                }
            }
            catch { }

            #endregion

            return Objeto;
        }
        #endregion

        public class ParametrosComando : ParametroGeneral
        {
            public string Etiqueta { get; set; }

        }

        public class ParametroEditarComando : ParametroGeneral
        {
            public int IdComando { get; set; }
            public string Etiqueta { get; set; }
            public int IdUsuario { get; set; }

        }
        public class ParametroGeneral
        {
            public int IdUsuario { get; set; }
            public string Token { get; set; }
        }
        public class ParametrosMigrarComando:ParametroGeneral
        {
            public  int IdComando { get; set; }
            public  int IdEmpresa { get; set; }
            public  Boolean BoolPublico { get; set; }
            public  Boolean BoolDistribuidor { get; set; }
        }
    }
}
