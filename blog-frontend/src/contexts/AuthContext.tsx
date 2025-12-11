// AuthContext.tsx

import React, { createContext, useState, useEffect, useContext } from 'react';
import api from '../services/api';
import type { LoginResponse, User, RegisterRequest } from '../types';
import { jwtDecode } from 'jwt-decode';
import { AxiosError } from 'axios';

export interface AuthContextType {
    isAuthenticated: boolean;
    user: User | null;
    token: string | null;
    login: (email: string, password: string) => Promise<void>;
    register: (data: RegisterRequest) => Promise<string[]>;
    logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Decodificação correta + captura robusta da role
const decodeToken = (token: string): User | null => {
    try {
        const decoded: any = jwtDecode(token);

        console.log("Token decodificado:", decoded);
        console.log("Chaves disponíveis no token:", Object.keys(decoded)); 

        // Busca a role em todos os formatos possíveis (O frontend está pronto para a solução do backend):
        let roleRaw = '';
        
        // 1. Formato longo do ASP.NET Identity (ClaimTypes.Role)
        // 2. Formato IANA padrão "rol" (Configurado no Program.cs)
        // 3. Formato alternativo "role"
        roleRaw = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
                  decoded['rol'] || 
                  decoded['role'] || 
                  '';
        
        // Se não encontrou, busca em todas as chaves do token (Fallback)
        if (!roleRaw) {
            const allKeys = Object.keys(decoded);
            for (const key of allKeys) {
                if (key.toLowerCase().includes('role') || key.toLowerCase().includes('rol')) {
                    roleRaw = decoded[key];
                    break;
                }
            }
        }
        
        // Ajusta para garantir que a role seja sempre uma string, tratando caso seja um array
        const role = Array.isArray(roleRaw) ? roleRaw[0] : roleRaw;

        console.log("Role capturada:", role);

        const user: User = {
          id: decoded.sub,
          email: decoded.email || decoded.unique_name,
          role: role,
          userName: ''
        };

        return user;
    } catch (error) {
        console.error("Erro ao decodificar token:", error);
        return null;
    }
};

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    // ... (restante do AuthProvider inalterado)

    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [user, setUser] = useState<User | null>(null);
    const [token, setToken] = useState<string | null>(null);

    useEffect(() => {
        const savedToken = localStorage.getItem('authToken');
        if (savedToken) {
            const decodedUser = decodeToken(savedToken);
            if (decodedUser) {
                setIsAuthenticated(true);
                setUser(decodedUser);
                setToken(savedToken);
            } else {
                localStorage.removeItem('authToken');
            }
        }
    }, []);

    const login = async (email: string, password: string) => {
        const response = await api.post<LoginResponse>('/auth/login', { email, password });
        const { token: newToken } = response.data;

        localStorage.setItem('authToken', newToken);

        const decodedUser = decodeToken(newToken);
        if (!decodedUser) throw new Error('Token inválido após login.');

        setUser(decodedUser);
        setIsAuthenticated(true);
        setToken(newToken);
    };

    const register = async (data: RegisterRequest): Promise<string[]> => {
        try {
            await api.post('/auth/register', {
                username: data.username,
                email: data.email,
                password: data.password
            });
            return [];
        } catch (error) {
            const axiosError = error as AxiosError;
            if (axiosError.response?.status === 400) {
                const errorData = axiosError.response.data as { errors?: string[]; message?: string };
                if (errorData.errors) return errorData.errors;
                if (errorData.message) return [errorData.message];
            }
            return ['Ocorreu um erro inesperado. Tente novamente.'];
        }
    };

    const logout = () => {
        localStorage.removeItem('authToken');
        setIsAuthenticated(false);
        setUser(null);
        setToken(null);
    };

    return (
        <AuthContext.Provider value={{ isAuthenticated, user, token, login, register, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) throw new Error('useAuth deve ser usado dentro de um AuthProvider');
    return context;
};