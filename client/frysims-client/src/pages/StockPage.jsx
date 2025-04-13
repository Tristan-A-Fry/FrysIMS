
import { useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

const StockPage = () => {
  const [stocks, setStocks] = useState([]);
  const [name, setName] = useState("");
  const [quantity, setQuantity] = useState(0);
  const [unit, setUnit] = useState("unit");
  const [originalPricePerUnit, setOriginalPricePerUnit] = useState("");
  const [isAdmin, setIsAdmin] = useState(false);

  useEffect(() => {
    fetchStocks();
    checkUserRole();
  }, []);

  const checkUserRole = () => {
    const token = localStorage.getItem("token");
    if (!token) return;
    const decoded = jwtDecode(token);
    const role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    setIsAdmin(role === "Admin");
  };

  const fetchStocks = async () => {

    const token = localStorage.getItem("token");
    try {
      const res = await fetch(`${API_BASE_URL}/api/stock`, {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type" : "application/json"
        }
      });
      const text = await res.text(); 
      const data = text ? JSON.parse(text) : [];
      setStocks(data);
    } catch (err) {
      console.error("Error fetching stocks:", err);
    }
  };

  const handleAddStock = async (e) => {
    
    if(!name || quantity < 0 || originalPricePerUnit < 0 || !unit )
    {
      alert("Please enter valid values. Quanity and price must be non-negative") 
      return;
    }

    const token = localStorage.getItem("token");
    e.preventDefault();
    try {
      const res = await fetch(`${API_BASE_URL}/api/stock`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({ name, quantity, unit, originalPricePerUnit })
      });

      if (res.ok) {
        fetchStocks();
        setName("");
        setQuantity(0);
        setUnit("unit");
        setOriginalPricePerUnit("");
      } else {
        console.error("Failed to add stock.");
      }
    } catch (err) {
      console.error("Add stock error:", err);
;
    }
  };


  const handleDelete = async (id) => {
    if (!window.confirm("Are you sure you want to delete this stock item?")) return;

    try {
      const res = await fetch(`${API_BASE_URL}/api/stock/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });

      if (!res.ok) {
        throw new Error("Failed to delete stock");
      }

      // Refetch stock list after deletion
      fetchStocks();
    } catch (error) {
      console.error("Error deleting stock:", error);
    }
    };

  const unitOptions = ["Pieces", "Ft", "Lbs", "Gallons", "Boxes"];

  return (
    <div className="p-8 text-white bg-gray-900 min-h-screen">
      <h2 className="text-3xl font-bold mb-6">Company Stock</h2>

      <div className="overflow-x-auto mb-8">
        <table className="min-w-full bg-gray-800 rounded-lg">
          <thead>
            <tr className="bg-gray-700 text-left">
              <th className="p-3">Name</th>
              <th className="p-3">Quantity</th>
              <th className="p-3">Unit</th>
              <th className="p-3">Original Price Per Unit</th>
            </tr>
          </thead>
          <tbody>
            {stocks.map((stock) => (
              <tr key={stock.id} className="border-t border-gray-700">
                <td className="p-3">{stock.name}</td>
                <td className="p-3">{stock.quantity}</td>
                <td className="p-3">{stock.unit}</td>
                <td className="p-3">${stock.originalPricePerUnit}</td>
                {isAdmin && (
                  <td className="p-3">
                    <button
                      onClick={() => handleDelete(stock.id)}
                      className="bg-red-600 hover:bg-red-700 text-white px-3 py-1 rounded"
                    >
                      Delete
                    </button>
                  </td> )}
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {isAdmin && (
        <form onSubmit={handleAddStock} className="bg-gray-800 p-6 rounded shadow-md max-w-md space-y-4">
          <h3 className="text-xl font-semibold">Add New Stock</h3>
          <input
            type="text"
            placeholder="Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="w-full p-2 rounded bg-gray-700 text-white"
            required
          />
          <input
            type="number"
            placeholder="Quantity"
            value={quantity}
            onChange={(e) => setQuantity(e.target.value)}
            className="w-full p-2 rounded bg-gray-700 text-white"
            required
          />
          <label className="block text-sm text-white mb-1">Unit</label>
          <select
            value={unit}
            onChange={(e) => setUnit(e.target.value)}
            className="w-full p-2 rounded bg-gray-700 text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="" disabled>Select Unit</option>
            {unitOptions.map((option) => (
              <option key={option} value={option}>{option}</option>
            ))}
          </select>
          <input
            type="number"
            placeholder="Original Price Per Unit"
            step="0.01"
            value={originalPricePerUnit}
            onChange={(e) => setOriginalPricePerUnit(e.target.value)}
            className="w-full p-2 rounded bg-gray-700 text-white"
            required
          />
          <button type="submit" className="bg-fryblue text-white px-4 py-2 rounded hover:bg-cyan-500">
            Add Stock
          </button>
        </form>
      )}
    </div>
  );
};

export default StockPage;

