import { useNavigate } from "react-router-dom";
import "./FilmeModal.css";

function FilmeModal({ filme, onClose }) {
  const navigate = useNavigate();

  const handleBackdropClick = (e) => {
    if (e.target.className === "modal active") {
      onClose();
    }
  };

  return (
    <div className="modal active" onClick={handleBackdropClick}>
      <div className="modal-content">
        <div className="modal-header">
          <h2>{filme.titulo}</h2>
          <button className="modal-close" onClick={onClose}>
            &times;
          </button>
        </div>

        {filme.backdropPath && (
          <img
            src={`https://image.tmdb.org/t/p/w780${filme.backdropPath}`}
            alt={filme.titulo}
            className="modal-backdrop"
          />
        )}

        <div className="modal-body">
          <p>
            <strong>Título Original:</strong> {filme.tituloOriginal || "N/A"}
          </p>
          <p>
            <strong>Gênero:</strong> {filme.genero || "N/A"}
          </p>
          <p>
            <strong>Lançamento:</strong>{" "}
            {filme.dataLancamento
              ? new Date(filme.dataLancamento).toLocaleDateString("pt-BR")
              : "N/A"}
          </p>
          <p>
            <strong>Duração:</strong> {filme.duracaoMinutos || "N/A"} minutos
          </p>
          <p>
            <strong>Avaliação:</strong> ⭐{" "}
            {filme.voteAverage?.toFixed(1) || "N/A"} ({filme.voteCount || 0}{" "}
            votos)
          </p>

          {filme.sinopse && (
            <>
              <p>
                <strong>Sinopse:</strong>
              </p>
              <p>{filme.sinopse}</p>
            </>
          )}
        </div>

        <div className="modal-footer">
          <button
            className="btn btn-primary"
            onClick={() => navigate("/sessoes")}
          >
            Ver Sessões
          </button>
          <button className="btn btn-danger" onClick={onClose}>
            Fechar
          </button>
        </div>
      </div>
    </div>
  );
}

export default FilmeModal;
