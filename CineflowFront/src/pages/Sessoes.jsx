import { useState, useEffect } from "react";
import { api } from "../services/api";
import IngressoModal from "../components/IngressoModal";
import "./Sessoes.css";

function Sessoes() {
  const [sessoes, setSessoes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedSessao, setSelectedSessao] = useState(null);

  useEffect(() => {
    loadSessoes();
  }, []);

  const loadSessoes = async () => {
    try {
      setLoading(true);
      const data = await api.getSessoes(7);
      setSessoes(data);
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const groupByDate = (sessoes) => {
    const grouped = {};
    sessoes.forEach((sessao) => {
      const data = new Date(sessao.horarioInicio).toLocaleDateString("pt-BR");
      if (!grouped[data]) {
        grouped[data] = [];
      }
      grouped[data].push(sessao);
    });
    return grouped;
  };

  if (loading) {
    return (
      <div className="content">
        <div className="loading">Carregando sessões...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="content">
        <div className="alert alert-error">Erro: {error}</div>
      </div>
    );
  }

  if (sessoes.length === 0) {
    return (
      <div className="content">
        <h2>Sessões Disponíveis</h2>
        <div className="alert alert-error">
          Nenhuma sessão cadastrada. Use a página Seed para criar sessões.
        </div>
      </div>
    );
  }

  const sessoesPorData = groupByDate(sessoes);

  return (
    <div className="content">
      <h2>Sessões Disponíveis</h2>
      <div className="sessoes-list">
        {Object.entries(sessoesPorData).map(([data, sessoesData]) => (
          <div key={data}>
            <h3 className="sessoes-date">📅 {data}</h3>
            {sessoesData.map((sessao) => (
              <div key={sessao.id} className="sessao-card">
                <div className="sessao-header">
                  <div>
                    <div className="sessao-time">
                      {new Date(sessao.horarioInicio).toLocaleTimeString(
                        "pt-BR",
                        {
                          hour: "2-digit",
                          minute: "2-digit",
                        },
                      )}
                    </div>
                    <h4>{sessao.filme?.titulo || "Filme não encontrado"}</h4>
                  </div>
                  <div className="sessao-info">
                    <span className="badge badge-info">
                      {sessao.sala?.nome || "Sala não encontrada"}
                    </span>
                    <span className="badge badge-success">
                      {sessao.sala?.capacidadeTotal || 0} lugares
                    </span>
                  </div>
                </div>
                <button
                  className="btn btn-primary"
                  onClick={() => setSelectedSessao(sessao)}
                >
                  Comprar Ingresso
                </button>
              </div>
            ))}
          </div>
        ))}
      </div>

      {selectedSessao && (
        <IngressoModal
          sessao={selectedSessao}
          onClose={() => setSelectedSessao(null)}
          onSuccess={() => {
            setSelectedSessao(null);
            loadSessoes();
          }}
        />
      )}
    </div>
  );
}

export default Sessoes;
