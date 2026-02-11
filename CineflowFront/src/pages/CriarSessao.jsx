import { useState, useEffect } from "react";
import { api } from "../services/api";
import "./CriarSessao.css";

function CriarSessao() {
  const [filmes, setFilmes] = useState([]);
  const [salas, setSalas] = useState([]);
  const [formData, setFormData] = useState({
    filmeId: "",
    salaId: "",
    data: "",
    horarioInicio: "",
    precoBase: "25.00",
    status: "Ativa",
  });
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState(null);

  useEffect(() => {
    carregarDados();
  }, []);

  const carregarDados = async () => {
    try {
      const [filmesData, salasData] = await Promise.all([
        api.getFilmes(),
        api.getSalas(),
      ]);
      setFilmes(filmesData);
      setSalas(salasData);
    } catch (error) {
      setMessage({
        type: "error",
        text: "Erro ao carregar dados: " + error.message,
      });
    }
  };

  const calcularPrecoBasePorSala = (salaId) => {
    if (!salaId) return "25.00";

    const sala = salas.find((s) => s.id === parseInt(salaId));
    if (!sala) return "25.00";

    const nomeSala = sala.nome.toLowerCase();

    if (nomeSala.includes("premium")) return "45.00";
    if (nomeSala.includes("vip")) return "50.00";
    if (nomeSala.includes("imax")) return "55.00";
    if (nomeSala.includes("3d")) return "40.00";
    if (nomeSala.includes("mini")) return "20.00";
    if (nomeSala.includes("standard") || nomeSala.includes("padrão"))
      return "30.00";

    if (sala.capacidadeTotal < 80) return "20.00";
    if (sala.capacidadeTotal < 150) return "30.00";
    if (sala.capacidadeTotal < 250) return "40.00";
    return "50.00";
  };

  const handleChange = (e) => {
    const { name, value } = e.target;

    if (name === "salaId") {
      const novoPreco = calcularPrecoBasePorSala(value);
      setFormData((prev) => ({
        ...prev,
        [name]: value,
        precoBase: novoPreco,
      }));
    } else {
      setFormData((prev) => ({
        ...prev,
        [name]: value,
      }));
    }
  };

  const calcularHorarioFim = () => {
    if (!formData.filmeId || !formData.data || !formData.horarioInicio) {
      return "";
    }

    const filme = filmes.find((f) => f.id === parseInt(formData.filmeId));
    if (!filme || !filme.duracaoMinutos) return "";

    const [ano, mes, dia] = formData.data.split("-");
    const [hora, minuto] = formData.horarioInicio.split(":");

    const inicio = new Date(ano, mes - 1, dia, hora, minuto);
    const fim = new Date(
      inicio.getTime() + (filme.duracaoMinutos + 30) * 60000,
    );

    return fim.toLocaleTimeString("pt-BR", {
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage(null);

    try {
      if (
        !formData.filmeId ||
        !formData.salaId ||
        !formData.data ||
        !formData.horarioInicio
      ) {
        throw new Error("Preencha todos os campos obrigatórios");
      }

      const filme = filmes.find((f) => f.id === parseInt(formData.filmeId));
      if (!filme) throw new Error("Filme não encontrado");

      const [ano, mes, dia] = formData.data.split("-");
      const [hora, minuto] = formData.horarioInicio.split(":");

      const horarioInicio = new Date(ano, mes - 1, dia, hora, minuto);
      const duracaoMinutos = filme.duracaoMinutos || 120;
      const horarioFim = new Date(
        horarioInicio.getTime() + (duracaoMinutos + 30) * 60000,
      );

      const sessaoData = {
        filmeId: parseInt(formData.filmeId),
        salaId: parseInt(formData.salaId),

        horarioInicio: horarioInicio.toISOString(),
        horarioFim: horarioFim.toISOString(),
        precoBase: parseFloat(formData.precoBase),
        status: formData.status,
      };

      await api.criarSessao(sessaoData);

      setMessage({ type: "success", text: "Sessão criada com sucesso!" });

      // Limpar formulário
      setFormData({
        filmeId: "",
        salaId: "",
        data: "",
        horarioInicio: "",
        precoBase: "25.00",
        status: "Ativa",
      });
    } catch (error) {
      setMessage({ type: "error", text: error.message });
    } finally {
      setLoading(false);
    }
  };

  const filmeSelecionado = filmes.find(
    (f) => f.id === parseInt(formData.filmeId),
  );
  const horarioFim = calcularHorarioFim();

  return (
    <div className="criar-sessao-container">
      <h1>Criar Nova Sessão</h1>

      {message && (
        <div className={`message ${message.type}`}>{message.text}</div>
      )}

      <form onSubmit={handleSubmit} className="criar-sessao-form">
        <div className="form-group">
          <label htmlFor="filmeId">Filme *</label>
          <select
            id="filmeId"
            name="filmeId"
            value={formData.filmeId}
            onChange={handleChange}
            required
          >
            <option value="">Selecione um filme</option>
            {filmes.map((filme) => (
              <option key={filme.id} value={filme.id}>
                {filme.titulo}{" "}
                {filme.duracaoMinutos && `(${filme.duracaoMinutos} min)`}
              </option>
            ))}
          </select>
        </div>

        {filmeSelecionado && (
          <div className="filme-info">
            <p>
              <strong>Gênero:</strong> {filmeSelecionado.genero || "N/A"}
            </p>
            <p>
              <strong>Duração:</strong>{" "}
              {filmeSelecionado.duracaoMinutos || "N/A"} minutos
            </p>
          </div>
        )}

        <div className="form-group">
          <label htmlFor="salaId">Sala *</label>
          <select
            id="salaId"
            name="salaId"
            value={formData.salaId}
            onChange={handleChange}
            required
          >
            <option value="">Selecione uma sala</option>
            {salas.map((sala) => (
              <option key={sala.id} value={sala.id}>
                {sala.nome} (Capacidade: {sala.capacidadeTotal})
              </option>
            ))}
          </select>
          {formData.salaId && (
            <small className="sala-info">
              💡 Preço sugerido baseado no tipo da sala
            </small>
          )}
        </div>

        <div className="form-row">
          <div className="form-group">
            <label htmlFor="data">Data *</label>
            <input
              type="date"
              id="data"
              name="data"
              value={formData.data}
              onChange={handleChange}
              min={new Date().toISOString().split("T")[0]}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="horarioInicio">Horário de Início *</label>
            <input
              type="time"
              id="horarioInicio"
              name="horarioInicio"
              value={formData.horarioInicio}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        {horarioFim && (
          <div className="horario-fim-info">
            <p>
              ⏰ Horário de término: <strong>{horarioFim}</strong>
            </p>
            <small>(Duração do filme + 30min de intervalo)</small>
          </div>
        )}

        <div className="form-row">
          <div className="form-group">
            <label htmlFor="precoBase">Preço Base (R$) *</label>
            <input
              type="number"
              id="precoBase"
              name="precoBase"
              value={formData.precoBase}
              onChange={handleChange}
              step="0.50"
              min="0"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="status">Status</label>
            <select
              id="status"
              name="status"
              value={formData.status}
              onChange={handleChange}
            >
              <option value="Ativa">Ativa</option>
              <option value="Cancelada">Cancelada</option>
              <option value="Encerrada">Encerrada</option>
            </select>
          </div>
        </div>

        <button type="submit" disabled={loading} className="btn-criar">
          {loading ? "Criando..." : "Criar Sessão"}
        </button>
      </form>
    </div>
  );
}

export default CriarSessao;
