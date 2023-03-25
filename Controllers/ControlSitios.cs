using Microsoft.AspNetCore.Mvc;
using BigDataJSN7.Modelo;
using System.Reflection;
using static BigDataJSN7.Modelo.DatosSitios;
using System.ComponentModel;
using OfficeOpenXml;

namespace BigDataJSN7.Controllers
{
    [Route("mc_Sitios")]
    [ApiController]
    public class ControlSitios : ControllerBase
    {

        static string NombreServicio = ServiciosMC.mc_Sitios.ToString();
        static string[] Directorio = new string[] { NombreServicio };


        public DatosSitios DatosSitios;
        public Utilidades Utilidad;
        public ControlSitios()
        {
            DatosSitios = new DatosSitios();
            Utilidad = new Utilidades();
        }

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
        #endregion+

        #region "-----------+SubirArchivo+-----------"
        [HttpPost("ImportarArchivo")]
        public RespuestaSitios ImportarArchivo([FromForm] ParametrosSitios Parametros)
        {

            RespuestaSitios Respuesta = new RespuestaSitios { Resultado = 0, Mensaje = string.Empty, Archivo = null };
            DatosSitios Datos = new DatosSitios();
            string ClaveServicio = DateTime.UtcNow.Ticks.ToString();
            var DirectorioExcel = Utilidades.ObjBackend.ObjAplicacion.DirectorioReporteArchivos;
            try
            {
                if (Parametros.Archivo == null)
                {
                    Respuesta.Resultado = -3;
                    Respuesta.Mensaje = "No existe un archivo";
                    Respuesta.Archivo = "";
                    return Respuesta;
                }

                var NombreArchivo = Parametros.Archivo.FileName;

                if (!Path.GetExtension(Parametros.Archivo.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    Respuesta.Resultado = -4;
                    Respuesta.Mensaje = "Tipo de archivo no valido";
                    Respuesta.Archivo = "";
                    return Respuesta;
                }

                var ListaDetalle = new List<DatosSitios.MigracionDetalle>();


                string NombreExcel = $"embe_{DateTime.Now.ToString("yyyyMMddHHmmssf") + "_" + Parametros.Archivo.FileName}";

                if (!Directory.Exists(DirectorioExcel))
                {
                    Directory.CreateDirectory(DirectorioExcel); 
                }
                var ArchivoString = new FileStream(Path.Combine(DirectorioExcel, NombreExcel), FileMode.Create);

                Parametros.Archivo.CopyToAsync(ArchivoString);


                ExcelPackage Paquete = new ExcelPackage(ArchivoString);

                ExcelWorksheet HojaExcel = Paquete.Workbook.Worksheets[0];
                
                if (HojaExcel.Dimension == null)
                {
                    Respuesta.Resultado = -5;
                    Respuesta.Mensaje = "El archivo esta vacio";
                    Respuesta.Archivo = NombreArchivo;
                    return Respuesta;
                }
                for (int row = 2; row <= HojaExcel.Dimension.Rows; row++)
                {
                    DatosSitios.MigracionDetalle Valor = new DatosSitios.MigracionDetalle();

                    Valor.Estado = 0;

                    if (HojaExcel.Cells[row, 1].Value == null)
                    {
                        Valor.Estado = -1;
                    }
                    if (Valor.Estado == 0)
                    {
                        if (HojaExcel.Cells[row, 3].Value == null && HojaExcel.Cells[row, 4].Value == null)
                        {
                            Valor.Estado = -2;
                        }
                    }
                    if (Valor.Estado == 0)
                    {
                        if (HojaExcel.Cells[row, 3].Value == null)
                        {
                            Valor.Estado = -3;
                        }
                    }
                    if (Valor.Estado == 0)
                    {
                        if (HojaExcel.Cells[row, 4].Value == null)
                        {
                            Valor.Estado = -4;
                        }
                    }
                    if (Valor.Estado == 0)
                    {
                        if (HojaExcel.Cells[row, 5].Value == null)
                        {
                            Valor.Estado = -5;
                        }
                    }

                    
                    Valor.Nombre = HojaExcel.Cells[row, 1].Value == null ? string.Empty : HojaExcel.Cells[row, 1].Value.ToString();
                    Valor.Descripcion = HojaExcel.Cells[row, 2].Value == null ? string.Empty : HojaExcel.Cells[row, 2].Value.ToString();
                    Valor.Latitud = Convert.ToDouble(HojaExcel.Cells[row, 3].Value == null ? "0" : HojaExcel.Cells[row, 3].Value.ToString());
                    Valor.Longitud = Convert.ToDouble(HojaExcel.Cells[row, 4].Value == null ? "0" : HojaExcel.Cells[row, 4].Value.ToString());
                    Valor.Radio = Convert.ToDouble(HojaExcel.Cells[row, 5].Value == null ? "0" : HojaExcel.Cells[row, 5].Value.ToString());

                    ListaDetalle.Add(Valor);
                }
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
                                    Respuesta = DatosSitios.SubirArchivo(Parametros, NombreArchivo, ListaDetalle, ClaveServicio);
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
                Respuesta.Archivo = "";
                Utilidades.RegistrarError(Directorio, NombreServicio, Ex, MethodBase.GetCurrentMethod(), ClaveServicio);

            }
            #region "----+Logs+----"
            try
            {
                if (Datos.ObjServicio.HabilitarLogServicio)
                {
                    object ObjParametros = new
                    {
                        Descripcion = Parametros.DescripcionFolio,
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
    }
    #endregion


}

