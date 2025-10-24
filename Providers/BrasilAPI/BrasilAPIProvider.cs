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

namespace GetCNPJ.Providers.BrasilAPI
{
    /// <summary>
    /// Provider para BrasilAPI - https://brasilapi.com.br
    /// </summary>
    public class BrasilAPIProvider : CnpjProviderBase
    {
        public override string ProviderName => "BrasilAPI";
        public override int Priority => 3;
        protected override string BaseUrl => "https://brasilapi.com.br/api/cnpj/v1/";

        public BrasilAPIProvider(HttpClient httpClient, IRateLimiter rateLimiter)
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
            var data = JsonSerializer.Deserialize<BrasilAPIResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return null;
            }

            return MapToCnpjData(data);
        }

        private CnpjData MapToCnpjData(BrasilAPIResponse response)
        {
            var cnpjData = new CnpjData
            {
                Cnpj = FormatCnpj(response.cnpj),
                RazaoSocial = response.razao_social,
                NomeFantasia = response.nome_fantasia,
                DataAbertura = ParseDate(response.data_inicio_atividade),
                Situacao = response.descricao_situacao_cadastral,
                DataSituacao = ParseDate(response.data_situacao_cadastral),
                Tipo = response.descricao_matriz_filial,
                Porte = response.descricao_porte,
                NaturezaJuridica = response.natureza_juridica,
                CapitalSocial = response.capital_social,
                Endereco = new Endereco
                {
                    Cep = FormatCep(response.cep),
                    Logradouro = $"{response.descricao_tipo_logradouro} {response.logradouro}".Trim(),
                    Numero = response.numero,
                    Complemento = response.complemento,
                    Bairro = response.bairro,
                    Municipio = response.municipio,
                    Uf = response.uf
                }
            };

            // Telefones
            if (!string.IsNullOrWhiteSpace(response.ddd_telefone_1))
                cnpjData.Telefones.Add(FormatPhone(response.ddd_telefone_1));

            if (!string.IsNullOrWhiteSpace(response.ddd_telefone_2))
                cnpjData.Telefones.Add(FormatPhone(response.ddd_telefone_2));

            // Atividade principal
            if (response.cnae_fiscal.HasValue)
            {
                cnpjData.AtividadePrincipal = new AtividadeEconomica
                {
                    Codigo = FormatCnae(response.cnae_fiscal.Value),
                    Descricao = response.cnae_fiscal_descricao
                };
            }

            // Atividades secundárias
            if (response.cnaes_secundarios != null)
            {
                cnpjData.AtividadesSecundarias = response.cnaes_secundarios
                    .Where(c => c.codigo.HasValue)
                    .Select(c => new AtividadeEconomica
                    {
                        Codigo = FormatCnae(c.codigo.Value),
                        Descricao = c.descricao
                    })
                    .ToList();
            }

            // Quadro societário
            if (response.qsa != null)
            {
                cnpjData.QuadroSocietario = response.qsa
                    .Select(s => new Socio
                    {
                        Nome = s.nome_socio,
                        Qualificacao = s.qualificacao_socio
                    })
                    .ToList();
            }

            // Simples Nacional
            cnpjData.Simples = new SimplesNacional
            {
                Optante = response.opcao_pelo_simples ?? false,
                DataOpcao = ParseDate(response.data_opcao_pelo_simples),
                DataExclusao = ParseDate(response.data_exclusao_do_simples),
                OptanteSimei = response.opcao_pelo_mei ?? false
            };

            return cnpjData;
        }

        private DateTime? ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            if (DateTime.TryParse(dateString, out date))
                return date;

            return null;
        }

        private string FormatCep(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8)
                return cep;

            return $"{cep.Substring(0, 5)}-{cep.Substring(5, 3)}";
        }

        private string FormatCnae(int cnae)
        {
            var cnaeStr = cnae.ToString("D7");
            return $"{cnaeStr.Substring(0, 2)}.{cnaeStr.Substring(2, 2)}-{cnaeStr.Substring(4, 1)}-{cnaeStr.Substring(5, 2)}";
        }

        private string FormatPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            // Remove caracteres não numéricos
            phone = new string(phone.Where(char.IsDigit).ToArray());

            if (phone.Length == 10)
                return $"({phone.Substring(0, 2)}) {phone.Substring(2, 4)}-{phone.Substring(6, 4)}";

            if (phone.Length == 11)
                return $"({phone.Substring(0, 2)}) {phone.Substring(2, 5)}-{phone.Substring(7, 4)}";

            return phone;
        }
    }
}
