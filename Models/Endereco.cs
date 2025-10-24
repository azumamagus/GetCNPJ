namespace GetCNPJ.Models
{
    /// <summary>
    /// Representa o endereço de uma empresa
    /// </summary>
    public class Endereco
    {
        /// <summary>
        /// CEP formatado (XXXXX-XXX)
        /// </summary>
        public string Cep { get; set; }

        /// <summary>
        /// Logradouro (rua, avenida, etc)
        /// </summary>
        public string Logradouro { get; set; }

        /// <summary>
        /// Número do endereço
        /// </summary>
        public string Numero { get; set; }

        /// <summary>
        /// Complemento (sala, andar, etc)
        /// </summary>
        public string Complemento { get; set; }

        /// <summary>
        /// Bairro
        /// </summary>
        public string Bairro { get; set; }

        /// <summary>
        /// Município
        /// </summary>
        public string Municipio { get; set; }

        /// <summary>
        /// UF (estado)
        /// </summary>
        public string Uf { get; set; }

        /// <summary>
        /// Retorna o endereço formatado
        /// </summary>
        public string EnderecoCompleto
        {
            get
            {
                var endereco = $"{Logradouro}, {Numero}";
                if (!string.IsNullOrWhiteSpace(Complemento))
                    endereco += $" - {Complemento}";
                endereco += $" - {Bairro}, {Municipio}/{Uf}";
                if (!string.IsNullOrWhiteSpace(Cep))
                    endereco += $" - CEP: {Cep}";
                return endereco;
            }
        }
    }
}
