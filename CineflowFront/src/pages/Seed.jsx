import { useState } from "react";
import { api } from "../services/api";
import "./Seed.css";

function Seed() {
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleImportarFilmes = async () => {
    try {
      setLoading(true);
      setResult({ type: "loading", message: "Importando filmes..." });
      const data = await api.importarFilmes(2);
      setResult({ type: "success", message: data.message });
    } catch (err) {
      setResult({ type: "error", message: err.message });
    } finally {
      setLoading(false);
    }
  };

  const handleCriarSalas = async () => {
    try {
      setLoading(true);
      setResult({ type: "loading", message: "Criando salas..." });
      const data = await api.criarSalas();
      setResult({ type: "success", message: data.message });
    } catch (err) {
      setResult({ type: "error", message: err.message });
    } finally {
      setLoading(false);
    }
  };

  const handleCriarSessoes = async () => {
    try {
      setLoading(true);
      setResult({ type: "loading", message: "Criando sessões..." });
      const data = await api.criarSessoes(7);
      setResult({ type: "success", message: data.message });
    } catch (err) {
      setResult({ type: "error", message: err.message });
    } finally {
      setLoading(false);
    }
  };

  const handleLimparDados = async () => {
    if (
      !window.confirm(
        "Tem certeza que deseja limpar todos os dados? Esta ação não pode ser desfeita!",
      )
    ) {
      return;
    }

    try {
      setLoading(true);
      setResult({ type: "loading", message: "Limpando dados..." });
      const data = await api.limparDados();
      setResult({ type: "success", message: data.message });
    } catch (err) {
      setResult({ type: "error", message: err.message });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="content">
      <h2>🌱 Popular Banco de Dados</h2>
      <p>Use os botões abaixo para popular o banco com dados de teste:</p>

      <div className="seed-card">
        <h3>1️⃣ Importar Filmes do TMDB</h3>
        <p>Importa filmes que estão em cartaz nos cinemas</p>
        <button
          className="btn btn-primary"
          onClick={handleImportarFilmes}
          disabled={loading}
        >
          Importar Filmes
        </button>
      </div>

      <div className="seed-card">
        <h3>2️⃣ Criar Salas</h3>
        <p>Cria 6 salas com capacidades diferentes</p>
        <button
          className="btn btn-success"
          onClick={handleCriarSalas}
          disabled={loading}
        >
          Criar Salas
        </button>
      </div>

      <div className="seed-card">
        <h3>3️⃣ Criar Sessões</h3>
        <p>Cria sessões para os próximos 7 dias</p>
        <button
          className="btn btn-success"
          onClick={handleCriarSessoes}
          disabled={loading}
        >
          Criar Sessões
        </button>
      </div>

      <div className="seed-card">
        <h3>🧹 Limpar Tudo</h3>
        <p>Remove todos os dados do banco (cuidado!)</p>
        <button
          className="btn btn-danger"
          onClick={handleLimparDados}
          disabled={loading}
        >
          Limpar Banco
        </button>
      </div>

      {result && (
        <div
          className={`alert alert-${result.type === "success" ? "success" : result.type === "error" ? "error" : "info"}`}
        >
          {result.message}
        </div>
      )}
    </div>
  );
}

export default Seed;
