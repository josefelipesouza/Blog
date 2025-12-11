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
  const isAdmin = user?.email === "admin@blog.com"; // ajuste conforme claims reais

  return (
    <header className="app-header">
      <div>
        <Link to="/" style={{ color: '#fff', marginRight: 20 }}>Home (Posts)</Link>
        {isAdmin && <Link to="/admin" style={{ color: 'yellow' }}>Admin (Usuários)</Link>}
      </div>

      <div>
        {isAuthenticated ? (
          <>
            <span style={{ marginRight: 12 }}>Olá, {user?.email}</span>
            <button onClick={logout} style={{ padding: '6px 10px', borderRadius: 6 }}>Logout</button>
          </>
        ) : (
          <Link to="/login" style={{ color: '#fff' }}>Login/Cadastro</Link>
        )}
      </div>
    </header>
  );
};

const App: React.FC = () => {
  return (
    <Router>
      <AuthProvider>
        <div className="app-root">
          <Header />

          {/* main ocupa todo o espaço restante e aplica o background das rotas */}
          <main className="app-main">
            <Routes>
              {/* Tela principal (posts) usa a mesma classe de background para estética */}
              <Route path="/" element={
                <div className="posts-background">
                  <PostagensList />
                </div>
              } />

              {/* Rota de login/cadastro */}
              <Route path="/login" element={
                <div className="auth-background">
                  <AuthForm />
                </div>
              } />

              {/* Rota admin (se existir) */}
              <Route path="/admin" element={
                <div className="posts-background">
                  <AdminUsersList />
                </div>
              } />

              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
          </main>
        </div>
      </AuthProvider>
    </Router>
  );
};

export default App;
