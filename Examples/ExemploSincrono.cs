using System;
using GetCNPJ;

namespace GetCNPJ.Examples
{
    /// <summary>
    /// Exemplo de uso síncrono da biblioteca GetCNPJ
    /// Ideal para aplicações Console, Windows Forms, ou aplicações legadas que não usam async/await
    /// </summary>
    public class ExemploSincrono
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== GetCNPJ - Exemplo de Uso Síncrono ===\n");

            // Criar cliente
            using var client = new CnpjClient();

            // Consultar CNPJ de forma síncrona
            Console.WriteLine("Consultando CNPJ 03.312.791/0001-83...\n");

            var result = client.Get("03.312.791/0001-83");

            if (result.Success)
            {
                var data = result.Data;

                Console.WriteLine("✓ Consulta realizada com sucesso!\n");
                Console.WriteLine($"CNPJ: {data.Cnpj}");
                Console.WriteLine($"Razão Social: {data.RazaoSocial}");
                Console.WriteLine($"Nome Fantasia: {data.NomeFantasia}");
                Console.WriteLine($"Situação: {data.Situacao}");
                Console.WriteLine($"Tipo: {data.Tipo}");
                Console.WriteLine($"Porte: {data.Porte}");
                Console.WriteLine($"\nEndereço:");
                Console.WriteLine($"  {data.Endereco.EnderecoCompleto}");
                Console.WriteLine($"\nContato:");
                Console.WriteLine($"  Email: {data.Email}");

                if (data.Telefones != null && data.Telefones.Count > 0)
                {
                    Console.WriteLine($"  Telefones: {string.Join(", ", data.Telefones)}");
                }

                Console.WriteLine($"\nAtividade Principal:");
                Console.WriteLine($"  {data.AtividadePrincipal}");

                if (data.QuadroSocietario != null && data.QuadroSocietario.Count > 0)
                {
                    Console.WriteLine($"\nQuadro Societário:");
                    foreach (var socio in data.QuadroSocietario)
                    {
                        Console.WriteLine($"  - {socio}");
                    }
                }

                Console.WriteLine($"\nProvedor usado: {data.Provedor}");
            }
            else
            {
                Console.WriteLine($"✗ Erro: {result.ErrorMessage}");
                Console.WriteLine("\nDetalhes dos erros:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  - {error.ProviderName}: {error.ErrorMessage}");
                }
            }

            // Exemplo com provedor específico
            Console.WriteLine("\n\n=== Consultando com Provedor Específico ===\n");

            var result2 = client.GetFromProvider("03312791000183", "BrasilAPI");

            if (result2.Success)
            {
                Console.WriteLine($"✓ Empresa: {result2.Data.RazaoSocial}");
                Console.WriteLine($"  Provedor: {result2.Data.Provedor}");
            }
            else
            {
                Console.WriteLine($"✗ Erro: {result2.ErrorMessage}");
            }

            Console.WriteLine("\n\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        /// <summary>
        /// Exemplo de uso em Windows Forms ou aplicações legadas
        /// </summary>
        public static void ExemploWindowsForms()
        {
            try
            {
                using var client = new CnpjClient();

                // Método síncrono - não bloqueia UI se chamado de forma apropriada
                var result = client.Get("03312791000183");

                if (result.Success)
                {
                    // Atualizar UI com os dados
                    string mensagem = $"Empresa: {result.Data.RazaoSocial}\n" +
                                    $"CNPJ: {result.Data.Cnpj}\n" +
                                    $"Endereço: {result.Data.Endereco.EnderecoCompleto}";

                    Console.WriteLine(mensagem);
                    // MessageBox.Show(mensagem, "Dados da Empresa");
                }
                else
                {
                    Console.WriteLine($"Erro ao consultar CNPJ: {result.ErrorMessage}");
                    // MessageBox.Show($"Erro ao consultar CNPJ: {result.ErrorMessage}", "Erro");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                // MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro");
            }
        }

        /// <summary>
        /// Exemplo de consulta em lote síncrona
        /// </summary>
        public static void ConsultarVariasCNPJs()
        {
            using var client = new CnpjClient();

            string[] cnpjs = {
                "03312791000183",
                "00000000000191",
                "18236120000158"
            };

            Console.WriteLine("=== Consultando Múltiplos CNPJs ===\n");

            foreach (var cnpj in cnpjs)
            {
                Console.WriteLine($"Consultando {cnpj}...");

                var result = client.Get(cnpj);

                if (result.Success)
                {
                    Console.WriteLine($"  ✓ {result.Data.RazaoSocial}");
                }
                else
                {
                    Console.WriteLine($"  ✗ Erro: {result.ErrorMessage}");
                }

                Console.WriteLine();
            }
        }
    }
}
