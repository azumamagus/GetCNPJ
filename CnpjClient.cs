using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GetCNPJ.Interfaces;
using GetCNPJ.Models;
using GetCNPJ.Providers.BrasilAPI;
using GetCNPJ.Providers.CNPJA;
using GetCNPJ.Providers.CNPJWS;
using GetCNPJ.Providers.ReceitaWS;
using GetCNPJ.RateLimiter;
using GetCNPJ.Services;

namespace GetCNPJ
{
    /// <summary>
    /// Cliente principal para consulta de CNPJ
    /// </summary>
    public class CnpjClient : IDisposable
    {
        private readonly ICnpjService _cnpjService;
        private readonly HttpClient _httpClient;
        private readonly bool _disposeHttpClient;

        /// <summary>
        /// Construtor com configuração padrão
        /// </summary>
        public CnpjClient() : this(null, null)
        {
        }

        /// <summary>
        /// Construtor com HttpClient customizado
        /// </summary>
        /// <param name="httpClient">HttpClient customizado (opcional)</param>
        /// <param name="options">Opções de configuração (opcional)</param>
        public CnpjClient(HttpClient httpClient = null, CnpjClientOptions options = null)
        {
            options = options ?? new CnpjClientOptions();

            // Configura HttpClient
            if (httpClient != null)
            {
                _httpClient = httpClient;
                _disposeHttpClient = false;
            }
            else
            {
                _httpClient = new HttpClient
                {
                    Timeout = options.Timeout
                };
                _disposeHttpClient = true;
            }

            // Configura Rate Limiter
            var rateLimiter = new SlidingWindowRateLimiter(
                options.MaxRequestsPerMinute,
                TimeSpan.FromMinutes(1)
            );

            // Configura provedores
            var providers = new List<ICnpjProvider>();

            if (options.EnableCNPJWS)
            {
                providers.Add(new CNPJWSProvider(_httpClient, rateLimiter));
            }

            if (options.EnableReceitaWS)
            {
                providers.Add(new ReceitaWSProvider(_httpClient, rateLimiter));
            }

            if (options.EnableBrasilAPI)
            {
                providers.Add(new BrasilAPIProvider(_httpClient, rateLimiter));
            }

            if (options.EnableCNPJA)
            {
                providers.Add(new CNPJAProvider(_httpClient, rateLimiter));
            }

            if (providers.Count == 0)
            {
                throw new InvalidOperationException("Pelo menos um provedor deve estar habilitado");
            }

            _cnpjService = new CnpjService(providers);
        }

        /// <summary>
        /// Consulta dados de um CNPJ
        /// </summary>
        /// <param name="cnpj">CNPJ a ser consultado (pode conter formatação)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Resultado da consulta</returns>
        public Task<CnpjResult> GetAsync(string cnpj, CancellationToken cancellationToken = default)
        {
            return _cnpjService.GetCnpjAsync(cnpj, cancellationToken);
        }

        /// <summary>
        /// Consulta dados de um CNPJ usando um provedor específico
        /// </summary>
        /// <param name="cnpj">CNPJ a ser consultado</param>
        /// <param name="providerName">Nome do provedor (ReceitaWS, BrasilAPI, CNPJA)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Resultado da consulta</returns>
        public Task<CnpjResult> GetFromProviderAsync(
            string cnpj,
            string providerName,
            CancellationToken cancellationToken = default)
        {
            return _cnpjService.GetCnpjFromProviderAsync(cnpj, providerName, cancellationToken);
        }

        /// <summary>
        /// Consulta dados de um CNPJ de forma síncrona
        /// </summary>
        /// <param name="cnpj">CNPJ a ser consultado (pode conter formatação)</param>
        /// <returns>Resultado da consulta</returns>
        public CnpjResult Get(string cnpj)
        {
            return GetAsync(cnpj).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Consulta dados de um CNPJ usando um provedor específico de forma síncrona
        /// </summary>
        /// <param name="cnpj">CNPJ a ser consultado</param>
        /// <param name="providerName">Nome do provedor (ReceitaWS, BrasilAPI, CNPJA)</param>
        /// <returns>Resultado da consulta</returns>
        public CnpjResult GetFromProvider(string cnpj, string providerName)
        {
            return GetFromProviderAsync(cnpj, providerName).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Libera os recursos utilizados
        /// </summary>
        public void Dispose()
        {
            if (_disposeHttpClient)
            {
                _httpClient?.Dispose();
            }
        }
    }

    /// <summary>
    /// Opções de configuração do CnpjClient
    /// </summary>
    public class CnpjClientOptions
    {
        /// <summary>
        /// Timeout para requisições HTTP (padrão: 30 segundos)
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Número máximo de requisições por minuto (padrão: 3)
        /// </summary>
        public int MaxRequestsPerMinute { get; set; } = 3;

        /// <summary>
        /// Habilita o provedor CNPJ.WS (padrão: true)
        /// Recomendado manter habilitado pois é o único que retorna inscrição estadual
        /// </summary>
        public bool EnableCNPJWS { get; set; } = true;

        /// <summary>
        /// Habilita o provedor ReceitaWS (padrão: true)
        /// </summary>
        public bool EnableReceitaWS { get; set; } = true;

        /// <summary>
        /// Habilita o provedor BrasilAPI (padrão: true)
        /// </summary>
        public bool EnableBrasilAPI { get; set; } = true;

        /// <summary>
        /// Habilita o provedor CNPJA (padrão: true)
        /// </summary>
        public bool EnableCNPJA { get; set; } = true;
    }
}
