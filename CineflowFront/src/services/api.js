const API_URL = "http://localhost/api";

export const api = {
  // Filmes
  getFilmes: async () => {
    const response = await fetch(`${API_URL}/filmes`);
    if (!response.ok) throw new Error("Erro ao buscar filmes");
    return response.json();
  },

  getFilme: async (id) => {
    const response = await fetch(`${API_URL}/filmes/${id}`);
    if (!response.ok) throw new Error("Erro ao buscar filme");
    return response.json();
  },

  importarFilmes: async (quantidadePaginas = 2) => {
    const response = await fetch(
      `${API_URL}/filmes/importar-now-playing?quantidadePaginas=${quantidadePaginas}`,
      {
        method: "POST",
      },
    );
    if (!response.ok) throw new Error("Erro ao importar filmes");
    return response.json();
  },

  // Sessões
  getSessoes: async (dias = 7) => {
    const response = await fetch(`${API_URL}/sessoes/cartaz?dias=${dias}`);
    if (!response.ok) throw new Error("Erro ao buscar sessões");
    return response.json();
  },

  getSessao: async (id) => {
    const response = await fetch(`${API_URL}/sessoes/${id}`);
    if (!response.ok) throw new Error("Erro ao buscar sessão");
    return response.json();
  },

  // Ingressos
  comprarIngresso: async (sessaoId, lugarMarcado, preco) => {
    const response = await fetch(`${API_URL}/sessoes/${sessaoId}/ingressos`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ lugarMarcado, preco }),
    });
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Erro ao comprar ingresso");
    }
    return response.json();
  },

  // Salas
  getSalas: async () => {
    const response = await fetch(`${API_URL}/salas`);
    if (!response.ok) throw new Error("Erro ao buscar salas");
    return response.json();
  },

  getOcupacaoSala: async (salaId) => {
    const response = await fetch(`${API_URL}/salas/${salaId}/ocupacao`);
    if (!response.ok) throw new Error("Erro ao buscar ocupação");
    return response.json();
  },

  // Seed
  criarSalas: async () => {
    const response = await fetch(`${API_URL}/seed/salas`, {
      method: "POST",
    });
    if (!response.ok) throw new Error("Erro ao criar salas");
    return response.json();
  },

  criarSessoes: async (diasFuturos = 7) => {
    const response = await fetch(
      `${API_URL}/seed/sessoes?diasFuturos=${diasFuturos}`,
      {
        method: "POST",
      },
    );
    if (!response.ok) throw new Error("Erro ao criar sessões");
    return response.json();
  },

  limparDados: async () => {
    const response = await fetch(`${API_URL}/seed/limpar`, {
      method: "DELETE",
    });
    if (!response.ok) throw new Error("Erro ao limpar dados");
    return response.json();
  },
};
