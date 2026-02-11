import { useState, useEffect } from "react";
import { api } from "../services/api";
import IngressoModal from "../components/IngressoModal";
import "./Sessoes.css";

function Sessoes() {
  const [sessoesPorDia, setSessoesPorDia] = useState([]);
  const [diaAtual, setDiaAtual] = useState(0);
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
      setSessoesPorDia(data);
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const formatarData = (data) => {
    const [ano, mes, dia] = data.split("-");
    return `${dia}/${mes}/${ano}`;
  };

  const proximoDia = () => {
    if (diaAtual < sessoesPorDia.length - 1) {
      setDiaAtual(diaAtual + 1);
    }
  };

  const diaAnterior = () => {
    if (diaAtual > 0) {
      setDiaAtual(diaAtual - 1);
    }
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

  if (sessoesPorDia.length === 0) {
    return (
      <div className="content">
        <h2>Sessões Disponíveis</h2>
        <div className="alert alert-error">
          Nenhuma sessão cadastrada. Use a página Seed para criar sessões.
        </div>
      </div>
    );
  }

  const dia = sessoesPorDia[diaAtual];

  return (
    <div className="content">
      <div className="sessoes-header">
        <button
          className="btn btn-nav"
          onClick={diaAnterior}
          disabled={diaAtual === 0}
        >
          ← Anterior
        </button>
        <h2 className="sessoes-date-title">
          📅 {formatarData(dia.data)} -{" "}
          {dia.diaSemana.charAt(0).toUpperCase() + dia.diaSemana.slice(1)}
        </h2>
        <button
          className="btn btn-nav"
          onClick={proximoDia}
          disabled={diaAtual === sessoesPorDia.length - 1}
        >
          Próximo →
        </button>
      </div>

      <div className="sessoes-grid">
        {dia.sessoes.map((sessao) => (
          <div key={sessao.id} className="sessao-card">
            <div className="sessao-time">
              {sessao.horarioInicio} - {sessao.horarioFim}
            </div>
            <h4>{sessao.filmeTitulo}</h4>

            <div className="sessao-badges">
              {sessao.filmeClassificacao && (
                <span className="badge badge-warning">
                  {sessao.filmeClassificacao}
                </span>
              )}
              <span className="badge badge-info">{sessao.salaNome}</span>
            </div>

            {sessao.filmeDuracao && (
              <div className="sessao-duracao">⏱️ {sessao.filmeDuracao} min</div>
            )}

            <div className="sessao-price">R$ {sessao.precoBase.toFixed(2)}</div>

            <div className="sessao-occupancy">
              <div className="progress-bar">
                <div
                  className="progress-fill"
                  style={{
                    width: `${(sessao.ingressosVendidos / sessao.salaCapacidade) * 100}%`,
                  }}
                ></div>
              </div>
              <span className="occupancy-text">
                {sessao.ingressosVendidos}/{sessao.salaCapacidade} lugares
              </span>
            </div>

            <button
              className="btn btn-primary btn-full"
              onClick={() => setSelectedSessao(sessao)}
              disabled={
                sessao.lugaresDisponiveis === 0 || sessao.status !== "Ativa"
              }
            >
              {sessao.lugaresDisponiveis === 0
                ? "Esgotado"
                : sessao.status !== "Ativa"
                  ? "Indisponível"
                  : "Comprar Ingresso"}
            </button>
          </div>
        ))}
      </div>

      {selectedSessao && (
        <IngressoModal
          sessao={{
            id: selectedSessao.id,
            filme: { titulo: selectedSessao.filmeTitulo },
            sala: { nome: selectedSessao.salaNome },
            horarioInicio: `${dia.data}T${selectedSessao.horarioInicio}:00`,
          }}
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
