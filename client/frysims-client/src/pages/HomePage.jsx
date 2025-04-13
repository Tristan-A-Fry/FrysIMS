
import { useAuth } from "../context/AuthContext";
import { useNavigate, Link } from "react-router-dom";
const HomePage = () => {
  const navigate = useNavigate();
  const { isAuthenticated, logout } = useAuth();

  return (
    <div className="bg-gray-900 min-h-screen flex flex-col items-center text-white pt-20 px-4 sm:px-6 md:px-8">

      <h1> HOME PAGE </h1>

    </div>
  );
};

export default HomePage;
