﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace svc_EnviarCorreo
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="svc_EnviarCorreo.svc_ISendEmail")]
    public interface svc_ISendEmail
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/svc_ISendEmail/EnviarCorreo", ReplyAction="http://tempuri.org/svc_ISendEmail/EnviarCorreoResponse")]
        bool EnviarCorreo(string Asunto, string Mensaje, string Destinatarios, string ConCopia, string ConCopiaOculta, string ArchivoAdjunto, string MailFrom, string MailName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/svc_ISendEmail/EnviarCorreo", ReplyAction="http://tempuri.org/svc_ISendEmail/EnviarCorreoResponse")]
        System.Threading.Tasks.Task<bool> EnviarCorreoAsync(string Asunto, string Mensaje, string Destinatarios, string ConCopia, string ConCopiaOculta, string ArchivoAdjunto, string MailFrom, string MailName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/svc_ISendEmail/EnviarCorreoError", ReplyAction="http://tempuri.org/svc_ISendEmail/EnviarCorreoErrorResponse")]
        bool EnviarCorreoError(string Asunto, string Mensaje, string Destinatarios, string ConCopia, string MailFrom, string MailName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/svc_ISendEmail/EnviarCorreoError", ReplyAction="http://tempuri.org/svc_ISendEmail/EnviarCorreoErrorResponse")]
        System.Threading.Tasks.Task<bool> EnviarCorreoErrorAsync(string Asunto, string Mensaje, string Destinatarios, string ConCopia, string MailFrom, string MailName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    public interface svc_ISendEmailChannel : svc_EnviarCorreo.svc_ISendEmail, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    public partial class svc_ISendEmailClient : System.ServiceModel.ClientBase<svc_EnviarCorreo.svc_ISendEmail>, svc_EnviarCorreo.svc_ISendEmail
    {
        
        /// <summary>
        /// Implemente este método parcial para configurar el punto de conexión de servicio.
        /// </summary>
        /// <param name="serviceEndpoint">El punto de conexión para configurar</param>
        /// <param name="clientCredentials">Credenciales de cliente</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public svc_ISendEmailClient() : 
                base(svc_ISendEmailClient.GetDefaultBinding(), svc_ISendEmailClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_svc_ISendEmail.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public svc_ISendEmailClient(EndpointConfiguration endpointConfiguration) : 
                base(svc_ISendEmailClient.GetBindingForEndpoint(endpointConfiguration), svc_ISendEmailClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public svc_ISendEmailClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(svc_ISendEmailClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public svc_ISendEmailClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(svc_ISendEmailClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public svc_ISendEmailClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public bool EnviarCorreo(string Asunto, string Mensaje, string Destinatarios, string ConCopia, string ConCopiaOculta, string ArchivoAdjunto, string MailFrom, string MailName)
        {
            return base.Channel.EnviarCorreo(Asunto, Mensaje, Destinatarios, ConCopia, ConCopiaOculta, ArchivoAdjunto, MailFrom, MailName);
        }
        
        public System.Threading.Tasks.Task<bool> EnviarCorreoAsync(string Asunto, string Mensaje, string Destinatarios, string ConCopia, string ConCopiaOculta, string ArchivoAdjunto, string MailFrom, string MailName)
        {
            return base.Channel.EnviarCorreoAsync(Asunto, Mensaje, Destinatarios, ConCopia, ConCopiaOculta, ArchivoAdjunto, MailFrom, MailName);
        }
        
        public bool EnviarCorreoError(string Asunto, string Mensaje, string Destinatarios, string ConCopia, string MailFrom, string MailName)
        {
            return base.Channel.EnviarCorreoError(Asunto, Mensaje, Destinatarios, ConCopia, MailFrom, MailName);
        }
        
        public System.Threading.Tasks.Task<bool> EnviarCorreoErrorAsync(string Asunto, string Mensaje, string Destinatarios, string ConCopia, string MailFrom, string MailName)
        {
            return base.Channel.EnviarCorreoErrorAsync(Asunto, Mensaje, Destinatarios, ConCopia, MailFrom, MailName);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_svc_ISendEmail))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("No se pudo encontrar un punto de conexión con el nombre \"{0}\".", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_svc_ISendEmail))
            {
                return new System.ServiceModel.EndpointAddress("http://192.168.20.40:8085/ServiciosX/svc_SendEmail.svc");
            }
            throw new System.InvalidOperationException(string.Format("No se pudo encontrar un punto de conexión con el nombre \"{0}\".", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return svc_ISendEmailClient.GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_svc_ISendEmail);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return svc_ISendEmailClient.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_svc_ISendEmail);
        }
        
        public enum EndpointConfiguration
        {
            
            BasicHttpBinding_svc_ISendEmail,
        }
    }
}
