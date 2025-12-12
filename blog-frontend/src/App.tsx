// src/App.tsx
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { AuthForm } from './components/Auth';
import { PostagensList } from './components/Postagens';
import { AdminUsersList } from './components/AdminUsers';

/* Header component - usa classes do CSS global */
const Header: React.FC = () => {
  const { isAuthenticated, user, logout } = useAuth();


  return (
    <header className="app-header">
      <div>
        {/* ALTERAÇÃO 2: Link de Postagens só aparece se estiver autenticado */}
        {isAuthenticated && (
          <Link to="/" style={{ color: '#fff', marginRight: 20 }}>Postagens (Home)</Link>
        )}
        

      </div>

      <div>
        {isAuthenticated ? (
          <>
            <span style={{ marginRight: 12 }}>Olá, {user?.email}</span>
            <button onClick={logout} style={{ padding: '6px 10px', borderRadius: 6 }}>Logout</button>
          </>
        ) : (
          /* Link de Login/Cadastro só aparece se NÃO estiver autenticado */
          <Link to="/login" style={{ color: '#fff' }}>Login/Cadastro</Link>
        )}
      </div>
    </header>
  );
};

// Componente Wrapper para Rotas Protegidas ou Específicas
const ProtectedRoute: React.FC<{ children: React.ReactElement }> = ({ children }) => {
  const { isAuthenticated } = useAuth();
  // ALTERAÇÃO 4: Redireciona para /login se não estiver autenticado
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  return children;
};


const App: React.FC = () => {
  return (
    <Router>
      <AuthProvider>
        <div className="app-root">
          <Header />

          <main className="app-main">
            <Routes>
              {/* Rota de login/cadastro (acessível por todos) */}
              <Route path="/login" element={
                <div className="auth-background">
                  <AuthForm />
                </div>
              } />

              {/* Rota principal de Postagens (PROTEGIDA) */}
              <Route path="/" element={
                <ProtectedRoute>
                  <div className="posts-background">
                    <PostagensList />
                  </div>
                </ProtectedRoute>
              } />

              {/* Rota admin (PROTEGIDA) */}
              <Route path="/admin" element={
                <ProtectedRoute>
                  <div className="posts-background">
                    <AdminUsersList />
                  </div>
                </ProtectedRoute>
              } />

              {/* Rota curinga: Redireciona para Home (Postagens) se logado, ou para Login se não logado */}
              {/* Esta lógica garante que a tela inicial será Postagens (se logado) ou Login (se deslogado) */}
              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
          </main>
        </div>
      </AuthProvider>
    </Router>
  );
};

export default App;