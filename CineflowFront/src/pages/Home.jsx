import { useNavigate } from "react-router-dom";
import "./Home.css";

function Home() {
  const navigate = useNavigate();

  return (
    <div className="content">
      <div className="hero">
        <h2>🎬 Bem-vindo ao CineFlow</h2>
        <p>Sistema completo de gerenciamento de cinema</p>
        <div className="btn-group">
          <button
            className="btn btn-primary"
            onClick={() => navigate("/filmes")}
          >
            Ver Filmes
          </button>
          <button
            className="btn btn-success"
            onClick={() => navigate("/sessoes")}
          >
            Ver Sessões
          </button>
        </div>
      </div>

      <div className="cards-grid">
        <div className="card" onClick={() => navigate("/filmes")}>
          <div className="card-content">
            <h3>📽️ Filmes em Cartaz</h3>
            <p className="card-info">
              Veja todos os filmes disponíveis no cinema
            </p>
          </div>
        </div>

        <div className="card" onClick={() => navigate("/sessoes")}>
          <div className="card-content">
            <h3>🎫 Sessões</h3>
            <p className="card-info">Confira os horários das sessões</p>
          </div>
        </div>

        <div className="card" onClick={() => navigate("/salas")}>
          <div className="card-content">
            <h3>🏢 Salas</h3>
            <p className="card-info">Gerencie as salas do cinema</p>
          </div>
        </div>

        <div className="card" onClick={() => navigate("/seed")}>
          <div className="card-content">
            <h3>🌱 Seed</h3>
            <p className="card-info">Popular banco com dados de teste</p>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Home;
