using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GetCNPJ.Exceptions;
using GetCNPJ.Interfaces;
using GetCNPJ.Models;

namespace GetCNPJ.Providers.Base
{
    /// <summary>
    /// Classe base para provedores de dados de CNPJ
    /// </summary>
    public abstract class CnpjProviderBase : ICnpjProvider
    {
        protected readonly HttpClient _httpClient;
        protected readonly IRateLimiter _rateLimiter;

        /// <inheritdoc/>
        public abstract string ProviderName { get; }

        /// <inheritdoc/>
        public abstract int Priority { get; }

        /// <summary>
        /// URL base da API
        /// </summary>
        protected abstract string BaseUrl { get; }

        protected CnpjProviderBase(HttpClient httpClient, IRateLimiter rateLimiter)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
        }

        /// <inheritdoc/>
        public async Task<CnpjData> GetCnpjDataAsync(string cnpj, CancellationToken cancellationToken = default)
        {
            try
            {
                // Valida e normaliza o CNPJ
                cnpj = ValidateAndNormalizeCnpj(cnpj);

                // Aguarda rate limiter
                await _rateLimiter.WaitIfNeededAsync(ProviderName, cancellationToken).ConfigureAwait(false);

                // Registra a requisição
                _rateLimiter.RecordRequest(ProviderName);

                // Faz a requisição
                var data = await FetchDataAsync(cnpj, cancellationToken).ConfigureAwait(false);

                if (data != null)
                {
                    data.Provedor = ProviderName;
                }

                return data;
            }
            catch (InvalidCnpjException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ProviderException(ProviderName, $"Erro ao consultar CNPJ: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<bool> IsAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Método abstrato para buscar dados do provedor específico
        /// </summary>
        protected abstract Task<CnpjData> FetchDataAsync(string cnpj, CancellationToken cancellationToken);

        /// <summary>
        /// Valida e normaliza um CNPJ (remove formatação)
        /// </summary>
        protected string ValidateAndNormalizeCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                throw new InvalidCnpjException("CNPJ não pode ser nulo ou vazio");

            // Remove caracteres não numéricos
            cnpj = Regex.Replace(cnpj, @"\D", "");

            // Valida tamanho
            if (cnpj.Length != 14)
                throw new InvalidCnpjException($"CNPJ deve ter 14 dígitos. Recebido: {cnpj}");

            // Valida dígitos verificadores
            if (!IsValidCnpj(cnpj))
                throw new InvalidCnpjException($"CNPJ inválido: {cnpj}");

            return cnpj;
        }

        /// <summary>
        /// Valida os dígitos verificadores do CNPJ
        /// </summary>
        protected bool IsValidCnpj(string cnpj)
        {
            if (cnpj.Length != 14)
                return false;

            // Verifica se todos os dígitos são iguais
            var allSame = true;
            for (int i = 1; i < 14 && allSame; i++)
            {
                if (cnpj[i] != cnpj[0])
                    allSame = false;
            }

            if (allSame)
                return false;

            // Valida primeiro dígito verificador
            int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int sum = 0;

            for (int i = 0; i < 12; i++)
                sum += int.Parse(cnpj[i].ToString()) * multiplier1[i];

            int remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cnpj[12].ToString()) != remainder)
                return false;

            // Valida segundo dígito verificador
            int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            sum = 0;

            for (int i = 0; i < 13; i++)
                sum += int.Parse(cnpj[i].ToString()) * multiplier2[i];

            remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;

            return int.Parse(cnpj[13].ToString()) == remainder;
        }

        /// <summary>
        /// Formata um CNPJ com pontuação
        /// </summary>
        protected string FormatCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14)
                return cnpj;

            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }
    }
}
