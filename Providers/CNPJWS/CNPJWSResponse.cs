using System;
using System.Collections.Generic;

namespace GetCNPJ.Providers.CNPJWS
{
    internal class CNPJWSResponse
    {
        public string cnpj_raiz { get; set; }
        public string razao_social { get; set; }
        public string capital_social { get; set; }
        public string responsavel_federativo { get; set; }
        public DateTime? atualizado_em { get; set; }
        public PorteWS porte { get; set; }
        public NaturezaJuridicaWS natureza_juridica { get; set; }
        public QualificacaoWS qualificacao_do_responsavel { get; set; }
        public List<SocioWS> socios { get; set; }
        public SimplesWS simples { get; set; }
        public EstabelecimentoWS estabelecimento { get; set; }
    }

    internal class PorteWS
    {
        public string id { get; set; }
        public string descricao { get; set; }
    }

    internal class NaturezaJuridicaWS
    {
        public string id { get; set; }
        public string descricao { get; set; }
    }

    internal class QualificacaoWS
    {
        public int id { get; set; }
        public string descricao { get; set; }
    }

    internal class SocioWS
    {
        public string cpf_cnpj_socio { get; set; }
        public string nome { get; set; }
        public string tipo { get; set; }
        public string data_entrada { get; set; }
        public string cpf_representante_legal { get; set; }
        public string nome_representante { get; set; }
        public string faixa_etaria { get; set; }
        public DateTime? atualizado_em { get; set; }
        public string pais_id { get; set; }
        public QualificacaoSocioWS qualificacao_socio { get; set; }
        public QualificacaoSocioWS qualificacao_representante { get; set; }
        public PaisWS pais { get; set; }
    }

    internal class QualificacaoSocioWS
    {
        public int id { get; set; }
        public string descricao { get; set; }
    }

    internal class PaisWS
    {
        public string id { get; set; }
        public string iso2 { get; set; }
        public string iso3 { get; set; }
        public string nome { get; set; }
        public string comex_id { get; set; }
    }

    internal class SimplesWS
    {
        public string simples { get; set; }
        public string data_opcao_simples { get; set; }
        public string data_exclusao_simples { get; set; }
        public string mei { get; set; }
        public string data_opcao_mei { get; set; }
        public string data_exclusao_mei { get; set; }
        public DateTime? atualizado_em { get; set; }
    }

    internal class EstabelecimentoWS
    {
        public string cnpj { get; set; }
        public List<AtividadeWS> atividades_secundarias { get; set; }
        public string cnpj_raiz { get; set; }
        public string cnpj_ordem { get; set; }
        public string cnpj_digito_verificador { get; set; }
        public string tipo { get; set; }
        public string nome_fantasia { get; set; }
        public string situacao_cadastral { get; set; }
        public string data_situacao_cadastral { get; set; }
        public string data_inicio_atividade { get; set; }
        public string nome_cidade_exterior { get; set; }
        public string tipo_logradouro { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public string ddd1 { get; set; }
        public string telefone1 { get; set; }
        public string ddd2 { get; set; }
        public string telefone2 { get; set; }
        public string ddd_fax { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string situacao_especial { get; set; }
        public string data_situacao_especial { get; set; }
        public DateTime? atualizado_em { get; set; }
        public AtividadeWS atividade_principal { get; set; }
        public PaisWS pais { get; set; }
        public EstadoWS estado { get; set; }
        public CidadeWS cidade { get; set; }
        public MotivoSituacaoWS motivo_situacao_cadastral { get; set; }
        public List<InscricaoEstadualWS> inscricoes_estaduais { get; set; }
    }

    internal class AtividadeWS
    {
        public string id { get; set; }
        public string secao { get; set; }
        public string divisao { get; set; }
        public string grupo { get; set; }
        public string classe { get; set; }
        public string subclasse { get; set; }
        public string descricao { get; set; }
    }

    internal class EstadoWS
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string sigla { get; set; }
        public int ibge_id { get; set; }
    }

    internal class CidadeWS
    {
        public int id { get; set; }
        public string nome { get; set; }
        public int ibge_id { get; set; }
        public string siafi_id { get; set; }
    }

    internal class MotivoSituacaoWS
    {
        public int id { get; set; }
        public string descricao { get; set; }
    }

    internal class InscricaoEstadualWS
    {
        public string inscricao_estadual { get; set; }
        public bool ativo { get; set; }
        public DateTime? atualizado_em { get; set; }
        public EstadoWS estado { get; set; }
    }
}
