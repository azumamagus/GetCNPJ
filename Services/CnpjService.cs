using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GetCNPJ.Interfaces;
using GetCNPJ.Models;

namespace GetCNPJ.Services
{
    /// <summary>
    /// Serviço principal de consulta de CNPJ
    /// Implementa Chain of Responsibility para tentar múltiplos provedores
    /// </summary>
    public class CnpjService : ICnpjService
    {
        private readonly IEnumerable<ICnpjProvider> _providers;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="providers">Lista de provedores disponíveis</param>
        public CnpjService(IEnumerable<ICnpjProvider> providers)
        {
            _providers = providers?.OrderBy(p => p.Priority)
                ?? throw new ArgumentNullException(nameof(providers));

            if (!_providers.Any())
            {
                throw new ArgumentException("Pelo menos um provedor deve ser configurado", nameof(providers));
            }
        }

        /// <inheritdoc/>
        public async Task<CnpjResult> GetCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
        {
            var errors = new List<ProviderError>();

            // Tenta cada provedor em ordem de prioridade
            foreach (var provider in _providers)
            {
                try
                {
                    var data = await provider.GetCnpjDataAsync(cnpj, cancellationToken).ConfigureAwait(false);

                    if (data != null)
                    {
                        return CnpjResult.CreateSuccess(data);
                    }

                    // Provedor retornou null (CNPJ não encontrado)
                    errors.Add(new ProviderError
                    {
                        ProviderName = provider.ProviderName,
                        ErrorMessage = "CNPJ não encontrado"
                    });
                }
                catch (OperationCanceledException)
                {
                    // Operação cancelada pelo usuário, propaga a exceção
                    throw;
                }
                catch (Exception ex)
                {
                    // Registra o erro e tenta o próximo provedor
                    errors.Add(new ProviderError
                    {
                        ProviderName = provider.ProviderName,
                        ErrorMessage = ex.Message,
                        Exception = ex
                    });
                }
            }

            // Todos os provedores falharam
            var errorMessage = $"Não foi possível consultar o CNPJ. Tentativas: {errors.Count}";
            return CnpjResult.CreateError(errorMessage, errors);
        }

        /// <inheritdoc/>
        public async Task<CnpjResult> GetCnpjFromProviderAsync(
            string cnpj,
            string providerName,
            CancellationToken cancellationToken = default)
        {
            var provider = _providers.FirstOrDefault(p =>
                p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
            {
                return CnpjResult.CreateError(
                    $"Provedor '{providerName}' não encontrado. Provedores disponíveis: {string.Join(", ", _providers.Select(p => p.ProviderName))}"
                );
            }

            try
            {
                var data = await provider.GetCnpjDataAsync(cnpj, cancellationToken).ConfigureAwait(false);

                if (data != null)
                {
                    return CnpjResult.CreateSuccess(data);
                }

                return CnpjResult.CreateError("CNPJ não encontrado");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var error = new ProviderError
                {
                    ProviderName = provider.ProviderName,
                    ErrorMessage = ex.Message,
                    Exception = ex
                };

                return CnpjResult.CreateError($"Erro ao consultar CNPJ: {ex.Message}", new List<ProviderError> { error });
            }
        }
    }
}
