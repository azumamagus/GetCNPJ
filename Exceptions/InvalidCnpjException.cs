using System;

namespace GetCNPJ.Exceptions
{
    /// <summary>
    /// Exceção lançada quando o CNPJ é inválido
    /// </summary>
    public class InvalidCnpjException : CnpjException
    {
        public string Cnpj { get; }

        public InvalidCnpjException(string cnpj)
            : base($"CNPJ inválido: {cnpj}")
        {
            Cnpj = cnpj;
        }

        public InvalidCnpjException(string cnpj, Exception innerException)
            : base($"CNPJ inválido: {cnpj}", innerException)
        {
            Cnpj = cnpj;
        }
    }
}
