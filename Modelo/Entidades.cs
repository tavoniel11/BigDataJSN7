namespace BigDataJSN7.Modelo
{
    
    public class RespuestaSitios
    {
        public int Resultado { get; set; }
        public string Mensaje { get; set; }
        public string Archivo { get; set; }
    }
    public class RespuestaEmpresaServidor
    {
        public int Estado { get; set; }
        public object Datos { get; set; }
        public string Mensaje { get; set; }
    }
    public class RespuestaTablas
    {
        public int Estado { get; set; }
        public object Datos { get; set; }
        public string Mensaje { get; set; }
    }
    public class RespuestaInteraccion
    {
        public int Resultado { get; set; }
        public string Mensaje { get; set; }
    }
    public class RespuestaFuncion
    {
        public int Resultado { get; set; }
        public string Mensaje { get; set; }
    }

    public enum ServiciosMC
    {
        mc_Comandos = 1,
        mc_Sitios = 2,
        mc_Interaccion = 3,
        mc_Funcion= 4
    }

    public class RespuestaComandos
    {
        public int Estado { get; set; }
        public object Datos { get; set; }
        public string Mensaje { get; set; }

    }

    public class Log
    {
        public string Clase { get; set; }
        public string Funcion { get; set; }
        public string Error { get; set; }

    }
}