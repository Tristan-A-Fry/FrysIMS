
import { jwtDecode } from "jwt-decode";
import { useAuth } from "../context/AuthContext";
import { useNavigate, Link } from "react-router-dom";
import { useEffect ,useState } from "react";

const Navbar = () => {
  const { isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();
  const [menuOpen, setMenuOpen] = useState(false);
  const [userRole, setUserRole] = useState("");

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      try {
        const decoded = jwtDecode(token);
        const role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        setUserRole(role);
      } catch (err) {
        console.error("Invalid token:", err);
        setUserRole("");
      }
    }
  }, []);

  return (
    <nav className="bg-gray-800 text-white p-4 top-0 w-full shadow-md">
      <div className="max-w-6xl mx-auto flex items-center justify-between">
        {/* Logo */}
        <Link to="/" className="text-xl font-bold text-white">
          <img src="/FR_logo_transparent.png" alt="FryReads Logo" className="h-12 w-auto" />
        </Link>

        {/* Hamburger Icon (visible on small screens only) */}
        <button
          className="md:hidden text-white focus:outline-none"
          onClick={() => setMenuOpen(!menuOpen)}
        >
          <svg
            className="w-6 h-6"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            {menuOpen ? (
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            ) : (
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
            )}
          </svg>
        </button>

        {/* Nav Links: Hidden on small screens */}
        <div className="hidden md:flex space-x-8 items-center text-lg font-semibold">
          <Link to="/" className="hover:text-gray-300">Home</Link>
          <Link to="/stock" className="hover:text-gray-300">Stock</Link>
          <Link to="/projects" className="hover:text-gray-300">Projects</Link>

          {isAuthenticated && userRole === "Admin" && (
            <button
              onClick={() => navigate("/admin-dashboard")}
              className="bg-[#2ac9ff] px-5 py-2 rounded-lg hover:shadow-lg hover:shadow-[0_0_10px_#2ac9ff]"
            >
              Admin Dashboard
            </button>
          )}

          {isAuthenticated ? (
            <button
              onClick={logout}
              className="bg-[#2ac9ff] px-5 py-2 rounded-lg hover:shadow-lg hover:shadow-[0_0_10px_#2ac9ff]"
            >
              Logout
            </button>
          ) : (
            <button
              onClick={() => navigate("/login")}
              className="bg-[#2ac9ff] px-5 py-2 rounded-lg hover:shadow-lg hover:shadow-[0_0_10px_#2ac9ff]"
            >
              Login
            </button>
          )}
        </div>
      </div>

      {/* Mobile Menu */}
      {menuOpen && (
        <div className="md:hidden flex flex-col space-y-3 mt-4 px-4 text-lg font-semibold">
          <Link to="/" onClick={() => setMenuOpen(false)} className="hover:text-gray-300">Home</Link>
          <Link to="/projects" onClick={() => setMenuOpen(false)} className="hover:text-gray-300">Projects</Link>
          <Link to="/stock" onClick={() => setMenuOpen(false)} className="hover:text-gray-300">Stock</Link>
          {isAuthenticated ? (
            <button
              onClick={() => {
                logout();
                navigate("/login");
                setMenuOpen(false);
              }}
              className="bg-cyan-400 px-4 py-2 rounded-lg"
            >
              Logout
            </button>
          ) : (
            <button
              onClick={() => {
                navigate("/login");
                setMenuOpen(false);
              }}
              className="bg-cyan-400 px-4 py-2 rounded-lg"
            >
              Login
            </button>
          )}
        </div>
      )}
    </nav>
  );
};
export default Navbar;




