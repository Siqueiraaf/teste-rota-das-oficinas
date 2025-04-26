# API de E-commerce Rota das Oficinas

## Visão Geral

Esta documentação descreve a implementação da API de e-commerce desenvolvida para o teste técnico da Rota das Oficinas. A API foi construída seguindo os padrões CQRS (Command Query Responsibility Segregation) e Repository Pattern, utilizando .NET 8.0, Entity Framework Core e PostgreSQL.

## Arquitetura

A solução foi estruturada seguindo os princípios da arquitetura limpa (Clean Architecture), com separação clara de responsabilidades:

- **Domain**: Contém as entidades de negócio e regras de domínio
- **Application**: Implementa a lógica de aplicação, comandos e consultas (CQRS)
- **Persistence**: Gerencia o acesso a dados e implementa os repositórios
- **Infrastructure**: Fornece implementações concretas para serviços externos
- **WebApi**: Expõe os endpoints da API e gerencia a comunicação HTTP
- **Tests**: Contém os testes unitários para validar o comportamento do sistema

## Funcionalidades Implementadas

### 1. Controle de Clientes, Vendas e Produtos

Foram implementadas operações CRUD (Criação, Leitura, Atualização e Remoção) para:

- **Produtos**: Gerenciamento completo do catálogo de produtos
- **Vendas**: Registro de transações de venda com itens associados
- **Clientes**: Gerenciamento de usuários do sistema

### 2. Autenticação e Autorização

- Sistema de login com geração de tokens JWT
- Controle de acesso baseado em roles (Admin e Customer)
- Proteção de endpoints sensíveis com autorização por role

### 3. Paginação, Filtragem e Ordenação

- Implementação de paginação em todas as listagens
- Filtragem por diversos critérios (ex: nome, data, preço)
- Ordenação customizável por diferentes campos
- Controle de tamanho de página e navegação entre páginas

### 4. Endpoints da API

Todos os endpoints "necessários" foram implementados e são acessíveis via:

- **AuthController**: Endpoints para login e registro de usuários
- **ProductsController**: Endpoints para gerenciamento de produtos
- **SalesController**: Endpoints para gerenciamento de vendas e análise

### 5. Análise de Vendas

- Endpoint específico para análise de vendas por período
- Retorno de quantidade de vendas no período
- Cálculo de renda total gerada no período
- Detalhamento da renda por produto vendido

### 6. Testes Unitários

- Implementação de testes para validar as regras de negócio
- Cobertura dos principais fluxos de comandos e consultas
- Utilização de Xunit, Moq e FluentAssertions

## Exemplo de estrutura com alguns dos Arquivos Implementados

### Entidades do Domínio
- `/RO.DevTest.Domain/Entities/Product.cs`
- `/RO.DevTest.Domain/Entities/Sale.cs`
- `/RO.DevTest.Domain/Entities/SaleItem.cs`

### Repositórios
- `/RO.DevTest.Application/Contracts/Persistance/Repositories/IBaseRepository.cs`
- `/RO.DevTest.Application/Contracts/Persistance/Repositories/IProductRepository.cs`
- `/RO.DevTest.Application/Contracts/Persistance/Repositories/ISaleRepository.cs`
- `/RO.DevTest.Persistence/Repositories/BaseRepository.cs`
- `/RO.DevTest.Persistence/Repositories/ProductRepository.cs`
- `/RO.DevTest.Persistence/Repositories/SaleRepository.cs`

### Configurações do Entity Framework
- `/RO.DevTest.Persistence/Configurations/ProductConfiguration.cs`
- `/RO.DevTest.Persistence/Configurations/SaleConfiguration.cs`
- `/RO.DevTest.Persistence/Configurations/SaleItemConfiguration.cs`
- `/RO.DevTest.Persistence/DefaultContext.cs`

### Autenticação e Autorização
- `/RO.DevTest.Application/Features/Auth/Commands/LoginCommand/LoginCommand.cs`
- `/RO.DevTest.Application/Features/Auth/Commands/LoginCommand/LoginResult.cs`
- `/RO.DevTest.Application/Features/Auth/Commands/LoginCommand/LoginCommandHandler.cs`
- `/RO.DevTest.Infrastructure/Abstractions/IdentityAbstractor.cs`

### Comandos e Queries de Produto
- `/RO.DevTest.Application/Features/Product/Commands/CreateProductCommand/CreateProductCommand.cs`
- `/RO.DevTest.Application/Features/Product/Commands/UpdateProductCommand/UpdateProductCommand.cs`
- `/RO.DevTest.Application/Features/Product/Commands/DeleteProductCommand/DeleteProductCommand.cs`
- `/RO.DevTest.Application/Features/Product/Queries/GetProductByIdQuery/GetProductByIdQuery.cs`
- `/RO.DevTest.Application/Features/Product/Queries/GetProductsQuery/GetProductsQuery.cs`

### Análise de Vendas
- `/RO.DevTest.Application/Features/Sale/Queries/GetSalesAnalysisQuery/GetSalesAnalysisQuery.cs`
- `/RO.DevTest.Application/Features/Sale/Queries/GetSalesAnalysisQuery/SalesAnalysisVm.cs`
- `/RO.DevTest.Application/Features/Sale/Queries/GetSalesAnalysisQuery/GetSalesAnalysisQueryHandler.cs`

### Controllers
- `/RO.DevTest.WebApi/Controllers/AuthController.cs`
- `/RO.DevTest.WebApi/Controllers/ProductsController.cs`
- `/RO.DevTest.WebApi/Controllers/SalesController.cs`

### Testes Unitários
- `/RO.DevTest.Tests/Unit/Application/Features/Product/Commands/CreateProductCommandHandlerTests.cs`
- `/RO.DevTest.Tests/Unit/Application/Features/Product/Queries/GetProductByIdQueryHandlerTests.cs`
- `/RO.DevTest.Tests/Unit/Application/Features/Sale/Commands/CreateSaleCommandHandlerTests.cs`

### Configuração da Aplicação
- `/RO.DevTest.WebApi/Program.cs`
- `/RO.DevTest.WebApi/appsettings.json`
- `/RO.DevTest.Persistence/SeedData.cs`

## Como Executar o Projeto

1. **Configurar o Banco de Dados**:
   - Instale o PostgreSQL se ainda não estiver instalado
   - Atualize a string de conexão em `appsettings.json` se necessário

2. **Executar as Migrações**:
   ```bash
   dotnet ef database update -p RO.DevTest.Persistence -s RO.DevTest.WebApi
   ```

3. **Executar a API**:
   ```bash
   cd RO.DevTest.WebApi
   dotnet run
   ```

4. **Acessar a Documentação da API**:
   - Navegue para `https://localhost:5001/swagger` para visualizar e testar os endpoints

## Usuários Pré-configurados

A aplicação cria automaticamente dois usuários para teste:

1. **Administrador**:
   - Email: admin@example.com
   - Senha: Admin123!
   - Role: Admin

2. **Cliente**:
   - Email: customer@example.com
   - Senha: Customer123!
   - Role: Customer

## Decisões Técnicas | O que eu entendi do projeto.

### Uso de CQRS
Creio que o padrão CQRS foi escolhido para separar claramente as operações de leitura e escrita, facilitando a manutenção e testabilidade do código. Implementei cada comando(Command) ou consulta(Queries) em sua própria classe e handler, o que torna o código mais modular e fácil de entender.

### Repository Pattern
Foi implementado para abstrair o acesso a dados, permitindo que a lógica de negócio seja independente da tecnologia de persistência utilizada. E Isso facilitará a substituição do ORM ou banco de dados no futuro, se necessário.

### Autenticação com JWT
A autenticação baseada em tokens JWT foi escolhida por ser stateless e escalável, ideal para APIs RESTful. Os tokens contêm informações sobre o usuário e suas permissões, permitindo um controle de acesso granular. Eu achei a parte do Enuns muito 

### Paginação e Filtragem
A implementação de paginação e filtragem foi feita de forma a otimizar o desempenho da API, evitando o carregamento desnecessário de grandes volumes de dados. Os parâmetros de paginação, filtragem e ordenação são passados via query string.

### Análise de Vendas
A funcionalidade de análise de vendas foi implementada com métodos específicos no repositório de vendas, permitindo consultas eficientes diretamente no banco de dados. Os resultados são transformados em DTOs para apresentação ao cliente, assim não Utilizaria diretamente o as nossas entidades principais o que poderia sobrecarregar a perfomance.

## Conclusão
A documentação via Swagger facilita o entendimento e uso da API.

Tentei implementar ao máximo tudo que foi solicitado. Eu não tinha experiência anterior com GitHub Workflows nem com a arquitetura CQRS, então precisei dedicar bastante tempo estudando e aplicando esses conceitos e sei que não ficou dos melhores. Consegui avançar bastante: usei o exemplo disponível de testes unitários para criar meus próprios "mocks" e busquei seguir o máximo possível os padrões propostos.

Apesar de não conseguir entregar 100% do que dava pra fazer, estou feliz por ter aprendido muito durante o processo. Achei o teste muito maneiro, bem elaborado e explicado, o que ajudou bastante no desenvolvimento.

Agradeço pela oportunidade!

<<<<<<< HEAD
A API de e-commerce desenvolvida atende a todos os requisitos especificados no teste técnico exceto os adcionais.
=======
A API de e-commerce desenvolvida atende a todos os requisitos especificados no teste técnico exceto os adcionais .
Estarei deixando detlhado o uso do docker

## Documentação de Uso - Aplicação Docker `ro-devtest`

## Requisitos

- **Docker** instalado na sua máquina. Caso ainda não tenha o Docker, siga as instruções de instalação [aqui](https://docs.docker.com/get-docker/).

## Rodando a Aplicação

Existem duas formas de rodar a aplicação: usando a **Interface Gráfica do Docker** ou a **Linha de Comando**. Abaixo, explico ambas as opções.

---

### 1. Usando a Interface Gráfica (Docker Desktop)

1. **Abrir o Docker Desktop**:
   - Inicie o Docker Desktop na sua máquina.

2. **Localizar a Imagem**:
   - No painel esquerdo, clique na aba **"Images"** (Imagens). Lá você verá a imagem `ro-devtest` listada.

3. **Rodar a Imagem**:
   - Clique na imagem `ro-devtest` e depois no botão **"Run"**.
   - O Docker criará e iniciará automaticamente o contêiner para você.

4. **Acessar a Aplicação**:
   - Acesse a aplicação no seu navegador em **`http://localhost:8080`** ou na porta que foi configurada.

---

### 2. Usando a Linha de Comando

1. **Abrir o Terminal**:
   - No Windows, abra o **PowerShell** ou o **Prompt de Comando**.
   - No macOS ou Linux, abra o **Terminal**.

2. **Rodar o Contêiner**:
   - Execute o seguinte comando para rodar o contêiner:

     ```bash
     docker run -d -p 8080:80 ro-devtest
     ```

   **Explicação dos parâmetros**:
   - **`-d`**: Executa o contêiner em segundo plano (modo "detached").
   - **`-p 8080:80`**: Mapeia a porta 8080 do seu computador para a porta 80 dentro do contêiner, onde a aplicação estará rodando.
   - **`ro-devtest`**: Nome da imagem Docker criada.

3. **Verificar a Aplicação**:
   - A aplicação estará acessível em **`http://localhost:8080`** no seu navegador.

4. **Verificar Contêineres em Execução**:
   - Caso queira verificar os contêineres que estão em execução, use o comando:

     ```bash
     docker ps
     ```

---

## Parando o Contêiner

Se precisar parar o contêiner, execute o seguinte comando:

```bash
docker stop <CONTAINER_ID>
>>>>>>> 87398b86bc8dff5d89569edccb7cd2d860e6e0fe
