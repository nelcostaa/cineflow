import { useState } from "react";
import { api } from "../services/api";
import "./FilmeModal.css";

function IngressoModal({ sessao, onClose, onSuccess }) {
  const [lugarMarcado, setLugarMarcado] = useState("");
  const [preco, setPreco] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      setLoading(true);
      setError(null);
      await api.comprarIngresso(sessao.id, lugarMarcado, parseFloat(preco));
      alert("Ingresso comprado com sucesso!");
      onSuccess();
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleBackdropClick = (e) => {
    if (e.target.className === "modal active") {
      onClose();
    }
  };

  return (
    <div className="modal active" onClick={handleBackdropClick}>
      <div className="modal-content">
        <div className="modal-header">
          <h2>Comprar Ingresso</h2>
          <button className="modal-close" onClick={onClose}>
            &times;
          </button>
        </div>

        <div className="modal-body">
          <p>
            <strong>Filme:</strong> {sessao.filme?.titulo}
          </p>
          <p>
            <strong>Sala:</strong> {sessao.sala?.nome}
          </p>
          <p>
            <strong>Horário:</strong>{" "}
            {new Date(sessao.horarioInicio).toLocaleString("pt-BR")}
          </p>

          {error && <div className="alert alert-error">{error}</div>}

          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Assento (ex: A1, B5):</label>
              <input
                type="text"
                value={lugarMarcado}
                onChange={(e) => setLugarMarcado(e.target.value)}
                required
                placeholder="Ex: A1"
                disabled={loading}
              />
            </div>

            <div className="form-group">
              <label>Preço (R$):</label>
              <input
                type="number"
                step="0.01"
                min="0.01"
                value={preco}
                onChange={(e) => setPreco(e.target.value)}
                required
                placeholder="Ex: 25.00"
                disabled={loading}
              />
            </div>

            <div className="modal-footer">
              <button
                type="submit"
                className="btn btn-success"
                disabled={loading}
              >
                {loading ? "Processando..." : "Confirmar Compra"}
              </button>
              <button
                type="button"
                className="btn btn-danger"
                onClick={onClose}
                disabled={loading}
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
