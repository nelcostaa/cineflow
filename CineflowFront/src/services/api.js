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

  importarFilmes: async (quantidadePaginas = 1) => {
    const response = await fetch(
      `${API_URL}/filmes/importar-em-cartaz?quantidadePaginas=${quantidadePaginas}`,
      {
        method: "POST",
      },
    );
    if (!response.ok) throw new Error("Erro ao importar filmes");
    return response.json();
  },

  // Sessões
  getSessoes: async (dias = 7) => {
    const response = await fetch(`${API_URL}/sessoes/por-dia?dias=${dias}`);
    if (!response.ok) throw new Error("Erro ao buscar sessões");
    return response.json();
  },

  getSessoesDia: async (data) => {
    const response = await fetch(`${API_URL}/sessoes/dia/${data}`);
    if (!response.ok) throw new Error("Erro ao buscar sessões do dia");
    return response.json();
  },

  getSessao: async (id) => {
    const response = await fetch(`${API_URL}/sessoes/${id}`);
    if (!response.ok) throw new Error("Erro ao buscar sessão");
    return response.json();
  },

  // Ingressos
  getAssentosDisponiveis: async (sessaoId) => {
    const response = await fetch(
      `${API_URL}/sessoes/${sessaoId}/assentos-disponiveis`,
    );
    if (!response.ok) throw new Error("Erro ao buscar assentos disponíveis");
    return response.json();
  },

  comprarIngresso: async (sessaoId, lugarMarcado, preco) => {
    const response = await fetch(`${API_URL}/sessoes/${sessaoId}/ingressos`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ lugarMarcado, preco, tipoIngresso: "Inteira" }),
    });
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Erro ao comprar ingresso");
    }
    return response.json();
  },

  comprarIngressoCompleto: async (
    sessaoId,
    lugarMarcado,
    preco,
    tipoIngresso,
  ) => {
    const response = await fetch(`${API_URL}/sessoes/${sessaoId}/ingressos`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ lugarMarcado, preco, tipoIngresso }),
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
  criarSalaUnica: async () => {
    const response = await fetch(`${API_URL}/seed/sala-unica`, {
      method: "POST",
    });
    if (!response.ok) throw new Error("Erro ao criar sala");
    return response.json();
  },

  criarSalas: async () => {
    const response = await fetch(`${API_URL}/seed/salas`, {
      method: "POST",
    });
    if (!response.ok) throw new Error("Erro ao criar salas");
    return response.json();
  },

  criarSessaoUnica: async () => {
    const response = await fetch(`${API_URL}/seed/sessao-unica`, {
      method: "POST",
    });
    if (!response.ok) throw new Error("Erro ao criar sessão");
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

  criarIngressoUnico: async () => {
    const response = await fetch(`${API_URL}/seed/ingresso-unico`, {
      method: "POST",
    });
    if (!response.ok) throw new Error("Erro ao criar ingresso");
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
