using Backend.Datos;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Reflection;


namespace BigDataJSN7.Modelo
{
    public class DatosInteraccion
    {
        #region "------+Variables/Propiedades+------"

        public Utilidades Utilidades;
        public ServicioLog ObjServicio;

        private string NombreServicio = ServiciosMC.mc_Interaccion.ToString();
        public static readonly string[] Directorio = { ServiciosMC.mc_Interaccion.ToString() };

        #endregion

        #region "-----------+Constructor+-----------"
        public DatosInteraccion()
        {
            Utilidades = new Utilidades();
            ObjServicio = new ServicioLog();

            if (Utilidades.DicServicios.ContainsKey(NombreServicio))
                ObjServicio = Utilidades.DicServicios[NombreServicio];
        }
        #endregion

        #region "---------+NuevaInteraccion+---------"
        public RespuestaInteraccion NuevaInteraccion(ParametrosInteraccion _Parametros, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Interaccion, 1);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };

            try
            {

                if (!string.IsNullOrEmpty(_Parametros.NombrePlantilla) || !string.IsNullOrWhiteSpace(_Parametros.NombrePlantilla))
                {
                    int ResultadoPlantilla = ExistePlantilla(_Parametros.NombrePlantilla, 0, _ClaveServicio);

                    if (ResultadoPlantilla == 0)
                    {
                        Query = "INSERT INTO bigdata.as_plantilla (" +
                            "var_nombre, var_titulo, var_mensaje ,bol_enuso, int_idusuario_modifico, int_idusuario_registro ,dt_modificacion, dt_registro)" +
                            $"VALUES(@NombrePlantilla,@TituloPlantilla,@MensajePlantilla,TRUE,{_Parametros.IdUsuario},{_Parametros.IdUsuario}," +
                            $"CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC',CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC'); " +
                            $"SELECT CURRVAL('bigdata.as_plantilla_int_id_seq');";

                        Conexion.Open();
                        var Transaccion = Conexion.BeginTransaction();

                        try
                        {
                            int IdResultado;
                            NpgsqlCommand Cmd2 = new NpgsqlCommand(Query, Conexion);
                            Cmd2.Parameters.AddWithValue("@NombrePlantilla", NpgsqlDbType.Varchar, _Parametros.NombrePlantilla);
                            Cmd2.Parameters.AddWithValue("@TituloPlantilla", NpgsqlDbType.Varchar, _Parametros.TituloPlantilla);
                            Cmd2.Parameters.AddWithValue("@MensajePlantilla", NpgsqlDbType.Varchar, _Parametros.MensajePlantilla);
                            IdResultado = int.Parse(Cmd2.ExecuteScalar().ToString());
                            if (IdResultado > 0)
                            {
                                if (!string.IsNullOrEmpty(_Parametros.Nombre))
                                {
                                    if (!string.IsNullOrWhiteSpace(_Parametros.NombrePlantilla))
                                    {
                                        int ResultadoInteraccion = ExisteInteraccion(_Parametros.Nombre, 0, _ClaveServicio);

                                        if (ResultadoInteraccion == 0)
                                        {
                                            Query = "INSERT INTO bigdata.as_interaccion(" +
                                                "var_nombre, int_idas_plantilla, bol_esinmediato, bol_estiempo, bol_esvelocidad,bol_escomando, bol_esdefault, var_comandos_activan, var_comandos_terminan," +
                                                "int_dependiente, int_maxima_flotilla, int_categoria, int_modulo, int_idas_plantilla_dependiente," +
                                                "int_idas_interaccion_dependiente, bol_enuso, int_idusuario_registro, dt_modificacion, dt_registro) " +
                                                $"VALUES ";

                                            Respuesta.Resultado = IdResultado;

                                            Query += string.Format("('{0}',{1},'{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9},{10},{11},{12},{13},{14},TRUE,{15},",


                                              _Parametros.Nombre,
                                              IdResultado,
                                              _Parametros.EsInmediato,
                                              _Parametros.EsTiempo,
                                              _Parametros.EsVelocidad,
                                              _Parametros.EsComando,
                                              _Parametros.EsDefault,
                                              _Parametros.ComandosActivan,
                                              _Parametros.ComandosTerminan,
                                              _Parametros.Dependiente,
                                              _Parametros.MaximaFlotilla,
                                              _Parametros.Categoria,
                                              _Parametros.Modulo,
                                              _Parametros.IdAsPlatillaDependiente,
                                              _Parametros.IdAsInteraccionDependiente,
                                              _Parametros.IdUsuario
                                              ) + " CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC',CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC');";

                                            NpgsqlCommand Cmd4 = new NpgsqlCommand(Query, Conexion, Transaccion);

                                            Cmd4.ExecuteNonQuery();
                                            Transaccion.Commit();
                                            Respuesta.Resultado = IdResultado;
                                            Respuesta.Mensaje = "Ok";
                                        }
                                        else
                                        {
                                            Respuesta.Resultado = -5;
                                            Respuesta.Mensaje = "Error: Nombre de la interaccion existente";
                                        }
                                    }
                                    else
                                    {
                                        Respuesta.Resultado = -6;
                                        Respuesta.Mensaje = "Error: Nombre de la interaccion no valido";
                                    }
                                }
                                else
                                {
                                    Respuesta.Resultado = -6;
                                    Respuesta.Mensaje = "Error: Nombre de la interaccion no valido";
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
                        Respuesta.Mensaje = "Error: Nombre de plantilla existente";
                    }
                }
                else
                {
                    Respuesta.Resultado = -4;
                    Respuesta.Mensaje = "Error: Nombre de plantilla no valido";
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

        #region "---------+EditarInteraccion+--------"
        public RespuestaInteraccion EditarInteraccion(ParametrosEditarInteraccion _Parametros, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Interaccion, 2);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };

            try
            {
                if (!string.IsNullOrEmpty(_Parametros.Nombre) || !string.IsNullOrWhiteSpace(_Parametros.Nombre))
                {
                    if (_Parametros.IdInteraccion > 0)
                    {
                        int ResultadoInteraccion = ExisteInteraccion(_Parametros.Nombre, _Parametros.IdInteraccion, _ClaveServicio);

                        if (ResultadoInteraccion == 0)
                        {
                            Query = "UPDATE bigdata.as_interaccion SET " +
                               "var_nombre=@NombreInteraccion,bol_esinmediato=@EsInmediato,bol_estiempo=@EsTiempo,bol_esvelocidad=@EsVelocidad," +
                               "bol_escomando=@EsComando,bol_esdefault=@EsDefault,var_comandos_activan=@ComandosActivan,var_comandos_terminan=@ComandosTerminan," +
                               "int_dependiente=@Dependiente,int_maxima_flotilla=@MaximaFlotilla,int_categoria=@Categoria,int_modulo=@Modulo," +
                               "int_idas_plantilla_dependiente=@IdAsPlatillaDependiente,int_idas_interaccion_dependiente=@IdAsInteraccionDependiente," +
                               "int_idusuario_modifico=@IdUsuario,dt_modificacion=Current_timestamp(3) AT TIME ZONE 'UTC' " +
                               "WHERE int_id=@IdInteraccion AND bol_enuso";

                            Conexion.Open();
                            NpgsqlCommand Cmd2 = new NpgsqlCommand(Query, Conexion);
                            Cmd2.CommandType = CommandType.Text;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@NombreInteraccion", NpgsqlDbType.Varchar)).Value = _Parametros.Nombre;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@EsInmediato", NpgsqlDbType.Boolean)).Value = _Parametros.EsInmediato;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@EsTiempo", NpgsqlDbType.Boolean)).Value = _Parametros.EsTiempo;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@EsVelocidad", NpgsqlDbType.Boolean)).Value = _Parametros.EsVelocidad;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@EsComando", NpgsqlDbType.Boolean)).Value = _Parametros.EsComando;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@EsDefault", NpgsqlDbType.Boolean)).Value = _Parametros.EsDefault;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@ComandosActivan", NpgsqlDbType.Varchar)).Value = _Parametros.ComandosActivan;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@ComandosTerminan", NpgsqlDbType.Varchar)).Value = _Parametros.ComandosTerminan;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@Dependiente", NpgsqlDbType.Integer)).Value = _Parametros.Dependiente;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@MaximaFlotilla", NpgsqlDbType.Integer)).Value = _Parametros.MaximaFlotilla;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@Categoria", NpgsqlDbType.Integer)).Value = _Parametros.Categoria;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@Modulo", NpgsqlDbType.Integer)).Value = _Parametros.Modulo;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@IdAsPlatillaDependiente", NpgsqlDbType.Integer)).Value = _Parametros.IdAsPlatillaDependiente;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@IdAsInteraccionDependiente", NpgsqlDbType.Integer)).Value = _Parametros.IdAsInteraccionDependiente;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@IdUsuario", NpgsqlDbType.Integer)).Value = _Parametros.IdUsuario;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@IdInteraccion", NpgsqlDbType.Integer)).Value = _Parametros.IdInteraccion;
                            int IdResultado = Cmd2.ExecuteNonQuery();

                            if (IdResultado > 0)
                            {
                                Respuesta.Mensaje = "Ok";
                                Respuesta.Resultado = IdResultado;
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
                            Respuesta.Mensaje = "Error: El nombre de la interaccion ya se encuentra registrado";
                        }
                        Conexion.Close();
                    }
                    else
                    {
                        Respuesta.Resultado = -4;
                        Respuesta.Mensaje = "Error: El IdInteraccion debe ser mayor a 0";
                    }
                }
                else
                {
                    Respuesta.Resultado = -5;
                    Respuesta.Mensaje = "Error: Nombre de Interaccion no valido";
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

        #region "---------+EditarPlantilla+--------"
        public RespuestaInteraccion EditarPlantilla(ParametrosEditarPlantilla _Parametros, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Interaccion, 3);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };

            try
            {
                if (!string.IsNullOrEmpty(_Parametros.NombrePlantilla) || !string.IsNullOrWhiteSpace(_Parametros.NombrePlantilla))
                {

                    if (_Parametros.IdPlantilla > 0)
                    {
                        int ResultadoPlantilla = ExistePlantilla(_Parametros.NombrePlantilla, _Parametros.IdPlantilla, _ClaveServicio);

                        if (ResultadoPlantilla == 0)
                        {
                            Query = "UPDATE bigdata.as_plantilla SET " +
                                "var_nombre=@NombrePlantilla,var_titulo=@TituloPlantilla," +
                                "var_mensaje=@MensajePlantilla,int_idusuario_modifico=@IdUsuario," +
                                "dt_modificacion=Current_timestamp(3) AT TIME ZONE 'UTC' " +
                                "WHERE int_id=@IdPlantilla ";

                            Conexion.Open();
                            NpgsqlCommand Cmd2 = new NpgsqlCommand(Query, Conexion);
                            Cmd2.CommandType = CommandType.Text;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@NombrePlantilla", NpgsqlDbType.Varchar)).Value = _Parametros.NombrePlantilla;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@TituloPlantilla", NpgsqlDbType.Varchar)).Value = _Parametros.TituloPlantilla;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@MensajePlantilla", NpgsqlDbType.Varchar)).Value = _Parametros.MensajePlantilla;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@IdUsuario", NpgsqlDbType.Integer)).Value = _Parametros.IdUsuario;
                            Cmd2.Parameters.Add(new NpgsqlParameter("@IdPlantilla", NpgsqlDbType.Integer)).Value = _Parametros.IdPlantilla;
                            int IdResultado = Cmd2.ExecuteNonQuery();

                            if (IdResultado > 0)
                            {
                                Respuesta.Mensaje = "Ok";
                                Respuesta.Resultado = IdResultado;
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
                            Respuesta.Mensaje = "Error: El nombre de la plantilla ya se encuentra registrado";
                        }
                        Conexion.Close();
                    }
                    else
                    {
                        Respuesta.Resultado = -4;
                        Respuesta.Mensaje = "Error: El IdPlantilla debe ser mayor a 0";
                    }
                }
                else
                {
                    Respuesta.Resultado = -5;
                    Respuesta.Mensaje = "Error: Nombre de plantilla no valido";
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

        #region "----------+MigrarInteraccion+----------"
        public RespuestaInteraccion MigrarInteraccion(ParametrosMigrarInteraccion _Parametros, string _ClaveServicio)
        {
            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Interaccion, 3);
            NpgsqlConnection ConexionNombre = Utilidades.ObtenerConexion(ServiciosMC.mc_Interaccion, 4);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };

            try
            {
                if (!string.IsNullOrEmpty(_Parametros.NombrePlantilla) || !string.IsNullOrWhiteSpace(_Parametros.NombrePlantilla))
                {
                    int ResultadoPlantilla = ExistePlantillaCatalogo(_Parametros.NombrePlantilla, _ClaveServicio);

                    if (ResultadoPlantilla == 0)
                    {
                        Conexion.Open();
                        Query = "INSERT INTO catalogos.tbl_as_plantilla (" +
                            "var_nombre, var_titulo, var_mensaje ,bol_enuso, int_usuario_modifico,dt_modificacion)" +
                            $"VALUES(@NombrePlantilla,@TituloPlantilla,@MensajePlantilla,TRUE,{_Parametros.IdUsuario}," +
                            $"CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC') ; " +
                            $"SELECT CURRVAL('catalogos.tbl_as_plantilla_int_id_seq');";

                        var Transaccion = Conexion.BeginTransaction();

                        try
                        {
                            int IdResultado;
                            NpgsqlCommand Cmd2 = new NpgsqlCommand(Query, Conexion);
                            Cmd2.Parameters.AddWithValue("@NombrePlantilla", NpgsqlDbType.Varchar, _Parametros.NombrePlantilla);
                            Cmd2.Parameters.AddWithValue("@TituloPlantilla", NpgsqlDbType.Varchar, _Parametros.TituloPlantilla);
                            Cmd2.Parameters.AddWithValue("@MensajePlantilla", NpgsqlDbType.Varchar, _Parametros.MensajePlantilla);
                            IdResultado = int.Parse(Cmd2.ExecuteScalar().ToString());
                            if (IdResultado > 0)
                            {
                                if (!string.IsNullOrEmpty(_Parametros.Nombre))
                                {
                                    int ResultadoInteraccion = ExisteInteraccionCatalogo(_Parametros.Nombre, _ClaveServicio);

                                    if (ResultadoInteraccion == 0)
                                    {
                                        Query = "INSERT INTO catalogos.tbl_as_interaccion(" +
                                            "var_nombre, int_idas_plantilla, bol_esinmediato, bol_estiempo, bol_esvelocidad,bol_escomando, bol_esdefault, var_comandos_activan, var_comandos_terminan," +
                                            "int_dependiente, int_maxima_flotilla, int_categoria, int_modulo, int_idas_plantilla_dependiente," +
                                            "int_idas_interaccion_dependiente, bol_enuso, dt_modificacion) " +
                                            $"VALUES ";

                                        Respuesta.Resultado = IdResultado;

                                        Query += string.Format("('{0}',{1},'{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9},{10},{11},{12},{13},{14},TRUE,",

                                          _Parametros.Nombre,
                                          IdResultado,
                                          _Parametros.EsInmediato,
                                          _Parametros.EsTiempo,
                                          _Parametros.EsVelocidad,
                                          _Parametros.EsComando,
                                          _Parametros.EsDefault,
                                          _Parametros.ComandosActivan,
                                          _Parametros.ComandosTerminan,
                                          _Parametros.Dependiente,
                                          _Parametros.MaximaFlotilla,
                                          _Parametros.Categoria,
                                          _Parametros.Modulo,
                                          _Parametros.IdAsPlatillaDependiente,
                                          _Parametros.IdAsInteraccionDependiente
                                          ) + " CURRENT_TIMESTAMP(3) AT TIME ZONE 'UTC');";

                                        NpgsqlCommand Cmd4 = new NpgsqlCommand(Query, Conexion, Transaccion);

                                        Cmd4.ExecuteNonQuery();
                                        Transaccion.Commit();
                                        Respuesta.Resultado = IdResultado;
                                        Respuesta.Mensaje = "Ok";
                                    }
                                    else
                                    {
                                        Respuesta.Resultado = -5;
                                        Respuesta.Mensaje = "Error: Nombre de la interaccion existente";
                                    }

                                }
                                else
                                {
                                    Respuesta.Resultado = -6;
                                    Respuesta.Mensaje = "Error: Nombre de la interaccion no valido";
                                }
                                Conexion.Close();
                            }
                        }
                        catch (Exception Ex)
                        {
                            Transaccion.Rollback();
                            Respuesta.Resultado = -2;
                            Respuesta.Mensaje = "Error: No se pudo completar el registro";
                            Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
                        }
                        ConexionNombre.Close();
                    }
                    else
                    {
                        Respuesta.Resultado = -3;
                        Respuesta.Mensaje = "Error: Nombre de plantilla existente";
                    }
                }
                else
                {
                    Respuesta.Resultado = -4;
                    Respuesta.Mensaje = "Error: Nombre de plantilla no valido";
                }
            }
            catch (Exception Ex)
            {
                Respuesta.Resultado = -1;
                Respuesta.Mensaje = "Error general en el control";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), _ClaveServicio);
            }
            return Respuesta;
        }

        #endregion

        #region "-----------+FuncionesPrivadas+-----------"

        #region "-----------+ExistePlantilla+-----------"
        private int ExistePlantilla(string _Nombre, int _IdPlantilla,string _ClaveServicio)
        {

            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Interaccion, 5);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            int Resultado = 0;

            try
            {
                Conexion.Open();
                Query = string.Format("SELECT COUNT(1) FROM bigdata.as_plantilla " +
                                "WHERE TRIM(LOWER(var_nombre)) = '{0}'", _Nombre.Trim().ToLower());
                if (_IdPlantilla > 0)
                {
                    Query += " AND int_id != " + _IdPlantilla;
                }

                NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                int CantidadPlantilla = Convert.ToInt32(Cmd1.ExecuteScalar());

                if (CantidadPlantilla > 0)
                {
                    Resultado = CantidadPlantilla;
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

        #region "-----------+ExisteInteraccion+-----------"
        private int ExisteInteraccion(string _Nombre, int _IdInteraccion, string _ClaveServicio)
        {

            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Interaccion, 6);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            int Resultado = 0;

            try 
            {
                Conexion.Open();
                Query = string.Format("SELECT COUNT(1) FROM bigdata.as_interaccion " +
                                        "WHERE TRIM(LOWER(var_nombre)) = '{0}'", _Nombre.Trim().ToLower());

                if (_IdInteraccion > 0)
                {
                    Query += " AND int_id != " + _IdInteraccion;
                }

                NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                int CantidadInteraccion = Convert.ToInt32(Cmd1.ExecuteScalar());

                if (CantidadInteraccion > 0)
                {
                    Resultado = CantidadInteraccion;
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

        #region "-----------+ExistePlantillaCatalogo+-----------"
        private int ExistePlantillaCatalogo(string _Nombre, string _ClaveServicio)
        {

            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Interaccion, 7);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            int Resultado = 0;
            try
            {

                Conexion.Open();
                Query = string.Format("SELECT COUNT(1) FROM bigdata.as_plantilla " +
                                "WHERE TRIM(LOWER(var_nombre)) = '{0}'", _Nombre.Trim().ToLower());


                NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                int CantidadPlantilla = Convert.ToInt32(Cmd1.ExecuteScalar());

                if (CantidadPlantilla > 0)
                {
                    Resultado = CantidadPlantilla;
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

        #region "-----------+ExisteInteraccionCatalogo+-----------"
        private int ExisteInteraccionCatalogo(string _Nombre,string _ClaveServicio)
        {

            string Query = string.Empty;
            NpgsqlConnection Conexion = Utilidades.ObtenerConexion(ServiciosMC.mc_Interaccion, 8);
            RespuestaInteraccion Respuesta = new RespuestaInteraccion() { Resultado = 0, Mensaje = "Error de datos" };
            int Resultado = 0;

            try 
            {
                Conexion.Open();
                Query = string.Format("SELECT COUNT(1) FROM catalogos.tbl_as_interaccion " +
                                       "WHERE TRIM(LOWER(var_nombre)) = '{0}'", _Nombre.Trim().ToLower());

                NpgsqlCommand Cmd1 = new NpgsqlCommand(Query, Conexion);
                int CantidadInteraccion = Convert.ToInt32(Cmd1.ExecuteScalar());

                if (CantidadInteraccion > 0)
                {
                    Resultado = CantidadInteraccion;
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
        public class ParametrosInteraccion : ParametroGeneral
        {
            public string NombrePlantilla { get; set; }
            public string TituloPlantilla { get; set; }
            public string MensajePlantilla { get; set; }
            public string Nombre { get; set; }
            public bool EsInmediato { get; set; }
            public bool EsTiempo { get; set; }
            public bool EsVelocidad { get; set; }
            public bool EsComando { get; set; }
            public bool EsDefault { get; set; }
            public string ComandosActivan { get; set; }
            public string ComandosTerminan { get; set; }
            public int Dependiente { get; set; }
            public int MaximaFlotilla { get; set; }
            public int Categoria { get; set; }
            public int Modulo { get; set; }
            public int IdAsPlatillaDependiente { get; set; }
            public int IdAsInteraccionDependiente { get; set; }
        }
        public class ParametrosEditarInteraccion : ParametroGeneral
        {
            public int IdInteraccion { get; set; }
            public string Nombre { get; set; }
            public bool EsInmediato { get; set; }
            public bool EsTiempo { get; set; }
            public bool EsVelocidad { get; set; }
            public bool EsComando { get; set; }
            public bool EsDefault { get; set; }
            public string ComandosActivan { get; set; }
            public string ComandosTerminan { get; set; }
            public int Dependiente { get; set; }
            public int MaximaFlotilla { get; set; }
            public int Categoria { get; set; }
            public int Modulo { get; set; }
            public int IdAsPlatillaDependiente { get; set; }
            public int IdAsInteraccionDependiente { get; set; }
        }
        public class ParametrosEditarPlantilla : ParametroGeneral
        {
            public int IdPlantilla { get; set; }
            public string NombrePlantilla { get; set; }
            public string TituloPlantilla { get; set; }
            public string MensajePlantilla { get; set; }
        }
        public class ParametrosMigrarInteraccion : ParametroGeneral
        {

            public string NombrePlantilla { get; set; }
            public string TituloPlantilla { get; set; }
            public string MensajePlantilla { get; set; }
            public string Nombre { get; set; }
            public bool EsInmediato { get; set; }
            public bool EsTiempo { get; set; }
            public bool EsVelocidad { get; set; }
            public bool EsComando { get; set; }
            public bool EsDefault { get; set; }
            public string ComandosActivan { get; set; }
            public string ComandosTerminan { get; set; }
            public int Dependiente { get; set; }
            public int MaximaFlotilla { get; set; }
            public int Categoria { get; set; }
            public int Modulo { get; set; }
            public int IdAsPlatillaDependiente { get; set; }
            public int IdAsInteraccionDependiente { get; set; }

        }
        public class ParametroGeneral
        {
            public int IdUsuario { get; set; }
            public string Token { get; set; }
        }
    }
}
