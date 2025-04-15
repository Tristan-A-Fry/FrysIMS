
import { useState } from "react";
import {jwtDecode} from "jwt-decode";
import { useNavigate } from "react-router-dom";
import {useAuth} from "../context/AuthContext";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

const LoginPage = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();
  const {login} = useAuth();

  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const res = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
      });

      if (!res.ok) {
        setError("Login failed.");
        return;
      }

      const data = await res.json();
      login(data.token);

      const decoded = jwtDecode(data.token);
      const userRole = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

      if (userRole === "Admin") {
        navigate("/projects");
      } else {
        navigate("/");
      }
    } catch (err) {
      setError("Something went wrong.");
    }
  };

  return (
    <div className="flex items-start justify-center min-h-screen bg-gray-900 pt-80">
      <div className="bg-gray-800 text-white p-8 rounded-lg shadow-lg w-96">
        <h2 className="text-2xl font-semibold text-center mb-4">Login</h2>
        <form onSubmit={handleLogin} className="space-y-4">
          <input
            type="email"
            placeholder="Email"
            className="w-full p-3 rounded bg-gray-700 text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
          <input
            type="password"
            placeholder="Password"
            className="w-full p-3 rounded bg-gray-700 text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          
          <button
            type="submit"
            className="w-full bg-[#2ac9ff] hover:bg-[#4494c0] text-white py-2 rounded transition"
          >
            Login
          </button>
        </form>
      </div>
    </div>
  );
};

export default LoginPage;
