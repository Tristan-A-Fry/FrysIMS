
import Navbar from "./components/Navbar"; // Adjust path based on your folder structure
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import HomePage from "./pages/HomePage"
import LoginPage from "./pages/LoginPage";
import StockPage from "./pages/StockPage";
import ProjectsPage from "./pages/ProjectsPage";
import { AuthProvider } from "./context/AuthContext"; 

function App() {
  const token = localStorage.getItem("token");

  return (
    <AuthProvider>
      <Router>
        <Navbar />
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route
            path="/stock"
            element={token ? <StockPage /> : <Navigate to="/" />}
          />
          <Route
            path="/projects"
            element={token ? <ProjectsPage /> : <Navigate to="/" />}
          />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;


