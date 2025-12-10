import React, { createContext, useState, useEffect, useContext } from 'react';
import api from '../services/api';
import type { LoginResponse, User, RegisterRequest } from '../types';
import { jwtDecode } from 'jwt-decode';
import { AxiosError } from 'axios';

interface AuthContextType {
  isAuthenticated: boolean;
  user: User | null;
  login: (email: string, password: string) => Promise<void>;
  register: (data: RegisterRequest) => Promise<string[]>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

const decodeToken = (token: string): User | null => {
  try {
    const decoded: any = jwtDecode(token);
    return {
      id: decoded.sub,
      email: decoded.email || decoded.unique_name
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
    try {
      const response = await api.post<LoginResponse>('/auth/login', { email, password });
      const { token } = response.data;

      localStorage.setItem('authToken', token);
      const decodedUser = decodeToken(token);

      if (decodedUser) {
        setUser(decodedUser);
        setIsAuthenticated(true);
      } else {
        throw new Error("Token inválido após login.");
      }
    } catch (error) {
      throw error;
    }
  };

  // Atualizado para enviar username também
  const register = async (data: RegisterRequest): Promise<string[]> => {
    try {
      await api.post('/auth/register', {
        username: data.username,     // NOVO
        email: data.email,
        password: data.password
      });

      console.log("Usuário registrado com sucesso. Por favor, faça login.");
      return [];
    } catch (error) {
      const axiosError = error as AxiosError;

      if (axiosError.response && axiosError.response.status === 400) {
        const errorData = axiosError.response.data as { errors?: string[]; message?: string };

        if (errorData.errors && Array.isArray(errorData.errors)) {
          return errorData.errors;
        } else if (errorData.message) {
          return [errorData.message];
        }
      }

      return ["Ocorreu um erro inesperado. Tente novamente."];
    }
  };

  const logout = async () => {
    try {
      await api.post('/auth/logout');
    } catch (error) {
      console.error("Erro no logout da API, limpando estado local mesmo assim.", error);
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
