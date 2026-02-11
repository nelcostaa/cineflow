# Cineflow - Sistema de Gerenciamento de Cinema

> API RESTful completa para gerenciar sessões de filmes, salas de cinema e venda de ingressos, desenvolvida como Projeto Final do curso de Desenvolvimento Web com .NET.

---

## Sobre o Projeto

O **Cineflow** é uma API RESTful desenvolvida para gerenciar um cinema, incluindo:

- Catálogo de filmes em cartaz com integração à API do TMDB (The Movie Database)
- Gestão de salas e suas capacidades
- Criação e controle de sessões com validação de conflitos de horário
- Sistema de venda de ingressos com verificação de lotação e assentos
- Relatórios de ocupação de salas e cartaz de programação

## Tecnologias Utilizadas

### Backend

- **.NET 10.0** - Framework principal
- **ASP.NET Core Web API** - Construção da API REST
- **Entity Framework Core** - ORM para acesso ao banco de dados
- **SQL Server** - Banco de dados relacional
- **LINQ** - Consultas e manipulação de dados

### Bibliotecas e Ferramentas

- **RestSharp** - Consumo da API TMDB
- **Swagger/OpenAPI** - Documentação interativa da API
- **DotNetEnv** - Gerenciamento de variáveis de ambiente

### Frontend (Extra)

- **React + Vite** - Interface administrativa
- **Axios** - Cliente HTTP

### Padrão de Camadas

```
Controllers/     → Recebem requisições HTTP e retornam respostas
Services/        → Lógica de negócio e regras de validação
Data/            → Contexto do EF Core e configurações do banco
Models/          → Entidades do domínio
DTOs/            → Objetos de transferência de dados
Middleware/      → Logging e tratamento de requisições
```

## Modelagem do Banco de Dados

### Diagrama Entidade-Relacionamento

```
[]
```

### Tabelas e Relacionamentos

```
[]
```

## ⚙️ Funcionalidades

#### **Agendamento de Sessões**

- Validação de conflitos de horário na mesma sala
- Verificação de interseção de horários (início/fim)
- Validação de existência de filme e sala

#### **Venda de Ingressos**

- Verificação de lotação da sessão
- Validação de assento já ocupado
- Registro de tipo de ingresso (Inteira/Meia)

##### **Filmes em Cartaz**

- Endpoint: `GET /api/sessoes/cartaz?dias=7`
- Retorna filmes com sessões disponíveis nos próximos N dias
- Ordenação por horário de início
- Uso de `.Include()` para trazer dados relacionados

##### **Taxa de Ocupação de Salas**

- Endpoint: `GET /api/salas/{id}/ocupacao?de=&ate=`
- Calcula percentual: `(ingressos vendidos / capacidade total das sessões) * 100`
- Permite filtro por período
- Consultas otimizadas com LINQ

#### **Integração com TMDB (The Movie Database)**

- Importação automática de filmes em cartaz
- Enriquecimento de dados (poster, sinopse, avaliações, gêneros)
- Endpoint: `POST /api/filmes/importar-em-cartaz?quantidadePaginas=1`

#### **Sistema de Assentos Dinâmico**

- Geração automática de assentos disponíveis (formato "Fileira + Número")
- Endpoint: `GET /api/sessoes/{id}/assentos-disponiveis`

#### **Middleware de Logging**

- Logging estruturado de todas as requisições
- Captura de request/response body
- Medição de tempo de resposta

#### **Seed Controller**

- Endpoints para popular banco com dados de exemplo
- Facilita testes e demonstrações

## **Por dentro das Engrenagens**

### 1. **Service Layer com Lógica de Negócio**

**Validação de Conflitos de Horário:**

```csharp
public async Task<bool> HasConflitoAsync(int salaId, DateTime inicio, DateTime fim, int? ignoreSessaoId = null)
{
    var query = _db.Sessoes.AsNoTracking().Where(s => s.SalaId == salaId);

    if (ignoreSessaoId.HasValue)
        query = query.Where(s => s.Id != ignoreSessaoId.Value);

    // Lógica de interseção: novo.inicio < existente.fim && novo.fim > existente.inicio
    return await query.AnyAsync(s => inicio < s.HorarioFim && fim > s.HorarioInicio);
}
```

### 2. **Cálculo de Taxa de Ocupação com LINQ**

```csharp
public async Task<double> GetTaxaOcupacaoSalaAsync(int salaId, DateTime? de = null, DateTime? ate = null)
{
    var sala = await _db.Salas.FindAsync(salaId);
    if (sala == null) throw new KeyNotFoundException("Sala não encontrada.");

    de ??= DateTime.UtcNow.AddDays(-30);
    ate ??= DateTime.UtcNow;

    // Busca sessões no período
    var sessoes = await _db.Sessoes
        .Where(s => s.SalaId == salaId && s.HorarioInicio >= de && s.HorarioInicio <= ate)
        .Select(s => new { s.Id })
        .ToListAsync();

    if (sessoes.Count == 0) return 0;

    // Conta ingressos vendidos
    var sessaoIds = sessoes.Select(s => s.Id).ToList();
    var totalIngressosVendidos = await _db.Ingressos
        .CountAsync(i => sessaoIds.Contains(i.SessaoId));

    // Capacidade total = número de sessões × capacidade da sala
    var capacidadeTotal = sessoes.Count * sala.CapacidadeTotal;

    return (double)totalIngressosVendidos / capacidadeTotal * 100;
}
```

---

## Como Executar o Projeto

### 1. Clonar o Repositório

```bash
git clone [https://github.com/nelcostaa/cineflow]
cd cineflow
```

### 2. Configurar Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto:

```env
# Banco de Dados
DB_CONNECTION_STRING=Server=localhost;Database=CineflowDb;Trusted_Connection=True;TrustServerCertificate=True
MSSQL_SA_PASSWORD=Str0ng!Passw0rd_ChangeMe
MSSQL_DB=CineFlowDb

# API TMDB (obtenha em https://www.themoviedb.org/settings/api)
TMDB_TOKEN=seu_token_aqui
```

### 3. Subir os Containers

Rodar os comandos `docker compose up -d --build`

### 4. Acessar o Swagger

Acesse o Swagger via: `http://localhost/swagger/`

### 5. Acessar o Frontend

Acesse o frontend via: `http://localhost/`

---

## 📁 Estrutura de Pastas

```
cineflow/                          
│
├── CineFlowAPI/                
│   ├── Controllers/               # Endpoints da API (REST Controllers)
│   │   ├── FilmeController.cs
│   │   ├── SalasController.cs
│   │   ├── SessaoController.cs
│   │   ├── IngressosController.cs
│   │   ├── TMBDController.cs
│   │   └── SeedController.cs
│   │
│   ├── Services/                  # Lógica de negócio
│   │   ├── Interfaces/
│   │   │   ├── IFilmeService.cs
│   │   │   ├── ISalaService.cs
│   │   │   ├── ISessaoService.cs
│   │   │   ├── IIngressoService.cs
│   │   │   └── ITmdbService.cs
│   │   ├── FilmeService.cs
│   │   ├── SalaService.cs
│   │   ├── SessaoService.cs
│   │   ├── IngressoService.cs
│   │   └── TmdbService.cs
│   │
│   ├── Models/                    # Entidades do domínio
│   │   ├── Filme.cs
│   │   ├── Sala.cs
│   │   ├── Sessao.cs
│   │   └── Ingresso.cs
│   │
│   ├── DTOs/                      # Data Transfer Objects
│   │   ├── TMDB/
│   │   │   ├── TmdbNowPlayingResponseDto.cs
│   │   │   ├── TmdbMovieListItemDto.cs
│   │   │   └── TmdbGenresResponseDto.cs
│   │   ├── ComprarIngressoDto.cs
│   │   ├── AssentosDisponiveisDto.cs
│   │   ├── CreateFilmeDto.cs
│   │   ├── UpdateFilmeDto.cs
│   │   └── SessoesPorDiaDto.cs
│   │
│   ├── Data/                      # Contexto do EF Core
│   │   ├── AppDbContext.cs
│   │   └── DatabaseSeeder.cs
│   │
│   ├── Middleware/                
│   │   └── RequestLoggingMiddleware.cs
│   │
│   ├── Migrations/                
│   │   ├── 20260211135836_InitialCreate.cs
│   │   ├── 20260211135836_InitialCreate.Designer.cs
│   │   └── AppDbContextModelSnapshot.cs
│   │
│
├── CineflowFront/              # Frontend - React + Vite
│
├── Cineflow.Tests/             # Testes Unitários
│
├── nginx/                      # Proxy Reverso              
│
├── docker-compose.yml          # Orquestração de containers
└── README.md
```
