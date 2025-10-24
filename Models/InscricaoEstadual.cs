using System;

namespace GetCNPJ.Models
{
    /// <summary>
    /// Representa uma inscrição estadual
    /// </summary>
    public class InscricaoEstadual
    {
        /// <summary>
        /// Número da inscrição estadual
        /// </summary>
        public string Inscricao { get; set; }

        /// <summary>
        /// Estado da inscrição (UF)
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Indica se a inscrição está ativa
        /// </summary>
        public bool Ativo { get; set; }

        /// <summary>
        /// Data de atualização da inscrição
        /// </summary>
        public DateTime? DataAtualizacao { get; set; }

        public override string ToString()
        {
            var status = Ativo ? "Ativa" : "Inativa";
            return $"{Inscricao} ({Estado}) - {status}";
        }
    }
}
