import { createContext, useContext, useState } from "react";
import axiosClient from "../api/axiosClient";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const saved = localStorage.getItem("user");
    return saved ? JSON.parse(saved) : null;
  });

  const login = async (email, password) => {
    const res = await axiosClient.post("/auth/login", { email, password });
    const { token, fullName, role } = res.data.data;

    localStorage.setItem("token", token);
    localStorage.setItem("user", JSON.stringify({ fullName, role }));
    setUser({ fullName, role });

    return { fullName, role };
  };

  const register = async (fullName, email, password, phone) => {
    await axiosClient.post("/auth/register", { fullName, email, password, phone });
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
