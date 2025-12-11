// src/components/Auth.tsx
import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { AxiosError } from 'axios';

export const AuthForm: React.FC = () => {
  const [isLogin, setIsLogin] = useState(true);
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const { login, register } = useAuth();

  const [errors, setErrors] = useState<string[]>([]);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrors([]);
    setLoading(true);

    try {
      if (isLogin) {
        // supondo que login lança ou resolve conforme seu AuthContext
        await login(email, password);
        // se login não redirecionar, seu AuthContext ou App deve reagir à mudança de auth
      } else {
        const registerErrors = await register({ username, email, password });
        if (registerErrors.length > 0) {
          setErrors(registerErrors);
          return;
        }

        alert('Cadastro realizado com sucesso! Faça login.');
        setIsLogin(true);
      }
    } catch (err: any) {
      const axiosErr = err as AxiosError;
      if (axiosErr?.response?.data) {
        const data = axiosErr.response.data as any;
        if (Array.isArray(data?.errors)) {
          setErrors(data.errors);
        } else if (data?.message) {
          setErrors([data.message]);
        } else {
          setErrors([axiosErr.message || 'Erro ao autenticar.']);
        }
      } else {
        setErrors([err?.message || 'Erro inesperado.']);
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ width: '100%', display: 'flex', justifyContent: 'center' }}>
      <div className="login-card" role="region" aria-label="Auth Card">
        <div className="visual-top">
          {isLogin ? 'Bem-vindo de volta' : 'Crie sua conta'}
        </div>

        <div className="form-wrap">
          <h2>{isLogin ? 'Login' : 'Cadastro'}</h2>

          {errors.length > 0 && (
            <div className="form-errors">
              {errors.map((err, idx) => <div key={idx}>• {err}</div>)}
            </div>
          )}

          <form onSubmit={handleSubmit} noValidate>
            {!isLogin && (
              <div className="input-wrapper">
                <span className="icon">👤</span>
                <input
                  type="text"
                  placeholder="Nome de usuário"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  required
                />
              </div>
            )}

            <div className="input-wrapper">
              <span className="icon">📧</span>
              <input
                type="email"
                placeholder="Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>

            <div className="input-wrapper">
              <span className="icon">🔒</span>
              <input
                type="password"
                placeholder="Senha"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </div>

            <button className="btn-primary" type="submit" disabled={loading}>
              {loading ? (isLogin ? 'Entrando...' : 'Registrando...') : (isLogin ? 'ENTRAR' : 'REGISTRAR')}
            </button>
          </form>

          <button
            className="card-footer-link"
            onClick={() => {
              setIsLogin(!isLogin);
              setErrors([]);
            }}
            aria-pressed={!isLogin}
          >
            {isLogin ? 'Ainda não tem conta? Crie sua conta' : 'Já possui uma conta? Acesse'}
          </button>
        </div>
      </div>
    </div>
  );
};
