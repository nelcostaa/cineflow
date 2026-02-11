import { useState, useEffect } from "react";
import { api } from "../services/api";
import "./Salas.css";

function Salas() {
  const [salas, setSalas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    loadSalas();
  }, []);

  const loadSalas = async () => {
    try {
      setLoading(true);
      const data = await api.getSalas();
      setSalas(data);
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleVerOcupacao = async (salaId) => {
    try {
      const data = await api.getOcupacaoSala(salaId);
      alert(
        `Taxa de ocupação: ${data.taxaOcupacaoPercentual}%\n` +
          `Período: ${new Date(data.periodoInicio).toLocaleDateString("pt-BR")} a ${new Date(data.periodoFim).toLocaleDateString("pt-BR")}`,
      );
    } catch (err) {
      alert("Erro ao carregar ocupação da sala");
    }
  };

  if (loading) {
    return (
      <div className="content">
        <div className="loading">Carregando salas...</div>
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

  return (
    <div className="content">
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
          {salas.map((sala) => (
            <tr key={sala.id}>
              <td>{sala.nome}</td>
              <td>{sala.capacidadeTotal} lugares</td>
              <td>
                <button
                  className="btn btn-primary btn-small"
                  onClick={() => handleVerOcupacao(sala.id)}
                >
                  Ver Ocupação
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default Salas;
