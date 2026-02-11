import { useState, useEffect } from "react";
import { api } from "../services/api";
import FilmeModal from "../components/FilmeModal";
import "./Filmes.css";

function Filmes() {
  const [filmes, setFilmes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedFilme, setSelectedFilme] = useState(null);

  useEffect(() => {
    loadFilmes();
  }, []);

  const loadFilmes = async () => {
    try {
      setLoading(true);
      const data = await api.getFilmes();
      setFilmes(data);
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleFilmeClick = async (id) => {
    try {
      const filme = await api.getFilme(id);
      setSelectedFilme(filme);
    } catch (err) {
      alert("Erro ao carregar detalhes do filme");
    }
  };

  if (loading) {
    return (
      <div className="content">
        <div className="loading">Carregando filmes...</div>
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

  if (filmes.length === 0) {
    return (
      <div className="content">
        <h2>Filmes em Cartaz</h2>
        <div className="alert alert-error">
          Nenhum filme cadastrado. Use a página Seed para importar filmes.
        </div>
      </div>
    );
  }

  return (
    <div className="content">
      <h2>Filmes em Cartaz</h2>
      <div className="filmes-grid">
        {filmes.map((filme) => (
          <div
            key={filme.id}
            className="filme-card"
            onClick={() => handleFilmeClick(filme.id)}
          >
            {filme.posterPath ? (
              <img
                src={`https://image.tmdb.org/t/p/w500${filme.posterPath}`}
                alt={filme.titulo}
                className="filme-poster"
              />
            ) : (
              <div className="filme-poster-placeholder"></div>
            )}
            <div className="filme-info">
              <h3>{filme.titulo}</h3>
              <p className="filme-rating">
                ⭐ {filme.voteAverage?.toFixed(1) || "N/A"}
              </p>
              <p className="filme-date">
                📅{" "}
                {filme.dataLancamento
                  ? new Date(filme.dataLancamento).toLocaleDateString("pt-BR")
                  : "N/A"}
              </p>
              {filme.genero && <p className="filme-genre">{filme.genero}</p>}
            </div>
          </div>
        ))}
      </div>

      {selectedFilme && (
        <FilmeModal
          filme={selectedFilme}
          onClose={() => setSelectedFilme(null)}
        />
      )}
    </div>
  );
}

export default Filmes;
