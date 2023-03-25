
using Npgsql;
using NpgsqlTypes;
using System.Reflection;
using Backend.Datos;
using System.Data;
using Microsoft.IdentityModel.Protocols.WsTrust;

namespace BigDataJSN7.Modelo
{
    public class DatosTablas
    {
        #region "------+Variables/Propiedades+------"
        private string NombreServicio = ServiciosMC.mc_Sitios.ToString();
        public static readonly string[] Directorio = { ServiciosMC.mc_Sitios.ToString() };
        public Utilidades Utilidades;
        public ServicioLog ObjServicio;
        #endregion

        #region "-----------+Constructor+-----------"
        public DatosTablas()
        {
            Utilidades = new Utilidades();
            if (Utilidades.DicServicios.ContainsKey(NombreServicio))
                ObjServicio = Utilidades.DicServicios[NombreServicio];
        }
        #endregion


        #region "---------+InsertarTabla+---------"
        public RespuestaTablas InsertarTabla(ParametrosTablas _Parametros, string _ClaveServicio)
        {
            int Resultado = 0;
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Comandos, 1);
            RespuestaTablas Objeto = new RespuestaTablas() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };

            try
            {
                if (!string.IsNullOrEmpty(_Parametros.var_nombre))
                {
                    if (!string.IsNullOrEmpty(_Parametros.var_schema))
                    {
                        Conexion.Open();
                        Query = string.Format("select count(1) from bigdata.tablas where trim(lower(var_nombre)) = '{0}' and trim(lower(var_schema)) = '{1}'", _Parametros.var_nombre.ToLower().Trim(), _Parametros.var_schema.ToLower().Trim());
                        NpgsqlCommand Comm = new NpgsqlCommand(Query, Conexion);
                        int existencia = Convert.ToInt32(Comm.ExecuteScalar());
                        if (existencia ==0)
                        {
                            if (_Parametros.int_tipo_particion_alternativo >= 0 && _Parametros.int_tipo_particion_alternativo <= 6)
                            {
                                Query = "INSERT INTO bigdata.tablas (var_schema, var_nombre, bol_activo, bol_enuso, int_idusuario_registro, int_idusuario_modifico, dt_registro, " +
                                "dt_modificacion, int_tipo_particion_alternativo) " +
                                $"VALUES(@schematbl, @nombretbl, true, true, {_Parametros.IdUsuario}, {_Parametros.IdUsuario}, Current_timestamp(3) AT TIME ZONE 'UTC', " +
                                "Current_timestamp(3) AT TIME ZONE 'UTC', @tipo) RETURNING int_id; ";

                                NpgsqlCommand Comm1 = new NpgsqlCommand(Query, Conexion);
                                Comm1.CommandType = CommandType.Text;
                                Comm1.Parameters.Add(new NpgsqlParameter("@schematbl", NpgsqlDbType.Varchar)).Value = _Parametros.var_schema.ToLower();
                                Comm1.Parameters.Add(new NpgsqlParameter("@nombretbl", NpgsqlDbType.Varchar)).Value = _Parametros.var_nombre.ToLower();
                                Comm1.Parameters.Add(new NpgsqlParameter("@tipo", NpgsqlDbType.Array | NpgsqlDbType.Integer)).Value = new[] { _Parametros.int_tipo_particion_alternativo };

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
                                Objeto.Mensaje = "Error: Tipo de particion no valido";
                            }
                        }
                        else
                        {
                            Objeto.Datos = -4;
                            Objeto.Estado = -4;
                            Objeto.Mensaje = "Error: Nombre de la tabla existente en schema o viceversa";
                        }
                    }
                    else
                    {
                        Objeto.Datos = -5;
                        Objeto.Estado = -5;
                        Objeto.Mensaje = "Error: schema tabla no valido";
                    }

                }
                else
                {
                    Objeto.Datos = -6;
                    Objeto.Estado = -6;
                    Objeto.Mensaje = "Error: nombre tabla no valido";
                }
            }
            catch (Exception Ex)
            {
                Objeto.Datos = -1;
                Objeto.Estado = -1;
                Objeto.Mensaje = "Error general";

                Utilidades.RegistrarError(DatosTablas.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
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

        #region "---------+ModificarTabla+---------"

        public RespuestaTablas ModificarTabla(ParametrosEditarTablas _Parametros, string _ClaveServicio)
        {
            int Resultado = 0;
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Comandos, 1);
            RespuestaTablas Objeto = new RespuestaTablas() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };

            try
            {
                if (_Parametros.int_id > 0) 
                { 
                    if (!string.IsNullOrEmpty(_Parametros.var_nombre))
                    {
                        if (!string.IsNullOrEmpty(_Parametros.var_schema))
                        {
                            Conexion.Open();
                            Query = string.Format("select count(1) from bigdata.tablas where trim(lower(var_nombre)) = '{0}' and trim(lower(var_schema)) = '{1}'", _Parametros.var_nombre.ToLower().Trim(), _Parametros.var_schema.ToLower().Trim());
                            NpgsqlCommand Comm = new NpgsqlCommand(Query, Conexion);
                            int existencia = Convert.ToInt32(Comm.ExecuteScalar());
                            if (existencia == 0)
                            {
                                if (_Parametros.int_tipo_particion_alternativo >= 0 && _Parametros.int_tipo_particion_alternativo <= 6)
                                {
                                    Query = $"UPDATE bigdata.tablas SET var_schema=@schematbl,var_nombre = @nombretbl, int_idusuario_modifico ={_Parametros.IdUsuario}," +
                                           "dt_modificacion=Current_timestamp(3) AT TIME ZONE 'UTC', int_tipo_particion_alternativo=@tipo WHERE int_id=@idtbl RETURNING int_id;";

                                    NpgsqlCommand Comm1 = new NpgsqlCommand(Query, Conexion);
                                    Comm1.CommandType = CommandType.Text;
                                    Comm1.Parameters.Add(new NpgsqlParameter("@idtbl", NpgsqlDbType.Integer)).Value = _Parametros.int_id;

                                    Comm1.Parameters.Add(new NpgsqlParameter("@schematbl", NpgsqlDbType.Varchar)).Value = _Parametros.var_schema.ToLower();
                                    Comm1.Parameters.Add(new NpgsqlParameter("@nombretbl", NpgsqlDbType.Varchar)).Value = _Parametros.var_nombre.ToLower();
                                    Comm1.Parameters.Add(new NpgsqlParameter("@tipo", NpgsqlDbType.Array | NpgsqlDbType.Integer)).Value = new[] { _Parametros.int_tipo_particion_alternativo };

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
                                    Objeto.Mensaje = "Error: Tipo de particion no valido";
                                }
                            }
                            else
                            {
                                Objeto.Datos = -4;
                                Objeto.Estado = -4;
                                Objeto.Mensaje = "Error: Nombre de la tabla existente en schema o viceversa";
                            }
                        }
                        else
                        {
                            Objeto.Datos = -5;
                            Objeto.Estado = -5;
                            Objeto.Mensaje = "Error: schema tabla no valido";
                        }

                    }
                    else
                    {
                        Objeto.Datos = -6;
                        Objeto.Estado = -6;
                        Objeto.Mensaje = "Error: nombre tabla no valido";
                    }

                }
                else
                {
                    Objeto.Datos = -7;
                    Objeto.Estado = -7;
                    Objeto.Mensaje = "Error: id tabla no valido";
                }
            }
            catch (Exception Ex)
            {
                Objeto.Datos = -1;
                Objeto.Estado = -1;
                Objeto.Mensaje = "Error general";

                Utilidades.RegistrarError(DatosTablas.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
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

        #region "---------+DesactivarTabla+---------"

        public RespuestaTablas DesactivarTabla(ParametrosDesactivarTablas _Parametros, string _ClaveServicio)
        {
            int Resultado = 0;
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Comandos, 1);
            RespuestaTablas Objeto = new RespuestaTablas() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };

            try
            {
                if (_Parametros.int_id > 0)
                {
                    Conexion.Open();
                    Query = string.Format("select count(1) from bigdata.tablas where int_id != {0};", _Parametros.int_id);
                    NpgsqlCommand Comm = new NpgsqlCommand(Query, Conexion);
                    int iddes = Convert.ToInt32(Comm.ExecuteScalar());

                    if (iddes != 0)
                    {
                        Query = $"UPDATE bigdata.tablas SET bol_activo=false, int_idusuario_modifico ={_Parametros.IdUsuario}," +
                                   "dt_modificacion=Current_timestamp(3) AT TIME ZONE 'UTC' WHERE int_id=@idtbl RETURNING int_id;";

                        NpgsqlCommand Comm0 = new NpgsqlCommand(Query, Conexion);
                        Comm0.CommandType = CommandType.Text;
                        Comm0.Parameters.Add(new NpgsqlParameter("@idtbl", NpgsqlDbType.Integer)).Value = _Parametros.int_id;
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
                            Objeto.Mensaje = "Error: No se pudo desactivar la tabla";
                        }
                    }
                    else
                    {
                        Objeto.Datos = -3;
                        Objeto.Estado = -3;
                        Objeto.Mensaje = "Error: problemas con el Id";
                    }
                }
                else
                {
                    Objeto.Datos = -4;
                    Objeto.Estado = -4;
                    Objeto.Mensaje = "Error: El Idtabla debe ser mayor a 0";
                }
            }
            catch (Exception Ex)
            {
                Objeto.Datos = -1;
                Objeto.Estado = -1;
                Objeto.Mensaje = "Error general";

                Utilidades.RegistrarError(DatosTablas.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
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

        public class ParametrosTablas : ParametroGeneral
        {
            public string var_schema { get; set; }
            public string var_nombre { get; set; }
            public int int_tipo_particion_alternativo { get; set; }
        }

        public class ParametrosEditarTablas : ParametroGeneral
        {
            public int int_id { get; set; }
            public string var_schema { get; set; }
            public string var_nombre { get; set; }
            public int int_tipo_particion_alternativo { get; set; }
        }

        public class ParametrosDesactivarTablas : ParametroGeneral
        {
            public int int_id { get; set; }
        }

        public class ParametroGeneral
        {
            public int IdUsuario { get; set; }
            public string Token { get; set; }
        }

    }

}
