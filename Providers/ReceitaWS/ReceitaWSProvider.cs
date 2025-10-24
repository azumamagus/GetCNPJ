using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GetCNPJ.Interfaces;
using GetCNPJ.Models;
using GetCNPJ.Providers.Base;

namespace GetCNPJ.Providers.ReceitaWS
{
    /// <summary>
    /// Provider para ReceitaWS - https://receitaws.com.br
    /// </summary>
    public class ReceitaWSProvider : CnpjProviderBase
    {
        public override string ProviderName => "ReceitaWS";
        public override int Priority => 2;
        protected override string BaseUrl => "https://receitaws.com.br/v1/cnpj/";

        public ReceitaWSProvider(HttpClient httpClient, IRateLimiter rateLimiter)
            : base(httpClient, rateLimiter)
        {
        }

        protected override async Task<CnpjData> FetchDataAsync(string cnpj, CancellationToken cancellationToken)
        {
            var url = $"{BaseUrl}{cnpj}";
            var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new HttpRequestException($"Erro na requisição: {response.StatusCode} - {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var data = JsonSerializer.Deserialize<ReceitaWSResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null || data.status == "ERROR")
            {
                return null;
            }

            return MapToCnpjData(data);
        }

        private CnpjData MapToCnpjData(ReceitaWSResponse response)
        {
            var cnpjData = new CnpjData
            {
                Cnpj = response.cnpj,
                RazaoSocial = response.nome,
                NomeFantasia = response.fantasia,
                DataAbertura = ParseDate(response.abertura),
                Situacao = response.situacao,
                DataSituacao = ParseDate(response.data_situacao),
                Tipo = response.tipo,
                Porte = response.porte,
                NaturezaJuridica = response.natureza_juridica,
                CapitalSocial = ParseDecimal(response.capital_social),
                Email = response.email,
                UltimaAtualizacao = response.ultima_atualizacao,
                Endereco = new Endereco
                {
                    Cep = response.cep,
                    Logradouro = response.logradouro,
                    Numero = response.numero,
                    Complemento = response.complemento,
                    Bairro = response.bairro,
                    Municipio = response.municipio,
                    Uf = response.uf
                }
            };

            // Telefones
            if (!string.IsNullOrWhiteSpace(response.telefone))
            {
                cnpjData.Telefones = response.telefone
                    .Split(new[] { '/', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();
            }

            // Atividade principal
            if (response.atividade_principal != null && response.atividade_principal.Any())
            {
                var atividade = response.atividade_principal.First();
                cnpjData.AtividadePrincipal = new AtividadeEconomica
                {
                    Codigo = atividade.code,
                    Descricao = atividade.text
                };
            }

            // Atividades secundárias
            if (response.atividades_secundarias != null)
            {
                cnpjData.AtividadesSecundarias = response.atividades_secundarias
                    .Select(a => new AtividadeEconomica
                    {
                        Codigo = a.code,
                        Descricao = a.text
                    })
                    .ToList();
            }

            // Quadro societário
            if (response.qsa != null)
            {
                cnpjData.QuadroSocietario = response.qsa
                    .Select(s => new Socio
                    {
                        Nome = s.nome,
                        Qualificacao = s.qual
                    })
                    .ToList();
            }

            // Simples Nacional
            if (response.simples != null)
            {
                cnpjData.Simples = new SimplesNacional
                {
                    Optante = response.simples.optante,
                    DataOpcao = ParseDate(response.simples.data_opcao),
                    DataExclusao = ParseDate(response.simples.data_exclusao),
                    OptanteSimei = response.simei?.optante ?? false
                };
            }

            return cnpjData;
        }

        private DateTime? ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            if (DateTime.TryParse(dateString, out date))
                return date;

            return null;
        }

        private decimal? ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;

            return null;
        }
    }
}
