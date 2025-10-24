using System.Threading;
using System.Threading.Tasks;

namespace GetCNPJ.Interfaces
{
    /// <summary>
    /// Interface para controle de rate limiting
    /// </summary>
    public interface IRateLimiter
    {
        /// <summary>
        /// Aguarda até que uma requisição possa ser feita
        /// </summary>
        /// <param name="providerName">Nome do provedor</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        Task WaitIfNeededAsync(string providerName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registra que uma requisição foi feita
        /// </summary>
        /// <param name="providerName">Nome do provedor</param>
        void RecordRequest(string providerName);

        /// <summary>
        /// Reseta o contador de requisições para um provedor
        /// </summary>
        /// <param name="providerName">Nome do provedor</param>
        void Reset(string providerName);

        /// <summary>
        /// Obtém o número de requisições disponíveis
        /// </summary>
        /// <param name="providerName">Nome do provedor</param>
        /// <returns>Número de requisições disponíveis</returns>
        int GetAvailableRequests(string providerName);
    }
}
