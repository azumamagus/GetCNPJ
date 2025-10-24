using System;
using System.Collections.Generic;

namespace GetCNPJ.Providers.BrasilAPI
{
    internal class BrasilAPIResponse
    {
        public string cnpj { get; set; }
        public int? identificador_matriz_filial { get; set; }
        public string descricao_matriz_filial { get; set; }
        public string razao_social { get; set; }
        public string nome_fantasia { get; set; }
        public string situacao_cadastral { get; set; }
        public string descricao_situacao_cadastral { get; set; }
        public string data_situacao_cadastral { get; set; }
        public int? motivo_situacao_cadastral { get; set; }
        public string nome_cidade_exterior { get; set; }
        public int? codigo_natureza_juridica { get; set; }
        public string data_inicio_atividade { get; set; }
        public int? cnae_fiscal { get; set; }
        public string cnae_fiscal_descricao { get; set; }
        public string descricao_tipo_logradouro { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public string uf { get; set; }
        public int? codigo_municipio { get; set; }
        public string municipio { get; set; }
        public string ddd_telefone_1 { get; set; }
        public string ddd_telefone_2 { get; set; }
        public string ddd_fax { get; set; }
        public string qualificacao_do_responsavel { get; set; }
        public decimal? capital_social { get; set; }
        public int? porte { get; set; }
        public string descricao_porte { get; set; }
        public bool? opcao_pelo_simples { get; set; }
        public string data_opcao_pelo_simples { get; set; }
        public string data_exclusao_do_simples { get; set; }
        public bool? opcao_pelo_mei { get; set; }
        public string situacao_especial { get; set; }
        public string data_situacao_especial { get; set; }
        public List<CnaeFiscalBrasil> cnaes_secundarios { get; set; }
        public string natureza_juridica { get; set; }
        public List<QsaBrasil> qsa { get; set; }
    }

    internal class CnaeFiscalBrasil
    {
        public int? codigo { get; set; }
        public string descricao { get; set; }
    }

    internal class QsaBrasil
    {
        public int? identificador_de_socio { get; set; }
        public string nome_socio { get; set; }
        public string cnpj_cpf_do_socio { get; set; }
        public int? codigo_qualificacao_socio { get; set; }
        public string qualificacao_socio { get; set; }
        public string data_entrada_sociedade { get; set; }
        public string pais { get; set; }
        public string representante_legal { get; set; }
        public string nome_representante_legal { get; set; }
        public string codigo_qualificacao_representante_legal { get; set; }
    }
}
