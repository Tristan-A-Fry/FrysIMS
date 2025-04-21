
import { useState } from "react";
import { useNavigate } from "react-router-dom";

const AdminPage = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [role, setRole] = useState("");
    const [errorMessage, setErrorMessage] = useState(""); // Added state for error message
    const navigate = useNavigate();


  const handleRegister = async (e) => {
    e.preventDefault();
    setErrorMessage(""); // Reset the error message before new submission

    if (password !== confirmPassword) {
      setErrorMessage("Passwords do not match.");
      return;
    }

    const requestBody = { 
      email, 
      password,
      confirmPassword,
      role
    };

    try {
      const token = localStorage.getItem("token");
      const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;
      const response = await fetch(`${API_BASE_URL}/api/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json",
        "Authorization" : `Bearer ${localStorage.getItem("token")}`},
        body: JSON.stringify(requestBody),
      });

      const data = await response.json();
      console.log("Backend Response:", data); // Log the response for debugging

      if (response.ok) {
        alert("Registration successful! Please log in.");
        navigate("/login");
      } else {
        // Check for the error in the response and display the message
        if (data.error) {
          setErrorMessage(data.error);  // Display the error from the backend
        } else {
          setErrorMessage("Registration failed. Please try again."); // Default error message
        }
      }
    } catch (error) {
      console.error("Error:", error);
      setErrorMessage("An error occurred while registering.");
    }
  };

  return (
    <div className="flex items-start justify-center min-h-screen bg-gray-900 pt-80">
      <div className="bg-gray-800 text-white p-8 rounded-lg shadow-lg w-96">
        <h2 className="text-2xl font-semibold text-center mb-4">Register</h2>
        {errorMessage && <div className="text-red-500 text-center mb-4">{errorMessage}</div>} {/* Error message display */}
        <form onSubmit={handleRegister} className="space-y-4">
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
          <input
            type="password"
            placeholder="Confirm Password"
            className="w-full p-3 rounded bg-gray-700 text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            required
          />
          <input
            type="role"
            placeholder="User Role"
            className="w-full p-3 rounded bg-gray-700 text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={role}
            onChange={(e) => setRole(e.target.value)}
            required
          />
          <button
            type="submit"
            className="w-full bg-[#2ac9ff] hover:bg-[#4494c0] text-white py-2 rounded transition"
          >
            Register
          </button>
        </form>
        <div className="text-center mt-3">
          <a href="/login" className="text-blue-400 hover:underline">
            Already have an account? Login
          </a>
        </div>
      </div>
    </div>
  );
};
export default AdminPage;
