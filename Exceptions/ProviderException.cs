using System;

namespace GetCNPJ.Exceptions
{
    /// <summary>
    /// Exceção lançada quando há erro em um provedor específico
    /// </summary>
    public class ProviderException : CnpjException
    {
        public string ProviderName { get; }

        public ProviderException(string providerName, string message)
            : base($"[{providerName}] {message}")
        {
            ProviderName = providerName;
        }

        public ProviderException(string providerName, string message, Exception innerException)
            : base($"[{providerName}] {message}", innerException)
        {
            ProviderName = providerName;
        }
    }
}
