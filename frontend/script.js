const API_URL = "http://localhost/api";

// Navegação
document.querySelectorAll(".nav-menu a").forEach((link) => {
  link.addEventListener("click", (e) => {
    e.preventDefault();
    const page = e.target.dataset.page;
    loadPage(page);
  });
});

// Carregar página inicial
loadPage("home");

async function loadPage(page) {
  const content = document.getElementById("content");

  switch (page) {
    case "home":
      renderHome(content);
      break;
    case "filmes":
      await renderFilmes(content);
      break;
    case "sessoes":
      await renderSessoes(content);
      break;
    case "salas":
      await renderSalas(content);
      break;
    case "seed":
      renderSeed(content);
      break;
  }
}

function renderHome(content) {
  content.innerHTML = `
        <div class="hero">
            <h2>🎬 Bem-vindo ao CineFlow</h2>
            <p>Sistema completo de gerenciamento de cinema</p>
            <div class="btn-group" style="justify-content: center;">
                <button class="btn btn-primary" onclick="loadPage('filmes')">Ver Filmes</button>
                <button class="btn btn-success" onclick="loadPage('sessoes')">Ver Sessões</button>
            </div>
        </div>
        <div class="cards-grid">
            <div class="card">
                <div class="card-content">
                    <h3>📽️ Filmes em Cartaz</h3>
                    <p class="card-info">Veja todos os filmes disponíveis no cinema</p>
                </div>
            </div>
            <div class="card">
                <div class="card-content">
                    <h3>🎫 Sessões</h3>
                    <p class="card-info">Confira os horários das sessões</p>
                </div>
            </div>
            <div class="card">
                <div class="card-content">
                    <h3>🏢 Salas</h3>
                    <p class="card-info">Gerencie as salas do cinema</p>
                </div>
            </div>
            <div class="card">
                <div class="card-content">
                    <h3>🌱 Seed</h3>
                    <p class="card-info">Popular banco com dados de teste</p>
                </div>
            </div>
        </div>
    `;
}

async function renderFilmes(content) {
  content.innerHTML = '<div class="loading">Carregando filmes...</div>';

  try {
    const response = await fetch(`${API_URL}/filmes`);
    const filmes = await response.json();

    if (filmes.length === 0) {
      content.innerHTML = `
                <div class="alert alert-error">
                    <p>Nenhum filme cadastrado. Use a página Seed para importar filmes.</p>
                </div>
            `;
      return;
    }

    content.innerHTML = `
            <h2>Filmes em Cartaz</h2>
            <div class="cards-grid">
                ${filmes
                  .map(
                    (filme) => `
                    <div class="card" onclick="showFilmeDetails(${filme.id})">
                        ${
                          filme.posterPath
                            ? `<img src="https://image.tmdb.org/t/p/w500${filme.posterPath}" alt="${filme.titulo}" class="card-image">`
                            : `<div class="card-image"></div>`
                        }
                        <div class="card-content">
                            <h3 class="card-title">${filme.titulo}</h3>
                            <p class="card-info">⭐ ${filme.voteAverage?.toFixed(1) || "N/A"} | 📅 ${filme.dataLancamento ? new Date(filme.dataLancamento).toLocaleDateString("pt-BR") : "N/A"}</p>
                            <p class="card-info">${filme.genero || "Sem gênero"}</p>
                        </div>
                    </div>
                `,
                  )
                  .join("")}
            </div>
        `;
  } catch (error) {
    content.innerHTML = `<div class="alert alert-error">Erro ao carregar filmes: ${error.message}</div>`;
  }
}

async function showFilmeDetails(id) {
  const modal = document.createElement("div");
  modal.className = "modal active";

  try {
    const response = await fetch(`${API_URL}/filmes/${id}`);
    const filme = await response.json();

    modal.innerHTML = `
            <div class="modal-content">
                <div class="modal-header">
                    <h2>${filme.titulo}</h2>
                    <button class="modal-close" onclick="this.closest('.modal').remove()">&times;</button>
                </div>
                ${
                  filme.backdropPath
                    ? `<img src="https://image.tmdb.org/t/p/w780${filme.backdropPath}" alt="${filme.titulo}" style="width: 100%; border-radius: 10px; margin-bottom: 1rem;">`
                    : ""
                }
                <p><strong>Título Original:</strong> ${filme.tituloOriginal || "N/A"}</p>
                <p><strong>Gênero:</strong> ${filme.genero || "N/A"}</p>
                <p><strong>Lançamento:</strong> ${filme.dataLancamento ? new Date(filme.dataLancamento).toLocaleDateString("pt-BR") : "N/A"}</p>
                <p><strong>Duração:</strong> ${filme.duracaoMinutos || "N/A"} minutos</p>
                <p><strong>Avaliação:</strong> ⭐ ${filme.voteAverage?.toFixed(1) || "N/A"} (${filme.voteCount || 0} votos)</p>
                <p><strong>Sinopse:</strong></p>
                <p>${filme.sinopse || "Sem sinopse disponível"}</p>
                <div class="btn-group">
                    <button class="btn btn-primary" onclick="verSessoesFilme(${filme.id})">Ver Sessões</button>
                    <button class="btn btn-danger" onclick="this.closest('.modal').remove()">Fechar</button>
                </div>
            </div>
        `;

    document.body.appendChild(modal);
  } catch (error) {
    alert("Erro ao carregar detalhes do filme");
  }
}

async function renderSessoes(content) {
  content.innerHTML = '<div class="loading">Carregando sessões...</div>';

  try {
    const response = await fetch(`${API_URL}/sessoes/cartaz?dias=7`);
    const sessoes = await response.json();

    if (sessoes.length === 0) {
      content.innerHTML = `
                <div class="alert alert-error">
                    <p>Nenhuma sessão cadastrada. Use a página Seed para criar sessões.</p>
                </div>
            `;
      return;
    }

    // Agrupar por data
    const sessoesPorData = {};
    sessoes.forEach((sessao) => {
      const data = new Date(sessao.horarioInicio).toLocaleDateString("pt-BR");
      if (!sessoesPorData[data]) {
        sessoesPorData[data] = [];
      }
      sessoesPorData[data].push(sessao);
    });

    content.innerHTML = `
            <h2>Sessões Disponíveis</h2>
            <div class="sessoes-list">
                ${Object.entries(sessoesPorData)
                  .map(
                    ([data, sessoesData]) => `
                    <h3 style="margin-top: 2rem; color: #667eea;">📅 ${data}</h3>
                    ${sessoesData
                      .map(
                        (sessao) => `
                        <div class="sessao-card">
                            <div class="sessao-header">
                                <div>
                                    <div class="sessao-time">${new Date(sessao.horarioInicio).toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" })}</div>
                                    <h4>${sessao.filme?.titulo || "Filme não encontrado"}</h4>
                                </div>
                                <div style="text-align: right;">
                                    <div class="badge badge-info">${sessao.sala?.nome || "Sala não encontrada"}</div>
                                    <div style="margin-top: 0.5rem;">
                                        <span class="badge badge-success">${sessao.sala?.capacidadeTotal || 0} lugares</span>
                                    </div>
                                </div>
                            </div>
                            <button class="btn btn-primary" onclick="comprarIngresso(${sessao.id})">Comprar Ingresso</button>
                        </div>
                    `,
                      )
                      .join("")}
                `,
                  )
                  .join("")}
            </div>
        `;
  } catch (error) {
    content.innerHTML = `<div class="alert alert-error">Erro ao carregar sessões: ${error.message}</div>`;
  }
}

async function comprarIngresso(sessaoId) {
  const modal = document.createElement("div");
  modal.className = "modal active";

  modal.innerHTML = `
        <div class="modal-content">
            <div class="modal-header">
                <h2>Comprar Ingresso</h2>
                <button class="modal-close" onclick="this.closest('.modal').remove()">&times;</button>
            </div>
            <form onsubmit="confirmarCompra(event, ${sessaoId})">
                <div class="form-group">
                    <label>Assento (ex: A1, B5):</label>
                    <input type="text" name="lugarMarcado" required placeholder="Ex: A1">
                </div>
                <div class="form-group">
                    <label>Preço (R$):</label>
                    <input type="number" name="preco" step="0.01" required placeholder="Ex: 25.00">
                </div>
                <div class="btn-group">
                    <button type="submit" class="btn btn-success">Confirmar Compra</button>
                    <button type="button" class="btn btn-danger" onclick="this.closest('.modal').remove()">Cancelar</button>
                </div>
            </form>
        </div>
    `;

  document.body.appendChild(modal);
}

async function confirmarCompra(event, sessaoId) {
  event.preventDefault();
  const form = event.target;
  const lugarMarcado = form.lugarMarcado.value;
  const preco = parseFloat(form.preco.value);

  try {
    const response = await fetch(`${API_URL}/sessoes/${sessaoId}/ingressos`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ lugarMarcado, preco }),
    });

    if (response.ok) {
      alert("Ingresso comprado com sucesso!");
      form.closest(".modal").remove();
    } else {
      const error = await response.json();
      alert(`Erro: ${error.message || "Não foi possível comprar o ingresso"}`);
    }
  } catch (error) {
    alert(`Erro ao comprar ingresso: ${error.message}`);
  }
}

async function renderSalas(content) {
  content.innerHTML = '<div class="loading">Carregando salas...</div>';

  try {
    const response = await fetch(`${API_URL}/salas`);
    const salas = await response.json();

    content.innerHTML = `
            <h2>Salas do Cinema</h2>
            <table>
                <thead>
                    <tr>
                        <th>Nome</th>
                        <th>Capacidade</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    ${salas
                      .map(
                        (sala) => `
                        <tr>
                            <td>${sala.nome}</td>
                            <td>${sala.capacidadeTotal} lugares</td>
                            <td>
                                <button class="btn btn-primary" onclick="verOcupacaoSala(${sala.id})">Ver Ocupação</button>
                            </td>
                        </tr>
                    `,
                      )
                      .join("")}
                </tbody>
            </table>
        `;
  } catch (error) {
    content.innerHTML = `<div class="alert alert-error">Erro ao carregar salas: ${error.message}</div>`;
  }
}

async function verOcupacaoSala(salaId) {
  try {
    const response = await fetch(`${API_URL}/salas/${salaId}/ocupacao`);
    const data = await response.json();

    alert(
      `Taxa de ocupação: ${data.taxaOcupacaoPercentual}%\nPeríodo: ${new Date(data.periodoInicio).toLocaleDateString("pt-BR")} a ${new Date(data.periodoFim).toLocaleDateString("pt-BR")}`,
    );
  } catch (error) {
    alert("Erro ao carregar ocupação da sala");
  }
}

function renderSeed(content) {
  content.innerHTML = `
        <h2>🌱 Popular Banco de Dados</h2>
        <p>Use os botões abaixo para popular o banco com dados de teste:</p>
        
        <div class="sessao-card">
            <h3>1️⃣ Importar Filmes do TMDB</h3>
            <p>Importa filmes que estão em cartaz nos cinemas</p>
            <button class="btn btn-primary" onclick="importarFilmes()">Importar Filmes</button>
        </div>
        
        <div class="sessao-card">
            <h3>2️⃣ Criar Salas</h3>
            <p>Cria 6 salas com capacidades diferentes</p>
            <button class="btn btn-success" onclick="criarSalas()">Criar Salas</button>
        </div>
        
        <div class="sessao-card">
            <h3>3️⃣ Criar Sessões</h3>
            <p>Cria sessões para os próximos 7 dias</p>
            <button class="btn btn-success" onclick="criarSessoes()">Criar Sessões</button>
        </div>
        
        <div class="sessao-card">
            <h3>🧹 Limpar Tudo</h3>
            <p>Remove todos os dados do banco (cuidado!)</p>
            <button class="btn btn-danger" onclick="limparDados()">Limpar Banco</button>
        </div>
        
        <div id="seed-result"></div>
    `;
}

async function importarFilmes() {
  const resultDiv = document.getElementById("seed-result");
  resultDiv.innerHTML = '<div class="loading">Importando filmes...</div>';

  try {
    const response = await fetch(
      `${API_URL}/filmes/importar-now-playing?quantidadePaginas=2`,
      {
        method: "POST",
      },
    );
    const data = await response.json();

    resultDiv.innerHTML = `<div class="alert alert-success">${data.message}</div>`;
  } catch (error) {
    resultDiv.innerHTML = `<div class="alert alert-error">Erro: ${error.message}</div>`;
  }
}

async function criarSalas() {
  const resultDiv = document.getElementById("seed-result");
  resultDiv.innerHTML = '<div class="loading">Criando salas...</div>';

  try {
    const response = await fetch(`${API_URL}/seed/salas`, {
      method: "POST",
    });
    const data = await response.json();

    resultDiv.innerHTML = `<div class="alert alert-success">${data.message}</div>`;
  } catch (error) {
    resultDiv.innerHTML = `<div class="alert alert-error">Erro: ${error.message}</div>`;
  }
}

async function criarSessoes() {
  const resultDiv = document.getElementById("seed-result");
  resultDiv.innerHTML = '<div class="loading">Criando sessões...</div>';

  try {
    const response = await fetch(`${API_URL}/seed/sessoes?diasFuturos=7`, {
      method: "POST",
    });
    const data = await response.json();

    resultDiv.innerHTML = `<div class="alert alert-success">${data.message}</div>`;
  } catch (error) {
    resultDiv.innerHTML = `<div class="alert alert-error">Erro: ${error.message}</div>`;
  }
}

async function limparDados() {
  if (
    !confirm(
      "Tem certeza que deseja limpar todos os dados? Esta ação não pode ser desfeita!",
    )
  ) {
    return;
  }

  const resultDiv = document.getElementById("seed-result");
  resultDiv.innerHTML = '<div class="loading">Limpando dados...</div>';

  try {
    const response = await fetch(`${API_URL}/seed/limpar`, {
      method: "DELETE",
    });
    const data = await response.json();

    resultDiv.innerHTML = `<div class="alert alert-success">${data.message}</div>`;
  } catch (error) {
    resultDiv.innerHTML = `<div class="alert alert-error">Erro: ${error.message}</div>`;
  }
}
