# GetCNPJ

[![NuGet](https://img.shields.io/nuget/v/GetCNPJ.svg)](https://www.nuget.org/packages/GetCNPJ/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Biblioteca .NET para consulta de CNPJ em APIs públicas brasileiras, com suporte a múltiplos provedores e rate limiting integrado.

## 📋 Características

- ✅ **Multi-Target**: Suporta .NET Framework 4.8, 4.8.1, .NET 8, .NET 9 e .NET Standard 2.0
- 🔄 **Chain of Responsibility**: Tenta automaticamente outros provedores em caso de falha
- ⏱️ **Rate Limiting**: Controle automático de 3 requisições por minuto por provedor
- 🎯 **Retorno Padronizado**: Dados consistentes independente do provedor utilizado
- 📋 **Inscrição Estadual**: Suporte completo a inscrições estaduais (via CNPJ.WS)
- 🔌 **Múltiplos Provedores**:
  - **CNPJ.WS** (https://publica.cnpj.ws) - **Provedor padrão** ⭐ *Único que retorna inscrição estadual*
  - ReceitaWS (https://receitaws.com.br)
  - BrasilAPI (https://brasilapi.com.br)
  - CNPJA (https://open.cnpja.com)
- 🛡️ **Validação**: Validação automática de dígitos verificadores do CNPJ
- ⚡ **Async/Await**: API totalmente assíncrona + métodos síncronos para compatibilidade
- 🎨 **Clean Code**: Seguindo padrões SOLID e melhores práticas

## 📦 Instalação

```bash
dotnet add package GetCNPJ
```

Ou via NuGet Package Manager:

```
Install-Package GetCNPJ
```

## 🚀 Uso Básico

### Consulta Simples

```csharp
using GetCNPJ;
using System;

// Criar cliente
using var client = new CnpjClient();

// Consultar CNPJ (aceita formatado ou apenas números)
var result = await client.GetAsync("03.312.791/0001-83");

if (result.Success)
{
    var data = result.Data;

    Console.WriteLine($"Razão Social: {data.RazaoSocial}");
    Console.WriteLine($"Nome Fantasia: {data.NomeFantasia}");
    Console.WriteLine($"CNPJ: {data.Cnpj}");
    Console.WriteLine($"Situação: {data.Situacao}");
    Console.WriteLine($"Endereço: {data.Endereco.EnderecoCompleto}");
    Console.WriteLine($"Email: {data.Email}");
    Console.WriteLine($"Telefones: {string.Join(", ", data.Telefones)}");

    // Inscrições Estaduais (disponível quando usa CNPJ.WS)
    if (data.InscricoesEstaduais != null && data.InscricoesEstaduais.Any())
    {
        Console.WriteLine("Inscrições Estaduais:");
        foreach (var ie in data.InscricoesEstaduais)
        {
            Console.WriteLine($"  - {ie}"); // Formato: Inscrição (UF) - Status
        }
    }

    Console.WriteLine($"Provedor usado: {data.Provedor}");
}
else
{
    Console.WriteLine($"Erro: {result.ErrorMessage}");

    // Exibir erros de cada provedor
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"- {error.ProviderName}: {error.ErrorMessage}");
    }
}
```

### Consulta com Provedor Específico

```csharp
using var client = new CnpjClient();

// Forçar uso de um provedor específico
var result = await client.GetFromProviderAsync("03312791000183", "BrasilAPI");

if (result.Success)
{
    Console.WriteLine($"Razão Social: {result.Data.RazaoSocial}");
}
```

### Consulta Síncrona

Para aplicações que não usam async/await, você pode usar os métodos síncronos:

```csharp
using var client = new CnpjClient();

// Método síncrono
var result = client.Get("03312791000183");

if (result.Success)
{
    Console.WriteLine($"Razão Social: {result.Data.RazaoSocial}");
    Console.WriteLine($"Nome Fantasia: {result.Data.NomeFantasia}");
}

// Provedor específico síncrono
var result2 = client.GetFromProvider("03312791000183", "ReceitaWS");
```

**Nota**: Os métodos síncronos bloqueiam a thread atual. Para aplicações modernas (ASP.NET Core, WPF, etc.), recomenda-se usar os métodos assíncronos.

### Configuração Customizada

```csharp
var options = new CnpjClientOptions
{
    MaxRequestsPerMinute = 5,           // Aumentar rate limit (use com cuidado!)
    Timeout = TimeSpan.FromSeconds(60), // Timeout customizado
    EnableReceitaWS = true,             // Habilitar/desabilitar provedores
    EnableBrasilAPI = true,
    EnableCNPJA = false                 // Desabilitar CNPJA, por exemplo
};

using var client = new CnpjClient(null, options);
var result = await client.GetAsync("03312791000183");
```

### HttpClient Customizado

```csharp
// Usar seu próprio HttpClient (recomendado em aplicações ASP.NET Core)
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(30)
};

using var client = new CnpjClient(httpClient);
var result = await client.GetAsync("03312791000183");
```

## 📊 Modelo de Dados Retornado

```csharp
public class CnpjData
{
    public string Cnpj { get; set; }                    // CNPJ formatado
    public string RazaoSocial { get; set; }             // Razão social
    public string NomeFantasia { get; set; }            // Nome fantasia
    public DateTime? DataAbertura { get; set; }         // Data de abertura
    public string Situacao { get; set; }                // Situação cadastral
    public DateTime? DataSituacao { get; set; }         // Data da situação
    public string Tipo { get; set; }                    // MATRIZ/FILIAL
    public string Porte { get; set; }                   // Porte da empresa
    public string NaturezaJuridica { get; set; }        // Natureza jurídica
    public decimal? CapitalSocial { get; set; }         // Capital social
    public Endereco Endereco { get; set; }              // Endereço completo
    public AtividadeEconomica AtividadePrincipal { get; set; }
    public List<AtividadeEconomica> AtividadesSecundarias { get; set; }
    public List<Socio> QuadroSocietario { get; set; }   // QSA
    public List<string> Telefones { get; set; }         // Telefones
    public string Email { get; set; }                   // Email
    public List<InscricaoEstadual> InscricoesEstaduais { get; set; }  // Inscrições estaduais (CNPJ.WS)
    public SimplesNacional Simples { get; set; }        // Info Simples Nacional
    public DateTime? UltimaAtualizacao { get; set; }    // Data da atualização
    public string Provedor { get; set; }                // Provedor usado
}
```

## ⚙️ Padrões de Projeto Utilizados

- **Strategy Pattern**: Diferentes implementações de provedores de API
- **Chain of Responsibility**: Tentativa automática em múltiplos provedores
- **Factory Pattern**: Criação de instâncias dos provedores
- **Repository Pattern**: Abstração da lógica de consulta
- **DTO Pattern**: Padronização dos retornos
- **Rate Limiter Pattern**: Controle de requisições usando Sliding Window

## 🔒 Rate Limiting

A biblioteca implementa rate limiting automático de 3 requisições por minuto por provedor, conforme solicitado. Cada provedor tem seu próprio contador independente.

O algoritmo utilizado é o **Sliding Window**, que garante distribuição uniforme das requisições ao longo do tempo.

## 🛠️ Arquitetura

```
GetCNPJ/
├── Models/              # DTOs padronizados
├── Interfaces/          # Contratos da biblioteca
├── Exceptions/          # Exceções customizadas
├── Enums/              # Enumerações
├── Providers/
│   ├── Base/           # Classe base para providers
│   ├── ReceitaWS/      # Provider ReceitaWS
│   ├── BrasilAPI/      # Provider BrasilAPI
│   └── CNPJA/          # Provider CNPJA
├── Services/           # Serviço principal (Chain of Responsibility)
├── RateLimiter/        # Implementação do Rate Limiter
└── CnpjClient.cs       # Cliente principal
```

## 🧪 Tratamento de Erros

A biblioteca trata erros de forma elegante:

- **CNPJ Inválido**: Lança `InvalidCnpjException`
- **Provedor Indisponível**: Tenta o próximo provedor automaticamente
- **Rate Limit Excedido**: Aguarda automaticamente antes de fazer nova requisição
- **Timeout**: Configurável via `CnpjClientOptions`
- **Todos Provedores Falharam**: Retorna `CnpjResult` com `Success = false` e lista de erros

## 📝 Requisitos

- .NET Framework 4.8+ **ou**
- .NET 8+ **ou**
- .NET Standard 2.0+

## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para:

1. Fazer fork do projeto
2. Criar uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abrir um Pull Request

## 📄 Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo LICENSE para detalhes.

## 🔗 APIs Utilizadas

- [**CNPJ.WS**](https://publica.cnpj.ws) - **API padrão** ⭐ Único provedor que retorna inscrição estadual
- [ReceitaWS](https://receitaws.com.br) - API gratuita de consulta de CNPJ
- [BrasilAPI](https://brasilapi.com.br) - API pública brasileira
- [CNPJA](https://open.cnpja.com) - API aberta de CNPJ

**Nota**: Por padrão, a biblioteca usa o provedor CNPJ.WS como primeira opção (prioridade 1) pois é o único que fornece dados de inscrição estadual. Os demais provedores são utilizados como fallback em caso de falha.

## ⚠️ Avisos Importantes

- **Rate Limiting**: Respeite os limites de requisição das APIs públicas (3 req/min por padrão)
- **Uso Responsável**: Esta biblioteca é para uso educacional e em aplicações que respeitem os termos de uso das APIs
- **Dados Públicos**: Os dados retornados são públicos e provenientes da Receita Federal do Brasil
- **Sem Garantias**: Os provedores podem ter indisponibilidade ou mudanças sem aviso prévio

## 🎯 Roadmap

- [ ] Adicionar cache de consultas
- [ ] Suporte a mais provedores
- [ ] Métricas e telemetria
- [ ] Retry policy configurável
- [ ] Suporte a consulta em lote
- [ ] Logging integrado

## 📞 Suporte

Para reportar bugs ou solicitar features, abra uma issue no GitHub.

---

Desenvolvido com ❤️ seguindo as melhores práticas de Clean Code e SOLID
