namespace GetCNPJ.Models
{
    /// <summary>
    /// Representa uma atividade econômica (CNAE)
    /// </summary>
    public class AtividadeEconomica
    {
        /// <summary>
        /// Código da atividade (CNAE)
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Descrição da atividade
        /// </summary>
        public string Descricao { get; set; }

        public override string ToString()
        {
            return $"{Codigo} - {Descricao}";
        }
    }
}
