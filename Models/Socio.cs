namespace GetCNPJ.Models
{
    /// <summary>
    /// Representa um sócio da empresa
    /// </summary>
    public class Socio
    {
        /// <summary>
        /// Nome do sócio
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Qualificação do sócio
        /// </summary>
        public string Qualificacao { get; set; }

        public override string ToString()
        {
            return $"{Nome} - {Qualificacao}";
        }
    }
}
