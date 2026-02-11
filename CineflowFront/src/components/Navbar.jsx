import { Link } from "react-router-dom";
import "./Navbar.css";

function Navbar() {
  return (
    <nav className="navbar">
      <div className="container">
        <h1 className="logo">🎬 CineFlow</h1>
        <ul className="nav-menu">
          <li>
            <Link to="/">Início</Link>
          </li>
          <li>
            <Link to="/filmes">Filmes</Link>
          </li>
          <li>
            <Link to="/sessoes">Sessões</Link>
          </li>
          <li>
            <Link to="/criar-sessao">Criar Sessão</Link>
          </li>
          <li>
            <Link to="/salas">Salas</Link>
          </li>
          <li>
            <Link to="/seed">Seed</Link>
          </li>
        </ul>
      </div>
    </nav>
  );
}

export default Navbar;
