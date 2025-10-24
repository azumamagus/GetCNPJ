using System;
using System.Collections.Generic;
using System.Linq;

namespace GetCNPJ.Models
{
    /// <summary>
    /// Resultado da consulta de CNPJ
    /// </summary>
    public class CnpjResult
    {
        /// <summary>
        /// Indica se a consulta foi bem-sucedida
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Dados do CNPJ (null se não houver sucesso)
        /// </summary>
        public CnpjData Data { get; set; }

        /// <summary>
        /// Mensagem de erro (null se houver sucesso)
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Lista de erros ocorridos durante as tentativas
        /// </summary>
        public List<ProviderError> Errors { get; set; }

        /// <summary>
        /// Provedores que falharam na consulta
        /// </summary>
        public List<string> FailedProviders
        {
            get { return Errors?.Select(e => e.ProviderName).ToList() ?? new List<string>(); }
        }

        public CnpjResult()
        {
            Errors = new List<ProviderError>();
        }

        /// <summary>
        /// Cria um resultado de sucesso
        /// </summary>
        public static CnpjResult CreateSuccess(CnpjData data)
        {
            return new CnpjResult
            {
                Success = true,
                Data = data
            };
        }

        /// <summary>
        /// Cria um resultado de erro
        /// </summary>
        public static CnpjResult CreateError(string errorMessage, List<ProviderError> errors = null)
        {
            return new CnpjResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                Errors = errors ?? new List<ProviderError>()
            };
        }
    }

    /// <summary>
    /// Representa um erro ocorrido em um provedor
    /// </summary>
    public class ProviderError
    {
        /// <summary>
        /// Nome do provedor
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Mensagem de erro
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Exception que causou o erro
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Timestamp do erro
        /// </summary>
        public DateTime Timestamp { get; set; }

        public ProviderError()
        {
            Timestamp = DateTime.Now;
        }
    }
}
