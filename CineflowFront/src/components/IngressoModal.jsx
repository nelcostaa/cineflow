import { useState, useEffect } from "react";
import { api } from "../services/api";
import "./IngressoModal.css";

function IngressoModal({ sessao, onClose, onSuccess }) {
  const [assentosInfo, setAssentosInfo] = useState(null);
  const [assentoSelecionado, setAssentoSelecionado] = useState("");
  const [tipoIngresso, setTipoIngresso] = useState("Inteira");
  const [loading, setLoading] = useState(true);
  const [comprando, setComprando] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    loadAssentosDisponiveis();
  }, [sessao.id]);

  const loadAssentosDisponiveis = async () => {
    try {
      setLoading(true);
      const data = await api.getAssentosDisponiveis(sessao.id);
      setAssentosInfo(data);
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const gerarAssentos = () => {
    const capacidade = assentosInfo?.capacidadeTotal || 80;

    // Calcular layout ideal (aproximadamente quadrado)
    const assentosPorFileira = Math.ceil(Math.sqrt(capacidade * 1.5)); // Largura maior que altura
    const numFileiras = Math.ceil(capacidade / assentosPorFileira);

    const fileiras = [];
    for (let i = 0; i < numFileiras; i++) {
      fileiras.push(String.fromCharCode(65 + i)); // A, B, C, D...
    }

    const assentos = [];
    let assentosCriados = 0;

    for (const fileira of fileiras) {
      for (let num = 1; num <= assentosPorFileira; num++) {
        if (assentosCriados >= capacidade) break;
        assentos.push(`${fileira}${num}`);
        assentosCriados++;
      }
      if (assentosCriados >= capacidade) break;
    }

    return assentos;
  };

  const calcularColunasGrid = () => {
    const capacidade = assentosInfo?.capacidadeTotal || 80;
    return Math.ceil(Math.sqrt(capacidade * 1.5));
  };

  const handleAssentoClick = (assento) => {
    if (!assentosInfo.assentosOcupados.includes(assento)) {
      setAssentoSelecionado(assento);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!assentoSelecionado) {
      setError("Por favor, selecione um assento.");
      return;
    }

    const preco =
      tipoIngresso === "Inteira"
        ? assentosInfo.precoInteira
        : assentosInfo.precoMeia;

    try {
      setComprando(true);
      setError(null);
      await api.comprarIngressoCompleto(
        sessao.id,
        assentoSelecionado,
        preco,
        tipoIngresso,
      );
      alert(
        `✅ Ingresso comprado com sucesso!\n\nAssento: ${assentoSelecionado}\nTipo: ${tipoIngresso}\nValor: R$ ${preco.toFixed(2)}`,
      );
      onSuccess();
    } catch (err) {
      setError(err.message);
    } finally {
      setComprando(false);
    }
  };

  const handleBackdropClick = (e) => {
    if (e.target.className === "modal active") {
      onClose();
    }
  };

  if (loading) {
    return (
      <div className="modal active" onClick={handleBackdropClick}>
        <div className="modal-content">
          <div className="loading">Carregando assentos...</div>
        </div>
      </div>
    );
  }

  const precoAtual =
    tipoIngresso === "Inteira"
      ? assentosInfo?.precoInteira
      : assentosInfo?.precoMeia;

  return (
    <div className="modal active" onClick={handleBackdropClick}>
      <div className="modal-content modal-large">
        <div className="modal-header">
          <h2>🎬 Comprar Ingresso</h2>
          <button className="modal-close" onClick={onClose}>
            &times;
          </button>
        </div>

        <div className="modal-body">
          <div className="sessao-info-box">
            <p>
              <strong>🎥 Filme:</strong> {sessao.filme?.titulo}
            </p>
            <p>
              <strong>🚪 Sala:</strong> {sessao.sala?.nome}
            </p>
            <p>
              <strong>🕐 Horário:</strong>{" "}
              {new Date(sessao.horarioInicio).toLocaleString("pt-BR")}
            </p>
            <p>
              <strong>💺 Disponíveis:</strong>{" "}
              {assentosInfo?.lugaresDisponiveis} de{" "}
              {assentosInfo?.capacidadeTotal}
            </p>
          </div>

          {error && <div className="alert alert-error">{error}</div>}

          <form onSubmit={handleSubmit}>
            {/* Tipo de Ingresso */}
            <div className="form-section">
              <h3>1. Tipo de Ingresso</h3>
              <div className="tipo-ingresso-options">
                <label
                  className={`tipo-option ${tipoIngresso === "Inteira" ? "selected" : ""}`}
                >
                  <input
                    type="radio"
                    name="tipo"
                    value="Inteira"
                    checked={tipoIngresso === "Inteira"}
                    onChange={(e) => setTipoIngresso(e.target.value)}
                    disabled={comprando}
                  />
                  <div className="tipo-content">
                    <span className="tipo-nome">🎫 Inteira</span>
                    <span className="tipo-preco">
                      R$ {assentosInfo?.precoInteira.toFixed(2)}
                    </span>
                  </div>
                </label>

                <label
                  className={`tipo-option ${tipoIngresso === "Meia" ? "selected" : ""}`}
                >
                  <input
                    type="radio"
                    name="tipo"
                    value="Meia"
                    checked={tipoIngresso === "Meia"}
                    onChange={(e) => setTipoIngresso(e.target.value)}
                    disabled={comprando}
                  />
                  <div className="tipo-content">
                    <span className="tipo-nome">🎓 Meia Entrada</span>
                    <span className="tipo-preco">
                      R$ {assentosInfo?.precoMeia.toFixed(2)}
                    </span>
                    <span className="tipo-desc">(Estudante, Idoso, PCD)</span>
                  </div>
                </label>
              </div>
            </div>

            {/* Mapa de Assentos */}
            <div className="form-section">
              <h3>2. Escolha seu Assento</h3>
              <div className="tela-cinema">🎬 TELA</div>

              <div className="legenda-assentos">
                <span className="legenda-item">
                  <span className="assento-exemplo disponivel"></span>{" "}
                  Disponível
                </span>
                <span className="legenda-item">
                  <span className="assento-exemplo ocupado"></span> Ocupado
                </span>
                <span className="legenda-item">
                  <span className="assento-exemplo selecionado"></span>{" "}
                  Selecionado
                </span>
              </div>

              <div
                className="assentos-grid"
                style={{
                  gridTemplateColumns: `repeat(${calcularColunasGrid()}, 1fr)`,
                }}
              >
                {gerarAssentos().map((assento) => {
                  const ocupado =
                    assentosInfo?.assentosOcupados.includes(assento);
                  const selecionado = assentoSelecionado === assento;

                  return (
                    <button
                      key={assento}
                      type="button"
                      className={`assento ${ocupado ? "ocupado" : ""} ${selecionado ? "selecionado" : ""}`}
                      onClick={() => handleAssentoClick(assento)}
                      disabled={ocupado || comprando}
                      title={
                        ocupado
                          ? "Assento ocupado"
                          : `Selecionar assento ${assento}`
                      }
                    >
                      {assento}
                    </button>
                  );
                })}
              </div>
            </div>

            {/* Resumo da Compra */}
            {assentoSelecionado && (
              <div className="resumo-compra">
                <h3>📋 Resumo da Compra</h3>
                <div className="resumo-details">
                  <p>
                    <strong>Assento:</strong> {assentoSelecionado}
                  </p>
                  <p>
                    <strong>Tipo:</strong> {tipoIngresso}
                  </p>
                  <p className="resumo-total">
                    <strong>Total:</strong> R$ {precoAtual?.toFixed(2)}
                  </p>
                </div>
              </div>
            )}

            <div className="modal-footer">
              <button
                type="submit"
                className="btn btn-success btn-large"
                disabled={comprando || !assentoSelecionado}
              >
                {comprando
                  ? "Processando..."
                  : `💳 Confirmar Compra - R$ ${precoAtual?.toFixed(2)}`}
              </button>
              <button
                type="button"
                className="btn btn-danger"
                onClick={onClose}
                disabled={comprando}
              >
                Cancelar
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

export default IngressoModal;
