using System;

namespace GetCNPJ.Exceptions
{
    /// <summary>
    /// Exceção lançada quando o rate limit é excedido
    /// </summary>
    public class RateLimitException : CnpjException
    {
        public string ProviderName { get; }
        public TimeSpan WaitTime { get; }

        public RateLimitException(string providerName, TimeSpan waitTime)
            : base($"Rate limit excedido para o provedor {providerName}. Aguarde {waitTime.TotalSeconds:F0} segundos.")
        {
            ProviderName = providerName;
            WaitTime = waitTime;
        }
    }
}
