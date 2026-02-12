# 🎬 Cineflow - Sistema de Gerenciamento de Cinema

> Sistema completo para gerenciar cinema com API RESTful, interface web, integração com TMDB e deploy automatizado via Docker.

---

## 📖 Sobre o Projeto

O **Cineflow** é uma aplicação full-stack para gestão completa de cinema, incluindo:

- 🎥 **Catálogo de filmes** sincronizado automaticamente com API do TMDB (The Movie Database)
- 🏛️ **Gestão de salas** com controle de capacidade e disponibilidade
- 📅 **Programação de sessões** com validação de conflitos de horário
- 🎟️ **Sistema de venda de ingressos** com controle de assentos e double-booking prevention
- 📊 **Relatórios** de ocupação de salas e cartaz de programação
- 🚀 **Deploy com um único comando** via Docker Compose

---

## 🛠️ Stack Tecnológica

### Backend
- **ASP.NET Core 10.0** - Framework Web API
- **Entity Framework Core 10.0.2** - ORM com Code-First Migrations
- **SQL Server 2022** - Banco de dados relacional
- **Swagger/OpenAPI** - Documentação interativa da API

### Frontend
- **React 19.2.0** - Biblioteca UI com hooks
- **Vite 7.3.1** - Build tool e dev server
- **React Router DOM 7.13.0** - Roteamento client-side
- **Axios** - Cliente HTTP para consumo da API

### DevOps & Infraestrutura
- **Docker & Docker Compose** - Containerização e orquestração
- **Nginx** - Reverse proxy + servidor estático (multi-stage build)
- **Multi-stage Dockerfiles** - Build otimizado do frontend dentro do container

### Integrações Externas
- **TMDB API v3** - The Movie Database para catálogo de filmes

---

## 🏗️ Arquitetura

### Arquitetura de Containers (3 serviços)

```
┌─────────────────────────────────────────────────────────────┐
│  Nginx (Port 80)                                            │
│  ├─ Serve React SPA (/usr/share/nginx/html)                │
│  ├─ Proxy /api/* → api:8080                                │
│  └─ Proxy /swagger → api:8080/swagger                      │
└─────────────────────────────────────────────────────────────┘
                            │
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  API - CineFlowAPI (Port 8080)                              │
│  ├─ ASP.NET Core Web API                                    │
│  ├─ Entity Framework Core                                   │
│  ├─ Auto-migrations on startup                              │
│  └─ Auto-seed: 10 filmes TMDB + 5 salas                    │
└─────────────────────────────────────────────────────────────┘
                            │
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  Database - SQL Server 2022 (Port 1433)                     │
│  └─ Volume persistente: mssql_data                          │
└─────────────────────────────────────────────────────────────┘
```

### Padrão de Camadas (Backend)

```
Controllers/     → Endpoints REST (validação de requests)
Services/        → Regras de negócio, validações complexas
Data/            → AppDbContext, DatabaseSeeder, migrations
Models/          → Entidades EF Core (Filme, Sala, Sessao, Ingresso)
DTOs/            → Data Transfer Objects para API contracts
Middleware/      → RequestLogging, tratamento global de erros
```

---

## 📊 Modelagem do Banco de Dados

### Diagrama Entidade-Relacionamento

```
┌─────────────┐       ┌─────────────┐       ┌─────────────┐
│   FILMES    │       │    SALAS    │       │  SESSOES    │
├─────────────┤       ├─────────────┤       ├─────────────┤
│ Id (PK)     │───┐   │ Id (PK)     │───┐   │ Id (PK)     │
│ IdTMDB*     │   │   │ Nome        │   │   │ FilmeId (FK)│──┐
│ Titulo      │   └──→│ Capacidade  │   └──→│ SalaId (FK) │  │
│ Genero      │  1:N  └─────────────┘  1:N  │ HorarioIni  │  │
│ DataLanc... │                              │ HorarioFim  │  │
│ Sinopse     │                              │ PrecoBase   │  │
│ PosterPath  │                              │ Status      │  │
│ VoteAverage │                              └─────────────┘  │
│ Popularity  │                                    │ 1        │
│ Atualizado  │                                    │          │
└─────────────┘                                    │ N        │
                                                   ↓          │
                                             ┌─────────────┐  │
                                             │  INGRESSOS  │  │
                                             ├─────────────┤  │
                                             │ Id (PK)     │  │
                                             │ SessaoId(FK)│──┘
                                             │ LugarMarc...│
                                             │ Preco       │
                                             │ DataCompra  │
                                             │ Status      │
                                             └─────────────┘

* UNIQUE INDEX    FK = Foreign Key    PK = Primary Key
```

### Relacionamentos e Constraints

| Relação | Cardinalidade | Delete Behavior | Índices |
|---------|---------------|-----------------|---------|
| **Filmes → Sessoes** | 1:N | **RESTRICT** | `IX_Sessoes_FilmeId` |
| **Salas → Sessoes** | 1:N | **RESTRICT** | `IX_Sessoes_SalaId_HorarioInicio` (composto) |
| **Sessoes → Ingressos** | 1:N | **CASCADE** | `IX_Ingressos_SessaoId_LugarMarcado` (UNIQUE) |

**Índices importantes:**
- `Filmes.IdTMDB` (UNIQUE) - Evita duplicação de filmes do TMDB
- `(Ingressos.SessaoId + LugarMarcado)` (UNIQUE) - **Previne double-booking**

---

## ⚙️ Funcionalidades Principais

### 🎬 Gestão de Filmes
- ✅ Importação automática de filmes "Now Playing" do TMDB
- ✅ Sincronização de metadados (poster, sinopse, avaliações, gêneros)
- ✅ CRUD completo com validações
- ✅ Tracking de última atualização

### 🏛️ Gestão de Salas
- ✅ CRUD de salas com controle de capacidade
- ✅ Validação de existência antes de criar sessões
- ✅ Relatórios de ocupação por período

### 📅 Agendamento de Sessões
- ✅ **Validação de conflitos de horário** na mesma sala
- ✅ Cálculo automático de `HorarioFim` baseado na duração do filme
- ✅ Verificação de interseção de intervalos de tempo
- ✅ Índice composto `(SalaId, HorarioInicio)` para performance

### 🎟️ Sistema de Venda de Ingressos
- ✅ **Prevenção de double-booking** com índice UNIQUE composto
- ✅ Verificação de capacidade da sala
- ✅ Validação de assento já ocupado
- ✅ Geração dinâmica de assentos disponíveis (formato: "A1", "B5")
- ✅ Suporte para tipos de ingresso (Inteira/Meia)

### 📊 Relatórios e Consultas
- ✅ **Cartaz de programação**: Filmes com sessões nos próximos N dias
- ✅ **Taxa de ocupação**: `(ingressos vendidos / capacidade) × 100`
- ✅ **Assentos disponíveis por sessão**: Lista de lugares não ocupados
- ✅ Filtragem por período, sala, filme

### 🔗 Integração TMDB
- ✅ Endpoint: `GET /api/tmbd/now-playing?paginas=1`
- ✅ Endpoint: `GET /api/tmbd/genres` (lista de gêneros)
- ✅ Enriquecimento automático de dados (poster, backdrop, sinopse)
- ✅ Conversão de gêneros (IDs → nomes em português)

### 🛠️ Ferramentas de Desenvolvimento
- ✅ **Seed Controller**: Popula banco com dados de teste
- ✅ **Auto-migrations**: Migrations aplicadas automaticamente no startup
- ✅ **Auto-seed**: 10 filmes TMDB + 5 salas criados na primeira execução
- ✅ **Request Logging Middleware**: Log estruturado de todas as requests
- ✅ **Swagger UI**: Documentação interativa em `/swagger`

---

## 💡 Destaques de Implementação

### 1. Validação de Conflitos de Horário

Evita que duas sessões ocupem a mesma sala em horários sobrepostos:

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

### 2. Cálculo de Taxa de Ocupação com LINQ

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

### 3. Multi-Stage Dockerfile (Nginx + React Build)

Build automático do React dentro do container:

```dockerfile
# Stage 1: Build React com Vite
FROM node:20-alpine AS build
WORKDIR /app
COPY CineflowFront/package*.json ./
RUN npm ci
COPY CineflowFront/ .
RUN npm run build

# Stage 2: Serve com Nginx
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
```

### 4. Auto-Seed na Inicialização

```csharp
// Program.cs
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Aplica migrations automaticamente
    
    var seeder = new DatabaseSeeder(db, tmdbService, logger);
    await seeder.SeedAsync(); // Popula se banco estiver vazio
}
```

---

## 🚀 Como Executar

### Pré-requisitos

- **Docker** e **Docker Compose** instalados
- Token de API do TMDB (gratuito) - [Obter aqui](https://www.themoviedb.org/settings/api)

### Passo 1: Clonar o Repositório

```bash
git clone https://github.com/nelcostaa/cineflow.git
cd cineflow
```

### Passo 2: Configurar Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto:

```env
# SQL Server
MSSQL_SA_PASSWORD=YourStrong!Passw0rd
MSSQL_DB=CineFlowDB

# TMDB API (obtenha em: https://www.themoviedb.org/settings/api)
TMDB_TOKEN=seu_bearer_token_aqui
```

**⚠️ Importante:** O password do SQL Server deve conter:
- Mínimo 8 caracteres
- Letras maiúsculas e minúsculas
- Números
- Caracteres especiais

### Passo 3: Subir a Aplicação (Comando Único!)

```bash
docker-compose up -d --build
```

**O que acontece automaticamente:**
1. ✅ Pull das imagens base (SQL Server, Node, Nginx)
2. ✅ Build do React com Vite (dentro do container nginx)
3. ✅ Build da API .NET
4. ✅ Criação do banco de dados
5. ✅ Aplicação de migrations
6. ✅ Seed automático (10 filmes TMDB + 5 salas)
7. ✅ Inicialização de todos os serviços

**Tempo estimado (primeira execução):** 2-3 minutos

### Passo 4: Acessar a Aplicação

| Serviço | URL | Descrição |
|---------|-----|-----------|
| **Frontend** | http://localhost | Interface React |
| **API** | http://localhost/api | Endpoints REST |
| **Swagger** | http://localhost/swagger | Documentação interativa |
| **SQL Server** | `localhost:1433` | Acesso externo ao banco |

### Passo 5: Verificar Status

```bash
# Ver containers rodando
docker-compose ps

# Ver logs em tempo real
docker-compose logs -f

# Ver logs de um serviço específico
docker-compose logs -f api
docker-compose logs -f nginx
docker-compose logs -f db
```

### Comandos Úteis

```bash
# Parar todos os containers
docker-compose down

# Parar e remover volumes (⚠️ apaga banco de dados)
docker-compose down -v

# Rebuild apenas um serviço
docker-compose up -d --build nginx
docker-compose up -d --build api

# Acessar shell de um container
docker-compose exec api bash
docker-compose exec db bash

# Ver uso de recursos
docker stats
```

---

## 📁 Estrutura do Projeto

```
Cineflow/
├── CineFlowAPI/                      # Backend ASP.NET Core
│   ├── Controllers/                  # Endpoints REST
│   │   ├── FilmesController.cs       # CRUD de filmes
│   │   ├── SalasController.cs        # CRUD de salas
│   │   ├── SessaoController.cs       # Gestão de sessões
│   │   ├── IngressosController.cs    # Venda de ingressos
│   │   ├── TMBDController.cs         # Integração TMDB
│   │   └── SeedController.cs         # Seed manual de dados
│   │
│   ├── Services/                     # Lógica de negócio
│   │   ├── Interfaces/               # Contratos dos serviços
│   │   │   ├── IFilmeService.cs
│   │   │   ├── ISalaService.cs
│   │   │   ├── ISessaoService.cs
│   │   │   ├── IIngressoService.cs
│   │   │   └── ITmdbService.cs
│   │   ├── FilmeService.cs           # Validações e CRUD
│   │   ├── SalaService.cs            # Gestão de salas
│   │   ├── SessaoService.cs          # Validação de conflitos
│   │   ├── IngressoService.cs        # Double-booking prevention
│   │   └── TmdbService.cs            # Consumo API TMDB
│   │
│   ├── Models/                       # Entidades EF Core
│   │   ├── Filme.cs                  # Catálogo de filmes
│   │   ├── Sala.cs                   # Salas do cinema
│   │   ├── Sessao.cs                 # Horários de exibição
│   │   └── Ingresso.cs               # Tickets vendidos
│   │
│   ├── DTOs/                         # Data Transfer Objects
│   │   ├── TMDB/                     # DTOs de integração TMDB
│   │   │   ├── TmdbNowPlayingResponseDto.cs
│   │   │   ├── TmdbMovieListItemDto.cs
│   │   │   └── TmdbGenresResponseDto.cs
│   │   ├── ComprarIngressoDto.cs
│   │   ├── AssentosDisponiveisDto.cs
│   │   ├── CreateFilmeDto.cs
│   │   ├── UpdateFilmeDto.cs
│   │   └── SessoesPorDiaDto.cs
│   │
│   ├── Data/                         # Camada de dados
│   │   ├── AppDbContext.cs           # Contexto EF Core
│   │   └── DatabaseSeeder.cs         # Seed automático
│   │
│   ├── Middleware/                   
│   │   └── RequestLoggingMiddleware.cs  # Log estruturado
│   │
│   ├── Migrations/                   # Code-First Migrations
│   │   ├── 20260211135836_InitialCreate.cs
│   │   └── AppDbContextModelSnapshot.cs
│   │
│   ├── Program.cs                    # Entry point + DI setup
│   ├── appsettings.json              # Configurações base
│   ├── appsettings.Development.json  # Config dev
│   └── Dockerfile                    # Build da API
│
├── CineflowFront/                    # Frontend React
│   ├── src/
│   │   ├── components/               # Componentes reutilizáveis
│   │   │   ├── Navbar.jsx/css
│   │   │   ├── FilmeModal.jsx/css
│   │   │   └── IngressoModal.jsx/css
│   │   ├── pages/                    # Páginas da aplicação
│   │   │   ├── Home.jsx              # Dashboard
│   │   │   ├── Filmes.jsx            # Catálogo
│   │   │   ├── Salas.jsx             # Gestão de salas
│   │   │   ├── Sessoes.jsx           # Programação
│   │   │   ├── CriarSessao.jsx       # Agendar sessão
│   │   │   └── Seed.jsx              # Ferramentas dev
│   │   ├── services/
│   │   │   └── api.js                # Cliente HTTP centralizado
│   │   ├── App.jsx                   # Router + layout
│   │   └── main.jsx                  # Entry point
│   │
│   ├── public/                       # Assets estáticos
│   ├── .env.development              # VITE_API_URL dev
│   ├── .env.production               # VITE_API_URL prod
│   ├── Dockerfile                    # Multi-stage build (Node → Nginx)
│   ├── package.json
│   └── vite.config.js
│
├── nginx/                            # Configuração do proxy
│   └── nginx.conf                    # Serve SPA + proxy API/Swagger
│
├── Cineflow.Tests/                   # Testes unitários
│   └── UnitTest1.cs
│
├── .github/
│   └── copilot-instructions.md       # Guidelines do projeto
│
├── docker-compose.yml                # Orquestração 3 containers
├── .env                              # Variáveis de ambiente (não comitar!)
├── .gitignore
└── README.md
```

---

## 📡 Principais Endpoints da API

### Filmes

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/filmes` | Lista todos os filmes |
| `GET` | `/api/filmes/{id}` | Busca filme por ID |
| `POST` | `/api/filmes` | Cria novo filme |
| `PUT` | `/api/filmes/{id}` | Atualiza filme |
| `DELETE` | `/api/filmes/{id}` | Remove filme |

### Sessões

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/sessao` | Lista todas as sessões |
| `GET` | `/api/sessao/{id}` | Detalhes de uma sessão |
| `GET` | `/api/sessao/filme/{filmeId}` | Sessões de um filme específico |
| `POST` | `/api/sessao` | Cria nova sessão (valida conflitos) |
| `PUT` | `/api/sessao/{id}` | Atualiza sessão |
| `DELETE` | `/api/sessao/{id}` | Cancela sessão |

### Ingressos

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/api/ingressos/comprar` | Compra ingresso (valida disponibilidade) |
| `GET` | `/api/ingressos/disponiveis/{sessaoId}` | Lista assentos disponíveis |

### Salas

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/salas` | Lista todas as salas |
| `POST` | `/api/salas` | Cria nova sala |
| `GET` | `/api/salas/{id}/ocupacao` | Taxa de ocupação da sala |

### TMDB

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/tmbd/now-playing?paginas=1` | Filmes "Now Playing" do TMDB |
| `GET` | `/api/tmbd/genres` | Lista de gêneros do TMDB |

### Seed (Dev)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/api/seed/sala-unica` | Cria sala de teste |
| `POST` | `/api/seed/salas` | Cria 5 salas de teste |
| `POST` | `/api/seed/filmes-tmdb` | Importa 10 filmes do TMDB |
| `DELETE` | `/api/seed/limpar` | ⚠️ **PERIGO** - Apaga todos os dados |

---

## 🧪 Testes

```bash
# Rodar testes unitários
cd Cineflow.Tests
dotnet test
```

---

## 🔧 Troubleshooting

### Erro: "Login failed for user 'sa'"
- Verifique se o password no `.env` atende os requisitos do SQL Server
- Aguarde 10-15 segundos após o `docker-compose up` (SQL Server demora a inicializar)

### Erro: "Port 80 already in use"
- Algum serviço está usando a porta 80 (Apache, outro nginx, etc)
- Mude a porta no `docker-compose.yml`: `"8080:80"`

### Frontend não carrega
- Verifique se o build do React foi feito: `docker-compose logs nginx | grep "npm run build"`
- Reconstrua o nginx: `docker-compose up -d --build nginx`

### Migrations não aplicadas
- Entre no container da API: `docker-compose exec api bash`
- Aplique manualmente: `dotnet ef database update`

---

## 📝 Variáveis de Ambiente

### Backend (`.env`)

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `MSSQL_SA_PASSWORD` | Password do SQL Server (SA) | `YourStrong!Passw0rd` |
| `MSSQL_DB` | Nome do banco de dados | `CineFlowDB` |
| `TMDB_TOKEN` | Bearer token da API TMDB | `eyJhbGciOiJIUzI1...` |

### Frontend (`CineflowFront/.env.development`)

| Variável | Descrição | Valor Padrão |
|----------|-----------|--------------|
| `VITE_API_URL` | Base URL da API | `http://localhost/api` |

**⚠️ Importante:** Variáveis `VITE_*` são injetadas no **build time**, não em runtime. Se mudar, precisa rebuildar o frontend.

---

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Add: MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto foi desenvolvido como trabalho acadêmico e está disponível para fins educacionais.

---

## 👨‍💻 Autor

**Nelson Costa**
- GitHub: [@nelcostaa](https://github.com/nelcostaa)
- LinkedIn: [Nelson Costa](https://linkedin.com/in/nelson-costa)

---

## 🙏 Agradecimentos

- [The Movie Database (TMDB)](https://www.themoviedb.org/) pela API gratuita
- Comunidade ASP.NET Core e React
- Documentação oficial do Docker e Entity Framework Core
