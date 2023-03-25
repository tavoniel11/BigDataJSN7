using Npgsql;
using NpgsqlTypes;
using System.Reflection;
using Backend.Datos;
using System.Data;

namespace BigDataJSN7.Modelo
{
    public class DatosEmpresaServidor
    {
        #region "------+Variables/Propiedades+------"
        private string NombreServicio = ServiciosMC.mc_Sitios.ToString();
        public static readonly string[] Directorio = { ServiciosMC.mc_Sitios.ToString() };
        public Utilidades Utilidades;
        public ServicioLog ObjServicio;
        #endregion

        #region "-----------+Constructor+-----------"
        public DatosEmpresaServidor()
        {
            Utilidades = new Utilidades();
            if (Utilidades.DicServicios.ContainsKey(NombreServicio))
                ObjServicio = Utilidades.DicServicios[NombreServicio];
        }
        #endregion


        #region "---------+InsertarEmpresaServidor+---------"
        public RespuestaEmpresaServidor InsertarEmpresaServidor(ParametrosEmpresas _Parametros, string _ClaveServicio)
        {
            int Resultado = 0;
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Comandos, 1);
            RespuestaEmpresaServidor Objeto = new RespuestaEmpresaServidor() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };

            try
            {
                Conexion.Open();
                Query = string.Format("select count(1) from bigdata.tablas where int_id = {0};", _Parametros.int_idtabla);
                NpgsqlCommand Comm = new NpgsqlCommand(Query, Conexion);
                int tabla = Convert.ToInt32(Comm.ExecuteScalar());

                if (tabla !=0)
                {
                    if (_Parametros.int_idservidor > 0 && _Parametros.int_idambiente >0 && _Parametros.int_idempresa_embe > 0)
                    {
                        Query = string.Format("select count(1) from bigdata.tabla_empresa_sevidor where int_idtabla = {0} and int_tipo_particion=" +
                        "{1} and int_idempresa_embe={2};", _Parametros.int_idtabla, _Parametros.int_tipo_particion, _Parametros.int_idempresa_embe);
                        NpgsqlCommand Comm0 = new NpgsqlCommand(Query, Conexion);
                        int existencia = Convert.ToInt32(Comm0.ExecuteScalar());
                        if (existencia == 0)
                        {
                            if (_Parametros.int_tipo_particion > 0 && _Parametros.int_tipo_particion <= 6)
                            {
                                Query = "INSERT INTO bigdata.tabla_empresa_sevidor (int_idtabla, int_idservidor, int_tipo_particion, int_idempresa_embe, int_idambiente, " +
                                "int_idusuario_registro, int_idusuario_modifico, bol_enuso, dt_registro, dt_modificacion) " +
                                $"VALUES(@idtabla, @idservidor, @tipo,@idempresa,@idambiente, {_Parametros.IdUsuario}, {_Parametros.IdUsuario}, true, Current_timestamp(3) AT TIME ZONE 'UTC', " +
                                "Current_timestamp(3) AT TIME ZONE 'UTC') RETURNING int_id; ";


                                NpgsqlCommand Comm1 = new NpgsqlCommand(Query, Conexion);
                                Comm1.CommandType = CommandType.Text;
                                Comm1.Parameters.Add(new NpgsqlParameter("@idtabla", NpgsqlDbType.Integer)).Value = _Parametros.int_idtabla;
                                Comm1.Parameters.Add(new NpgsqlParameter("@idservidor", NpgsqlDbType.Integer)).Value = _Parametros.int_idservidor;
                                Comm1.Parameters.Add(new NpgsqlParameter("@tipo", NpgsqlDbType.Integer)).Value = _Parametros.int_tipo_particion;
                                Comm1.Parameters.Add(new NpgsqlParameter("@idempresa", NpgsqlDbType.Integer)).Value = _Parametros.int_idempresa_embe;
                                Comm1.Parameters.Add(new NpgsqlParameter("@idambiente", NpgsqlDbType.Integer)).Value = _Parametros.int_idambiente;


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
                                Objeto.Mensaje = "Error: tipo de partición no valido";
                            }
                        }
                        else
                        {
                            Objeto.Datos = -4;
                            Objeto.Estado = -4;
                            Objeto.Mensaje = "Error: particion tabla de empresa existente";
                        }
                    }
                    else
                    {
                        Objeto.Datos = -5;
                        Objeto.Estado = -5;
                        Objeto.Mensaje = "Error: Id de registro no valido";
                    }

                }
                else
                {
                    Objeto.Datos = -6;
                    Objeto.Estado = -6;
                    Objeto.Mensaje = "Error: Id tabla no existente";
                }
            }
            catch (Exception Ex)
            {
                Objeto.Datos = -1;
                Objeto.Estado = -1;
                Objeto.Mensaje = "Error general";

                Utilidades.RegistrarError(DatosEmpresaServidor.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
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

        #region "---------+EliminarEmpresaServidor+---------"

        public RespuestaEmpresaServidor EliminarEmpresaServidor(ParametrosDesactivarEmpresas _Parametros, string _ClaveServicio)
        {
            int Resultado = 0;
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Comandos, 1);
            RespuestaEmpresaServidor Objeto = new RespuestaEmpresaServidor() { Estado = 0, Datos = 0, Mensaje = "Error de datos" };

            try
            {
                if (_Parametros.int_id > 0)
                {
                    Conexion.Open();
                    Query = string.Format("select count(1) from bigdata.tabla_empresa_sevidor where int_id != {0};", _Parametros.int_id);
                    NpgsqlCommand Comm = new NpgsqlCommand(Query, Conexion);
                    int iddes = Convert.ToInt32(Comm.ExecuteScalar());

                    if (iddes != 0)
                    {
                        Query = $"UPDATE bigdata.tabla_empresa_sevidor SET bol_enuso=false, int_idusuario_modifico ={_Parametros.IdUsuario}," +
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

                Utilidades.RegistrarError(DatosEmpresaServidor.Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
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
        public class ParametrosEmpresas : ParametroGeneral
        {
            public int int_idtabla {  get; set; }
            public int int_idservidor { get; set; }
            public int int_tipo_particion { get; set; }
            public int int_idempresa_embe { get; set; }
            public int int_idambiente { get; set; }
            
        }

        public class ParametrosDesactivarEmpresas : ParametroGeneral
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
