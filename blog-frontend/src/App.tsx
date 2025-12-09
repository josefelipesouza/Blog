// src/App.tsx

import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { AuthForm } from './components/Auth';
import { PostagensList } from './components/Postagens';
import { AdminUsersList } from './components/AdminUsers';

// Componente para proteger a rota (ex: Admin)
const ProtectedRoute: React.FC<{ element: React.ReactElement, allowedRole?: string }> = ({ element, allowedRole }) => {
    const { isAuthenticated, user } = useAuth();

    // Verificação de autenticação básica
    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    // Se a role for necessária, verifique (aqui estamos simplificando, você deve verificar a role real do seu 'user' objeto)
    if (allowedRole && user && user.email !== "admin@blog.com") { // Exemplo simplificado de verificação de 'Admin'
         return <Navigate to="/" replace />;
    }

    return element;
};

const Header: React.FC = () => {
    const { isAuthenticated, user, logout } = useAuth();
    // Você precisará de uma forma robusta de verificar a Role do usuário (e.g., usando claims do JWT)
    const isAdmin = user?.email === "admin@blog.com"; 

    return (
        <nav style={{ background: '#333', padding: '15px', color: 'white', display: 'flex', justifyContent: 'space-between' }}>
            <div>
                <Link to="/" style={{ color: 'white', marginRight: '20px', textDecoration: 'none' }}>Home (Posts)</Link>
                {isAdmin && (
                    <Link to="/admin" style={{ color: 'yellow', textDecoration: 'none' }}>Admin (Usuários)</Link>
                )}
            </div>
            <div>
                {isAuthenticated ? (
                    <>
                        <span style={{ marginRight: '15px' }}>Olá, {user?.email}</span>
                        <button onClick={logout} style={{ padding: '5px 10px' }}>Logout</button>
                    </>
                ) : (
                    <Link to="/login" style={{ color: 'white', textDecoration: 'none' }}>Login/Cadastro</Link>
                )}
            </div>
        </nav>
    );
}


const App: React.FC = () => {
  return (
    <Router>
      <AuthProvider>
        <Header />
        <Routes>
          {/* Tela de Postagens (acessível para todos) */}
          <Route path="/" element={<PostagensList />} />
          
          {/* Tela de Login/Cadastro */}
          <Route path="/login" element={<AuthForm />} />
          
          {/* Tela de Admin (somente para usuários autenticados com permissão de Admin) */}
          <Route 
            path="/admin" 
            element={<ProtectedRoute element={<AdminUsersList />} allowedRole="Admin" />} 
          />

          {/* Fallback para páginas não encontradas */}
          <Route path="*" element={<h1>404 - Página Não Encontrada</h1>} />
        </Routes>
      </AuthProvider>
    </Router>
  );
};

export default App;