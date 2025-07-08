# Sistema de Aluguel de Imóveis APP

Aplicação Desktop desenvolvida em WPF para consumo da API de aluguel de imóveis.

## Funcionalidades

- Login
- Cadastros de usuários
- Listagem de imóveis com paginação
- Criação de imóveis
- Exibição dos detalhes de imóveis
- Registro de Aluguel
- Exibição de relatórios com filtros

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Como executar

1. **Clone o repositório:**
```bash
   git clone repo-url
   cd aluguel-de-imoveis-wpf
```
2. **Ajuste a porta do endpoint:** Ao abrir a aplicação, vá até a pasta 'Service/BaseUrl.cs' e ajuste a URL base do endpoint.
 
3. **Execute a aplicação**.

## Estrutura do Projeto
```bash
aluguel-de-imoveis-wpf/
  ├── Assets/                 # Armazena arquivos como (imagens e fontes)
  ├── Communication/
  │   ├── Request/            # Modelos para envio de dados para API
  │   ├── Response/           # Modelos de entrada de dados da API
  │   └── Wrapper/            # Modelos emcapsulados e personalizados para uso nos Requests/Responses
  ├── Converters/             # Tratamento para exibição/formatação de dados na aplicação
  ├── Model/                  # Modelos utilizados na aplicação
  ├── Relatorios/             # Lógica para geração de relatórios
  ├── Security/               # Tratamento do token JWT
  ├── Services/               # Responsável por se comunicar com o backend
  ├── Utils/                  # Classes de Utilidade e Enums
  │   └── Enums/              # Armazena dados do tipo enum
  ├── View                    # Arquivos Xaml para exibição das páginas
  ├── ViewModel               # Classes para tratar dados da View
  └── MainWindow.xaml.cs      # Contém as injeções de dados para as páginas
```
