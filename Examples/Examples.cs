using System;
using System.Threading.Tasks;
using GetCNPJ;

namespace GetCNPJ.Examples
{
    /// <summary>
    /// Exemplos de uso da biblioteca GetCNPJ
    /// </summary>
    public class Examples
    {
        /// <summary>
        /// Exemplo básico de consulta
        /// </summary>
        public static async Task BasicExample()
        {
            Console.WriteLine("=== Exemplo Básico ===\n");

            using var client = new CnpjClient();

            var result = await client.GetAsync("03.312.791/0001-83");

            if (result.Success)
            {
                var data = result.Data;

                Console.WriteLine($"✓ Consulta realizada com sucesso!");
                Console.WriteLine($"\nRazão Social: {data.RazaoSocial}");
                Console.WriteLine($"Nome Fantasia: {data.NomeFantasia}");
                Console.WriteLine($"CNPJ: {data.Cnpj}");
                Console.WriteLine($"Situação: {data.Situacao}");
                Console.WriteLine($"Tipo: {data.Tipo}");
                Console.WriteLine($"Porte: {data.Porte}");
                Console.WriteLine($"\nEndereço:");
                Console.WriteLine($"  {data.Endereco.EnderecoCompleto}");
                Console.WriteLine($"\nContato:");
                Console.WriteLine($"  Email: {data.Email}");
                Console.WriteLine($"  Telefones: {string.Join(", ", data.Telefones)}");
                Console.WriteLine($"\nAtividade Principal:");
                Console.WriteLine($"  {data.AtividadePrincipal}");
                Console.WriteLine($"\nQuadro Societário:");
                foreach (var socio in data.QuadroSocietario)
                {
                    Console.WriteLine($"  - {socio}");
                }
                Console.WriteLine($"\nProvedor usado: {data.Provedor}");
            }
            else
            {
                Console.WriteLine($"✗ Erro: {result.ErrorMessage}");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  - {error.ProviderName}: {error.ErrorMessage}");
                }
            }
        }

        /// <summary>
        /// Exemplo com provedor específico
        /// </summary>
        public static async Task SpecificProviderExample()
        {
            Console.WriteLine("\n\n=== Exemplo com Provedor Específico ===\n");

            using var client = new CnpjClient();

            // Testa cada provedor
            string[] providers = { "ReceitaWS", "BrasilAPI", "CNPJA" };
            var cnpj = "03312791000183";

            foreach (var provider in providers)
            {
                Console.WriteLine($"\nTestando provedor: {provider}");
                var result = await client.GetFromProviderAsync(cnpj, provider);

                if (result.Success)
                {
                    Console.WriteLine($"  ✓ {result.Data.RazaoSocial}");
                }
                else
                {
                    Console.WriteLine($"  ✗ {result.ErrorMessage}");
                }
            }
        }

        /// <summary>
        /// Exemplo com configuração customizada
        /// </summary>
        public static async Task CustomConfigExample()
        {
            Console.WriteLine("\n\n=== Exemplo com Configuração Customizada ===\n");

            var options = new CnpjClientOptions
            {
                MaxRequestsPerMinute = 3,
                Timeout = TimeSpan.FromSeconds(30),
                EnableReceitaWS = true,
                EnableBrasilAPI = true,
                EnableCNPJA = false // Desabilita CNPJA
            };

            using var client = new CnpjClient(null, options);

            var result = await client.GetAsync("03312791000183");

            if (result.Success)
            {
                Console.WriteLine($"✓ Empresa: {result.Data.RazaoSocial}");
                Console.WriteLine($"  Provedor: {result.Data.Provedor}");
                Console.WriteLine($"  (CNPJA foi desabilitado nas opções)");
            }
        }

        /// <summary>
        /// Exemplo de tratamento de erros
        /// </summary>
        public static async Task ErrorHandlingExample()
        {
            Console.WriteLine("\n\n=== Exemplo de Tratamento de Erros ===\n");

            using var client = new CnpjClient();

            // Testa CNPJ inválido
            Console.WriteLine("Testando CNPJ inválido:");
            var result = await client.GetAsync("00000000000000");

            if (!result.Success)
            {
                Console.WriteLine($"  ✓ Erro capturado: {result.ErrorMessage}");
            }

            // Testa CNPJ inexistente
            Console.WriteLine("\nTestando CNPJ inexistente:");
            result = await client.GetAsync("12345678901234");

            if (!result.Success)
            {
                Console.WriteLine($"  ✓ Erro capturado: {result.ErrorMessage}");
                Console.WriteLine($"  Provedores que falharam:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"    - {error.ProviderName}: {error.ErrorMessage}");
                }
            }
        }

        /// <summary>
        /// Exemplo de múltiplas consultas (demonstra rate limiting)
        /// </summary>
        public static async Task RateLimitExample()
        {
            Console.WriteLine("\n\n=== Exemplo de Rate Limiting ===\n");
            Console.WriteLine("Fazendo múltiplas consultas para demonstrar o rate limiting...\n");

            using var client = new CnpjClient();

            string[] cnpjs = {
                "03312791000183", // SOS System
                "00000000000191", // Banco do Brasil
                "18236120000158", // Nubank
                "33000167000101"  // Banco Santander
            };

            for (int i = 0; i < cnpjs.Length; i++)
            {
                Console.WriteLine($"[{i + 1}/{cnpjs.Length}] Consultando CNPJ {cnpjs[i]}...");
                var startTime = DateTime.Now;

                var result = await client.GetAsync(cnpjs[i]);

                var elapsed = DateTime.Now - startTime;

                if (result.Success)
                {
                    Console.WriteLine($"  ✓ {result.Data.RazaoSocial}");
                    Console.WriteLine($"  Tempo: {elapsed.TotalSeconds:F2}s");
                }
                else
                {
                    Console.WriteLine($"  ✗ Erro: {result.ErrorMessage}");
                }

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Executa todos os exemplos
        /// </summary>
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("╔═══════════════════════════════════════════╗");
            Console.WriteLine("║   GetCNPJ - Exemplos de Uso              ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝");

            try
            {
                await BasicExample();
                await SpecificProviderExample();
                await CustomConfigExample();
                await ErrorHandlingExample();
                await RateLimitExample();

                Console.WriteLine("\n\n✓ Todos os exemplos foram executados com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Erro inesperado: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}
