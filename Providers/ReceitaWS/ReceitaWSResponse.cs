using System;
using System.Collections.Generic;

namespace GetCNPJ.Providers.ReceitaWS
{
    internal class ReceitaWSResponse
    {
        public string abertura { get; set; }
        public string situacao { get; set; }
        public string tipo { get; set; }
        public string nome { get; set; }
        public string fantasia { get; set; }
        public string porte { get; set; }
        public string natureza_juridica { get; set; }
        public List<AtividadeWS> atividade_principal { get; set; }
        public List<AtividadeWS> atividades_secundarias { get; set; }
        public List<SocioWS> qsa { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string municipio { get; set; }
        public string bairro { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }
        public string data_situacao { get; set; }
        public string cnpj { get; set; }
        public DateTime? ultima_atualizacao { get; set; }
        public string status { get; set; }
        public string capital_social { get; set; }
        public SimplesWS simples { get; set; }
        public SimplesWS simei { get; set; }
    }

    internal class AtividadeWS
    {
        public string code { get; set; }
        public string text { get; set; }
    }

    internal class SocioWS
    {
        public string nome { get; set; }
        public string qual { get; set; }
    }

    internal class SimplesWS
    {
        public bool optante { get; set; }
        public string data_opcao { get; set; }
        public string data_exclusao { get; set; }
    }
}
