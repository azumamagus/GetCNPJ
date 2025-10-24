using System;

namespace GetCNPJ.Exceptions
{
    /// <summary>
    /// Exceção base para erros relacionados a CNPJ
    /// </summary>
    public class CnpjException : Exception
    {
        public CnpjException() : base() { }

        public CnpjException(string message) : base(message) { }

        public CnpjException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
