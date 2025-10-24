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

namespace GetCNPJ.Providers.CNPJWS
{
    /// <summary>
    /// Provider para CNPJ.WS - https://publica.cnpj.ws
    /// Este é o provedor padrão pois retorna inscrição estadual
    /// </summary>
    public class CNPJWSProvider : CnpjProviderBase
    {
        public override string ProviderName => "CNPJWS";
        public override int Priority => 1; // Prioridade mais alta (padrão)
        protected override string BaseUrl => "https://publica.cnpj.ws/cnpj/";

        public CNPJWSProvider(HttpClient httpClient, IRateLimiter rateLimiter)
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
            var data = JsonSerializer.Deserialize<CNPJWSResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null || data.estabelecimento == null)
            {
                return null;
            }

            return MapToCnpjData(data);
        }

        private CnpjData MapToCnpjData(CNPJWSResponse response)
        {
            var estab = response.estabelecimento;

            var cnpjData = new CnpjData
            {
                Cnpj = FormatCnpj(estab.cnpj),
                RazaoSocial = response.razao_social,
                NomeFantasia = estab.nome_fantasia,
                DataAbertura = ParseDate(estab.data_inicio_atividade),
                Situacao = estab.situacao_cadastral,
                DataSituacao = ParseDate(estab.data_situacao_cadastral),
                Tipo = estab.tipo,
                Porte = response.porte?.descricao,
                NaturezaJuridica = response.natureza_juridica != null
                    ? $"{response.natureza_juridica.id} - {response.natureza_juridica.descricao}"
                    : null,
                CapitalSocial = ParseDecimal(response.capital_social),
                Email = estab.email,
                UltimaAtualizacao = response.atualizado_em,
                Endereco = new Endereco
                {
                    Cep = FormatCep(estab.cep),
                    Logradouro = $"{estab.tipo_logradouro} {estab.logradouro}".Trim(),
                    Numero = estab.numero,
                    Complemento = estab.complemento,
                    Bairro = estab.bairro,
                    Municipio = estab.cidade?.nome,
                    Uf = estab.estado?.sigla
                }
            };

            // Telefones
            if (!string.IsNullOrWhiteSpace(estab.ddd1) && !string.IsNullOrWhiteSpace(estab.telefone1))
            {
                cnpjData.Telefones.Add(FormatPhone(estab.ddd1, estab.telefone1));
            }

            if (!string.IsNullOrWhiteSpace(estab.ddd2) && !string.IsNullOrWhiteSpace(estab.telefone2))
            {
                cnpjData.Telefones.Add(FormatPhone(estab.ddd2, estab.telefone2));
            }

            // Atividade principal
            if (estab.atividade_principal != null)
            {
                cnpjData.AtividadePrincipal = new AtividadeEconomica
                {
                    Codigo = FormatCnae(estab.atividade_principal.subclasse),
                    Descricao = estab.atividade_principal.descricao
                };
            }

            // Atividades secundárias
            if (estab.atividades_secundarias != null)
            {
                cnpjData.AtividadesSecundarias = estab.atividades_secundarias
                    .Select(a => new AtividadeEconomica
                    {
                        Codigo = FormatCnae(a.subclasse),
                        Descricao = a.descricao
                    })
                    .ToList();
            }

            // Quadro societário
            if (response.socios != null)
            {
                cnpjData.QuadroSocietario = response.socios
                    .Select(s => new Socio
                    {
                        Nome = s.nome,
                        Qualificacao = s.qualificacao_socio?.descricao
                    })
                    .ToList();
            }

            // Inscrições Estaduais
            if (estab.inscricoes_estaduais != null)
            {
                cnpjData.InscricoesEstaduais = estab.inscricoes_estaduais
                    .Select(ie => new InscricaoEstadual
                    {
                        Inscricao = ie.inscricao_estadual,
                        Estado = ie.estado?.sigla,
                        Ativo = ie.ativo,
                        DataAtualizacao = ie.atualizado_em
                    })
                    .ToList();
            }

            // Simples Nacional
            if (response.simples != null)
            {
                cnpjData.Simples = new SimplesNacional
                {
                    Optante = response.simples.simples?.Equals("Sim", StringComparison.OrdinalIgnoreCase) ?? false,
                    DataOpcao = ParseDate(response.simples.data_opcao_simples),
                    DataExclusao = ParseDate(response.simples.data_exclusao_simples),
                    OptanteSimei = response.simples.mei?.Equals("Sim", StringComparison.OrdinalIgnoreCase) ?? false
                };
            }

            return cnpjData;
        }

        private DateTime? ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            // Tenta formato ISO 8601
            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            // Tenta formato yyyy-MM-dd
            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
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

        private string FormatCep(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8)
                return cep;

            return $"{cep.Substring(0, 5)}-{cep.Substring(5, 3)}";
        }

        private string FormatPhone(string ddd, string number)
        {
            if (string.IsNullOrWhiteSpace(ddd) || string.IsNullOrWhiteSpace(number))
                return $"{ddd}{number}";

            if (number.Length == 8)
                return $"({ddd}) {number.Substring(0, 4)}-{number.Substring(4, 4)}";

            if (number.Length == 9)
                return $"({ddd}) {number.Substring(0, 5)}-{number.Substring(5, 4)}";

            return $"({ddd}) {number}";
        }

        private string FormatCnae(string cnae)
        {
            if (string.IsNullOrWhiteSpace(cnae))
                return cnae;

            // Remove caracteres não numéricos e barras
            cnae = cnae.Replace("-", "").Replace("/", "");

            if (cnae.Length != 7)
                return cnae;

            return $"{cnae.Substring(0, 2)}.{cnae.Substring(2, 2)}-{cnae.Substring(4, 1)}-{cnae.Substring(5, 2)}";
        }
    }
}
