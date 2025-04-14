import { useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

const ProjectsPage = () => {
  const [projects, setProjects] = useState([]);
  const [name, setName] = useState("");
  const [budget, setBudget] = useState("");
  const [isAdmin, setIsAdmin] = useState(false);
  const [currentUserId, setCurrentUserId] = useState("");

  const [showModalFor, setShowModalFor] = useState(null);
  const [selectedProjectId, setSelectedProjectId] = useState(null);
  const [stockList, setStockList] = useState([]);
  const [selectedStockId, setSelectedStockId] = useState("");
  const [quantityUsed, setQuantityUsed] = useState("");
  const [unitPrice, setUnitPrice] = useState("");
  const [projectMaterials, setProjectMaterials] = useState([]);

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      const decoded = jwtDecode(token);
      const role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
      setIsAdmin(role === "Admin");
      setCurrentUserId(decoded.sub);
    }
    fetchProjects();
  }, []);

  const fetchProjects = async () => {
    try {
      const res = await fetch(`${API_BASE_URL}/api/projects`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`
        }
      });
      if (!res.ok) throw new Error("Failed to fetch projects");
      const data = await res.json();
      console.log("Fetched Projects:", data);
      setProjects(data);
    } catch (err) {
      console.error("Error fetching projects:", err);
    }
  };


  const fetchStockList = async () => {
    try {
      const res = await fetch(`${API_BASE_URL}/api/stock`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`
        }
      });
      const data = await res.json();
      setStockList(data);
    } catch (err) {
      console.error("Error fetching stock list:", err);
    }
  };

  const fetchProjectMaterials = async (projectId) => {
    try {
      const res = await fetch(`${API_BASE_URL}/api/projectMaterial/project/${projectId}`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`
        }
      });
      const data = await res.json();
      setProjectMaterials(data);
    } catch (err) {
      console.error("Error fetching project materials:", err);
    }
  };

  const handleAddProject = async () => {
    if (!name || !budget || budget < 0) {
      alert("Project name and a valid non-negative budget are required.");
      return;
    }

    try {
      const res = await fetch(`${API_BASE_URL}/api/projects`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`
        },
        body: JSON.stringify({ name, budget: parseFloat(budget) })
      });

      if (!res.ok) throw new Error("Failed to add project");
      setName("");
      setBudget("");
      fetchProjects();
    } catch (err) {
      console.error("Error adding project:", err);
    }
  };

  const handleAssignMaterial = async () => {
    if (!selectedStockId || quantityUsed <= 0) {
      alert("Please select a stock and a valid quantity.");
      return;
    }

    try {
      const selected = stockList.find(s => s.id === parseInt(selectedStockId));
      const res = await fetch(`${API_BASE_URL}/api/projectMaterial`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`
        },
        body: JSON.stringify({
          projectId: selectedProjectId,
          stockId: parseInt(selectedStockId),
          quantityUsed: parseInt(quantityUsed),
          unitCostSnapshot: selected?.originalPricePerUnit || 0
        })
      });

      if (!res.ok) throw new Error("Failed to assign material");
      alert("Material assigned successfully.");
      fetchProjectMaterials(selectedProjectId);
      setQuantityUsed("");
      setSelectedStockId("");
      fetchProjects();
    } catch (err) {
      console.error("Error assigning material:", err);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Are you sure you want to delete this project?")) return;

    try {
      const res = await fetch(`${API_BASE_URL}/api/projects/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });

      if (!res.ok) {
        throw new Error("Failed to delete stock");
      }

      // Refetch stock list after deletion
      fetchProjects();
    } catch (error) {
      console.error("Error deleting project:", error);
    }
  };

  const deleteProjectMaterial = async (id) =>{

    if (!window.confirm("Are you sure you want to delete this project material item?")) return;
    try {
      const res = await fetch(`${API_BASE_URL}/api/projectMaterial/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });

      if (!res.ok) {
        throw new Error("Failed to delete stock");
      }

    await fetchProjectMaterials(selectedProjectId);
    await fetchProjects();
    await fetchStockList();
    } catch (error) {
      console.error("Error deleting project:", error);
    }
  };

  return (
    <div className="min-h-screen bg-gray-900 text-white p-8">
      <h1 className="text-2xl font-bold mb-6">Projects</h1>

      <div className="overflow-x-auto mb-10">
        <table className="min-w-full bg-gray-800 text-left">
          <thead className="bg-gray-700">
            <tr>
              <th className="py-2 px-4">Name</th>
              <th className="py-2 px-4">Budget</th>
              <th className="py-2 px-4">Budget Left</th>
              <th className="py-2 px-4">Date Created</th>
              <th className="py-2 px-4">Created By</th>
              <th className="py-2 px-4">Actions</th>
            </tr>
          </thead>
          <tbody>
            {projects.map((project) => (
              <tr key={project.id} className="border-b border-gray-700">
                <td className="py-2 px-4">{project.name}</td>
                <td className="py-2 px-4">${project.budget}</td>
                <td className="py-2 px-4">
                  ${(
                    project.budget -
                    project.projectMaterials.reduce(
                      (acc, mat) => acc + mat.unitCostSnapshot * mat.quantityUsed,
                      0
                    )
                  ).toFixed(2)}
                </td>
                <td className="py-2 px-4">{new Date(project.dateCreated).toLocaleDateString()}</td>
                <td className="py-2 px-4">{project.createdByUserId}</td>
                <td className="py-2 px-4">
                  {project.createdByUserId === currentUserId && (
                    <button
                      className="text-red-400 underline"
                      onClick={() => {
                        handleDelete(project.id);
                      }}
                    >
                      Delete 
                    </button> 
                  )}
                    <button
                      className="text-blue-400 underline"
                      onClick={() => {
                        setShowModalFor(project.id);
                        setSelectedProjectId(project.id);
                        fetchStockList();
                        fetchProjectMaterials(project.id);
                      }}
                    >
                      View Details
                    </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {isAdmin && (
        <div className="bg-gray-800 p-6 rounded w-full max-w-sm">
          <h2 className="text-xl font-semibold mb-4">Add New Project</h2>
          <input
            type="text"
            placeholder="Project Name"
            className="w-full mb-3 p-2 rounded bg-gray-700"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
          <input
            type="number"
            placeholder="Budget"
            className="w-full mb-3 p-2 rounded bg-gray-700"
            value={budget}
            onChange={(e) => setBudget(e.target.value)}
          />
          <button
            onClick={handleAddProject}
            className="bg-blue-500 hover:bg-blue-600 px-4 py-2 rounded w-full"
          >
            Add Project
          </button>
        </div>
      )}

      {showModalFor && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
          <div className="bg-gray-800 text-white p-6 rounded-lg w-full max-w-lg">
            <h2 className="text-xl font-semibold mb-4">Assign Material to Project</h2>
            <label className="block mb-2">Select Material</label>
            <select
              value={selectedStockId}
              onChange={(e) => {
                const selected = stockList.find(s => s.id === parseInt(e.target.value));
                setSelectedStockId(e.target.value);
                setUnitPrice(selected?.originalPricePerUnit || 0);
              }}
              className="w-full mb-4 p-2 rounded bg-gray-700"
            >
              <option value="">-- Select Stock --</option>
              {stockList.map(stock => (
                <option key={stock.id} value={stock.id}>
                  {stock.name} (Available: {stock.quantity})
                </option>
              ))}
            </select>

            <label className="block mb-2">Quantity</label>
            <input
              type="number"
              value={quantityUsed}
              onChange={(e) => setQuantityUsed(e.target.value)}
              className="w-full mb-4 p-2 rounded bg-gray-700"
              min="1"
            />

            <p className="mb-4">Unit Price: <strong>${unitPrice}</strong></p>

            <button
              className="bg-fryblue px-4 py-2 rounded hover:bg-cyan-500 mr-4"
              onClick={handleAssignMaterial}
            >
              Assign
            </button>
            <button
              className="bg-gray-600 px-4 py-2 rounded hover:bg-gray-500"
              onClick={() => setShowModalFor(null)}
            >
              Close
            </button>

            {/* Show Assigned Materials */}
            <div className="mt-6">
              <h3 className="text-lg font-semibold mb-2">Assigned Materials</h3>
              {projectMaterials.length > 0 ? (
                <ul className="space-y-2">
                  {projectMaterials.map((mat) => (
                    <li key={mat.id} className="bg-gray-700 p-2 rounded">
                      {mat.stockName} — Qty: {mat.quantityUsed}, Unit Cost: ${mat.unitCostSnapshot}, Total Cost: ${mat.unitCostSnapshot * mat.quantityUsed}
                    <button
                      onClick={() => deleteProjectMaterial(mat.id)}
                      className="bg-red-600 hover:bg-red-700 text-white px-3 py-1 rounded"
                    >
                      Delete
                    </button>
                    </li>
                  ))}
                </ul>
              ) : (
                <p className="text-sm text-gray-400">No materials assigned yet.</p>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ProjectsPage;
