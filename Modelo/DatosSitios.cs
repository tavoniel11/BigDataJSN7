
using NetTopologySuite.Geometries;
using Npgsql;
using NpgsqlTypes;
using System.Reflection;
using Backend.Datos;

namespace BigDataJSN7.Modelo
{
    public class DatosSitios
    {
        #region "------+Variables/Propiedades+------"
        private string NombreServicio = ServiciosMC.mc_Sitios.ToString();
        public static readonly string[] Directorio = { ServiciosMC.mc_Sitios.ToString() };
        public Utilidades Utilidades;
        public ServicioLog ObjServicio;
        #endregion

        #region "-----------+Constructor+-----------"
        public DatosSitios()
        {
            Utilidades = new Utilidades();
            if (Utilidades.DicServicios.ContainsKey(NombreServicio))
                ObjServicio = Utilidades.DicServicios[NombreServicio];
        }
        #endregion

        #region "-----------+SubirArchivo+-----------"
        public RespuestaSitios SubirArchivo(ParametrosSitios _Parametros, string _NombreArchivo, List<MigracionDetalle> _ListaDetalle, string _ClaveServicio)
        {
            RespuestaSitios Respuesta = new RespuestaSitios { Resultado = 1, Mensaje = "OK", Archivo = _NombreArchivo };
            string Query = string.Empty;
            string GeoPosicion = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Sitios, 1);

            try
            {
                Query = "INSERT INTO bigdata.sitio_migracion(" +
                    "int_idempresa, int_idambiente, int_idcategoria_sitio, int_idempresa_cliente, var_nombre, var_descripcion, int_estado, int_tipo," +
                    "dt_procesado, int_cargado, int_correcto, int_incorrecto, bol_enuso, int_idusuario_modifico, int_idusuario_registro," +
                    "dt_modificacion, dt_registro)" +
                    $"VALUES({_Parametros.IdEmpresa},{_Parametros.IdAmbiente},{_Parametros.IdCategoriaSitio},{_Parametros.IdEmpresaCliente}," +
                    $"@NombreArchivo,@DescripcionFolio,0,{_Parametros.Tipo},NULL,0,0,0,TRUE,{_Parametros.IdUsuario},{_Parametros.IdUsuario}," +
                    $"CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC',CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC');" +
                    $"SELECT CURRVAL('bigdata.sitio_migracion_int_id_seq');";

                Conexion.Open();
                using (var Transaccion = Conexion.BeginTransaction())
                {
                    try
                    {
                        int IdResultado;
                        using (NpgsqlCommand Cmd = new NpgsqlCommand(Query, Conexion))
                        {
                            Cmd.Parameters.AddWithValue("@NombreArchivo", NpgsqlDbType.Varchar, _NombreArchivo);
                            Cmd.Parameters.AddWithValue("@DescripcionFolio", NpgsqlDbType.Varchar, _Parametros.DescripcionFolio);
                            IdResultado = int.Parse(Cmd.ExecuteScalar().ToString());
                            if (IdResultado > 0)
                            {
                                Query = "INSERT INTO bigdata.sitio_migracion_detalle( " +
                                "int_idsitio_migracion, int_idempresa, int_idambiente, var_nombre, var_descripcion, g_posicion, int_radio, int_estado," +
                                "dt_procesado, bol_enuso, int_idusuario_modifico, int_idusuario_registro, dt_modificacion,dt_registro) " +
                                $"VALUES";

                                Respuesta.Resultado = IdResultado;

                                for (int i = 0; i < _ListaDetalle.Count; i++)
                                {
                                    MigracionDetalle Obj = _ListaDetalle.ElementAt(i);

                                    if (Obj.Nombre == string.Empty && Obj.Descripcion == string.Empty && Obj.Radio == 0)
                                    {
                                        continue;
                                    }
                                    if (i > 0)
                                    {
                                        Query += ",";
                                    }

                                    Coordinate Coordenada = new Coordinate(Obj.Longitud, Obj.Latitud);

                                    Point GeoCentro = new Point(Coordenada) { SRID = 4326 };

                                    byte[] BWKBPunto = GeoCentro.AsBinary();

                                    GeoPosicion = string.Format(" ST_GeomFromWKB({0},4326)", @"E'\\x" + BitConverter.ToString(BWKBPunto).Replace("-", "") + "'");

                                    Query += string.Format("({0},{1},{2},'{3}','{4}',{5},{6},{7},NULL,TRUE,{8},{9},",

                                        IdResultado,
                                        _Parametros.IdEmpresa,
                                        _Parametros.IdAmbiente,
                                        Obj.Nombre.Replace("'", "''"),
                                        Obj.Descripcion.Replace("'", "''"),
                                        GeoPosicion,
                                        Obj.Radio,
                                        Obj.Estado,
                                        _Parametros.IdUsuario,
                                        _Parametros.IdUsuario
                                        ) + " CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC',CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC')";
                                }
                            }
                        }
                        using (NpgsqlCommand Cmd = new NpgsqlCommand(Query, Conexion, Transaccion))
                        {
                            Cmd.ExecuteNonQuery();
                            Transaccion.Commit();
                        }
                    }
                    catch (Exception Ex)
                    {
                        Transaccion.Rollback();
                        Respuesta.Resultado = -2;
                        Respuesta.Mensaje = "Error: No se pudo completar el registro";
                        Respuesta.Archivo = _NombreArchivo;
                        Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
                    }

                }
                Conexion.Close();
            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Respuesta.Archivo = "";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            finally
            {
                if (Conexion != null && Conexion.State == System.Data.ConnectionState.Open)
                { Conexion.Close(); }
            }
            #region "----+Logs+----"
            try
            {
                if (ObjServicio.HabilitarLogServicio)
                {
                    Utilidades.LogDatos(new List<string> { "Peticiones" }, NombreServicio, MethodBase.GetCurrentMethod(), _ClaveServicio, Respuesta);
                }
            }
            catch { }
            #endregion

            return Respuesta;
        }
        #endregion
        public class MigracionDetalle
        {
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public double Latitud { get; set; }
            public double Longitud { get; set; }
            public double Radio { get; set; }
            public int Estado { get; set; }
        }

        public class ParametrosSitios : ParametroGeneral
        {
            public int IdEmpresa { get; set; }
            public int IdAmbiente { get; set; }
            public int IdCategoriaSitio { get; set; }
            public int IdEmpresaCliente { get; set; }
            public string DescripcionFolio { get; set; }
            public int Tipo { get; set; }
            public IFormFile Archivo { get; set; }
        }

        public class ParametroGeneral
        {
            public int IdUsuario { get; set; }
            public string Token { get; set; }
        }
    }

}
