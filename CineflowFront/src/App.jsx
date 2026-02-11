import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Navbar from "./components/Navbar";
import Home from "./pages/Home";
import Filmes from "./pages/Filmes";
import Sessoes from "./pages/Sessoes";
import Salas from "./pages/Salas";
import Seed from "./pages/Seed";
import CriarSessao from "./pages/CriarSessao";
import "./App.css";

function App() {
  return (
    <Router>
      <div className="app">
        <Navbar />
        <main className="container">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/filmes" element={<Filmes />} />
            <Route path="/sessoes" element={<Sessoes />} />
            <Route path="/criar-sessao" element={<CriarSessao />} />
            <Route path="/salas" element={<Salas />} />
            <Route path="/seed" element={<Seed />} />
          </Routes>
        </main>
        <footer>
          <div className="container">
            <p>&copy; 2026 CineFlow - Sistema de Gerenciamento de Cinema</p>
          </div>
        </footer>
      </div>
    </Router>
  );
}

export default App;
