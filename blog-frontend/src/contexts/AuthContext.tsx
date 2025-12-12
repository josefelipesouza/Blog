import React, { createContext, useState, useEffect, useContext } from 'react';
import api from '../services/api';
import type { LoginResponse, User, RegisterRequest } from '../types'; // Certifique-se de que os tipos foram atualizados
import { jwtDecode } from 'jwt-decode';
import { AxiosError } from 'axios';

// --------------------------------------------------------------------------------
// 1. ATUALIZAÇÃO DO CONTEXTO (Adicionando isAdmin)
// --------------------------------------------------------------------------------
export interface AuthContextType {
    isAuthenticated: boolean;
    user: User | null;
    token: string | null;
    login: (email: string, password: string) => Promise<void>;
    register: (data: RegisterRequest) => Promise<string[]>;
    logout: () => void;
    isAdmin: () => boolean; // <-- NOVA FUNÇÃO: Checa se é Admin
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// --------------------------------------------------------------------------------
// 2. LÓGICA DE DECODIFICAÇÃO (Para persistência e refresh)
// --------------------------------------------------------------------------------
// Decodifica o token e constrói o objeto User (agora com roles como lista)
const decodeToken = (token: string): User | null => {
    try {
        const decoded: any = jwtDecode(token);

        // O ClaimTypes.Role (formato longo) ou "rol" (curto IANA)
        const roleClaimKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
        const roleClaimKeyIANA = 'rol';
        const roleClaimKeyShort = 'role';

        // O token pode ter a role como string ou como array de strings. 
        // Aqui, assumimos que o backend configurou uma das chaves:
        
        let roles: string[] = [];
        const roleRaw = decoded[roleClaimKey] || decoded[roleClaimKeyIANA] || decoded[roleClaimKeyShort];

        if (roleRaw) {
            // Se for uma string única (caso comum com uma role), transforma em array
            if (typeof roleRaw === 'string') {
                roles = [roleRaw];
            } 
            // Se for um array (caso de múltiplas roles), usa o array
            else if (Array.isArray(roleRaw)) {
                roles = roleRaw;
            }
        }
        
        // Retorna a estrutura User atualizada com a lista de roles
        const user: User = {
            id: decoded.sub,
            email: decoded.email || decoded.unique_name,
            userName: decoded.unique_name || decoded.email, // Ajuste o campo que contém o nome de usuário
            roles: roles 
        };

        return user;
    } catch (error) {
        console.error("Erro ao decodificar token:", error);
        return null;
    }
};

// --------------------------------------------------------------------------------
// 3. IMPLEMENTAÇÃO DO PROVIDER
// --------------------------------------------------------------------------------
export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [user, setUser] = useState<User | null>(null);
    const [token, setToken] = useState<string | null>(null);

    // Efeito para carregar token e usuário do Local Storage
    useEffect(() => {
        const savedToken = localStorage.getItem('authToken');
        if (savedToken) {
            const decodedUser = decodeToken(savedToken);
            if (decodedUser) {
                // Se o token for válido e decodificado, inicializa o estado
                setToken(savedToken);
                setUser(decodedUser);
                setIsAuthenticated(true);
            } else {
                localStorage.removeItem('authToken');
            }
        }
    }, []);

    // FUNÇÃO DE LOGIN (Usa o corpo da resposta para garantir as roles)
    const login = async (email: string, password: string) => {
        // A resposta agora contém o token E a lista de roles
        const response = await api.post<LoginResponse>('/auth/login', { email, password });
        const { token: newToken, username, email: userEmail, roles } = response.data; // <-- Capturando ROLES AQUI
        
        localStorage.setItem('authToken', newToken);

        // Criamos o objeto User diretamente com as informações da resposta (mais seguro após o login)
        const newUser: User = {
            id: jwtDecode(newToken).sub as string, // Usamos o token apenas para o ID (sub)
            userName: username,
            email: userEmail,
            roles: roles // <-- Inicializado com a lista de roles do corpo da resposta
        };
        
        setUser(newUser);
        setToken(newToken);
        setIsAuthenticated(true);
    };

    // FUNÇÃO PARA CHECAR ADMIN
    const isAdmin = (): boolean => {
        // Checa se o usuário está logado e se a role 'Admin' está na lista de roles
        return isAuthenticated && (user?.roles.includes('Admin') ?? false);
    };

    // ... (funções register e logout inalteradas, exceto a tipagem)
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
        <AuthContext.Provider value={{ isAuthenticated, user, token, login, register, logout, isAdmin }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) throw new Error('useAuth deve ser usado dentro de um AuthProvider');
    return context;
};