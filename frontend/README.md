# Frontend CineFlow

Frontend simples para visualização e interação com o sistema CineFlow.

## Como usar

1. Certifique-se de que a API está rodando em `http://localhost/api`
2. Abra o arquivo `index.html` diretamente no navegador

## Funcionalidades

### 🏠 Início

- Página inicial com resumo das funcionalidades

### 📽️ Filmes

- Listagem de todos os filmes em cartaz
- Visualização de detalhes completos do filme
- Imagens dos pôsteres (quando disponíveis)

### 🎫 Sessões

- Listagem de sessões organizadas por data
- Horários e informações da sala
- Compra de ingressos

### 🏢 Salas

- Listagem de todas as salas
- Visualização da taxa de ocupação

### 🌱 Seed

- Importar filmes do TMDB
- Criar salas de teste
- Criar sessões automáticas
- Limpar banco de dados

## Estrutura

```
frontend/
├── index.html   # Página principal
├── style.css    # Estilos
├── script.js    # Lógica JavaScript
└── README.md    # Este arquivo
```

## Requisitos

- Navegador moderno com suporte a ES6+
- API CineFlow rodando (docker-compose up)
- CORS habilitado na API (já configurado)
