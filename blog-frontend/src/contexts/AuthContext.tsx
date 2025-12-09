// src/contexts/AuthContext.tsx

import React, { createContext, useState, useEffect, useContext } from 'react';
import api from '../services/api';
import { LoginResponse, User, RegisterRequest } from '../types';
import { jwtDecode } from 'jwt-decode';

interface AuthContextType {
  isAuthenticated: boolean;
  user: User | null;
  login: (email: string, password: string) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

const decodeToken = (token: string): User | null => {
  try {
    const decoded: any = jwtDecode(token);
    // Assumindo que o token JWT contém 'sub' (ID do usuário) e 'email'
    return {
      id: decoded.sub,
      email: decoded.email || decoded.unique_name,
    } as User;
  } catch (error) {
    console.error("Erro ao decodificar token:", error);
    return null;
  }
};

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    const token = localStorage.getItem('authToken');
    if (token) {
      const decodedUser = decodeToken(token);
      if (decodedUser) {
        setIsAuthenticated(true);
        setUser(decodedUser);
      } else {
        localStorage.removeItem('authToken');
      }
    }
  }, []);

  const login = async (email: string, password: string) => {
    const response = await api.post<LoginResponse>('/auth/login', { email, password });
    const { token } = response.data;

    localStorage.setItem('authToken', token);
    const decodedUser = decodeToken(token);

    if (decodedUser) {
      setUser(decodedUser);
      setIsAuthenticated(true);
    }
  };

  const register = async (data: RegisterRequest) => {
    await api.post('/auth/register', data);
    // Após o cadastro, o usuário pode ser automaticamente logado ou redirecionado para o login
    console.log("Usuário registrado com sucesso. Por favor, faça login.");
  };

  const logout = async () => {
    try {
      await api.post('/auth/logout');
    } catch (error) {
      console.error("Erro no logout da API, limpando estado local de qualquer forma.", error);
    } finally {
      localStorage.removeItem('authToken');
      setIsAuthenticated(false);
      setUser(null);
    }
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, user, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth deve ser usado dentro de um AuthProvider');
  }
  return context;
};