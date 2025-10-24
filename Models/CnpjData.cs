using System;
using System.Collections.Generic;

namespace GetCNPJ.Models
{
    /// <summary>
    /// Representa os dados padronizados de um CNPJ
    /// </summary>
    public class CnpjData
    {
        /// <summary>
        /// CNPJ formatado (XX.XXX.XXX/XXXX-XX)
        /// </summary>
        public string Cnpj { get; set; }

        /// <summary>
        /// Razão Social da empresa
        /// </summary>
        public string RazaoSocial { get; set; }

        /// <summary>
        /// Nome Fantasia da empresa
        /// </summary>
        public string NomeFantasia { get; set; }

        /// <summary>
        /// Data de abertura da empresa
        /// </summary>
        public DateTime? DataAbertura { get; set; }

        /// <summary>
        /// Situação cadastral da empresa
        /// </summary>
        public string Situacao { get; set; }

        /// <summary>
        /// Data da situação cadastral
        /// </summary>
        public DateTime? DataSituacao { get; set; }

        /// <summary>
        /// Tipo de estabelecimento (Matriz/Filial)
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// Porte da empresa
        /// </summary>
        public string Porte { get; set; }

        /// <summary>
        /// Natureza jurídica
        /// </summary>
        public string NaturezaJuridica { get; set; }

        /// <summary>
        /// Capital social da empresa
        /// </summary>
        public decimal? CapitalSocial { get; set; }

        /// <summary>
        /// Endereço completo da empresa
        /// </summary>
        public Endereco Endereco { get; set; }

        /// <summary>
        /// Atividade principal da empresa
        /// </summary>
        public AtividadeEconomica AtividadePrincipal { get; set; }

        /// <summary>
        /// Atividades secundárias da empresa
        /// </summary>
        public List<AtividadeEconomica> AtividadesSecundarias { get; set; }

        /// <summary>
        /// Quadro societário
        /// </summary>
        public List<Socio> QuadroSocietario { get; set; }

        /// <summary>
        /// Telefones da empresa
        /// </summary>
        public List<string> Telefones { get; set; }

        /// <summary>
        /// Email da empresa
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Inscrições estaduais da empresa
        /// </summary>
        public List<InscricaoEstadual> InscricoesEstaduais { get; set; }

        /// <summary>
        /// Informações sobre Simples Nacional
        /// </summary>
        public SimplesNacional Simples { get; set; }

        /// <summary>
        /// Data da última atualização dos dados
        /// </summary>
        public DateTime? UltimaAtualizacao { get; set; }

        /// <summary>
        /// Provedor que forneceu os dados (ReceitaWS, BrasilAPI, CNPJA)
        /// </summary>
        public string Provedor { get; set; }

        public CnpjData()
        {
            AtividadesSecundarias = new List<AtividadeEconomica>();
            QuadroSocietario = new List<Socio>();
            Telefones = new List<string>();
            InscricoesEstaduais = new List<InscricaoEstadual>();
        }
    }
}
