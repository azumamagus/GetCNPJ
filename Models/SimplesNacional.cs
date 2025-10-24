using System;

namespace GetCNPJ.Models
{
    /// <summary>
    /// Informações sobre o Simples Nacional
    /// </summary>
    public class SimplesNacional
    {
        /// <summary>
        /// Indica se é optante pelo Simples Nacional
        /// </summary>
        public bool Optante { get; set; }

        /// <summary>
        /// Data de opção pelo Simples Nacional
        /// </summary>
        public DateTime? DataOpcao { get; set; }

        /// <summary>
        /// Data de exclusão do Simples Nacional
        /// </summary>
        public DateTime? DataExclusao { get; set; }

        /// <summary>
        /// Indica se é optante pelo SIMEI
        /// </summary>
        public bool OptanteSimei { get; set; }
    }
}
