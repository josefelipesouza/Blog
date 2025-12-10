import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { AxiosError } from 'axios';

export const AuthForm: React.FC = () => {
  const [isLogin, setIsLogin] = useState(true);
  const [username, setUsername] = useState('');      // [NOVO]
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const { login, register } = useAuth();

  const [errors, setErrors] = useState<string[]>([]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrors([]);

    try {
      if (isLogin) {
        await login(email, password);
      } else {

        // Envio atualizado com username
        const registerErrors = await register({ username, email, password });

        if (registerErrors.length > 0) {
          setErrors(registerErrors);
          return;
        }

        alert('Cadastro realizado com sucesso! Faça login.');
        setIsLogin(true);
      }
    } catch (err: any) {
      const axiosError = err as AxiosError;

      if (axiosError.response && axiosError.response.data) {
        const responseData = axiosError.response.data as { message?: string, errors?: string[] };
        const errorMessage =
          responseData.message ||
          responseData.errors?.join(' | ') ||
          'Credenciais inválidas ou erro no login.';

        setErrors([errorMessage]);
      } else {
        setErrors(['Ocorreu um erro inesperado ao tentar autenticar.']);
      }
    }
  };

  return (
    <div style={{ maxWidth: '400px', margin: '50px auto', padding: '20px', border: '1px solid #ccc' }}>
      <h2>{isLogin ? 'Login' : 'Cadastro'}</h2>

      <form onSubmit={handleSubmit}>

        {/* Campo username somente no cadastro */}
        {!isLogin && (
          <input
            type="text"
            placeholder="Nome de usuário"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            style={{ width: '100%', padding: '10px', marginBottom: '10px' }}
          />
        )}

        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          style={{ width: '100%', padding: '10px', marginBottom: '10px' }}
        />

        <input
          type="password"
          placeholder="Senha"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          style={{ width: '100%', padding: '10px', marginBottom: '20px' }}
        />

        {errors.length > 0 && (
          <div style={{ marginBottom: '10px' }}>
            {errors.map((err, index) => (
              <p key={index} style={{ color: 'red', margin: '5px 0' }}>
                {err}
              </p>
            ))}
          </div>
        )}

        <button
          type="submit"
          style={{
            padding: '10px 15px',
            backgroundColor: '#007bff',
            color: 'white',
            border: 'none'
          }}
        >
          {isLogin ? 'Entrar' : 'Registrar'}
        </button>
      </form>

      <button
        onClick={() => setIsLogin(!isLogin)}
        style={{
          marginTop: '15px',
          background: 'none',
          border: 'none',
          color: '#007bff',
          cursor: 'pointer'
        }}
      >
        {isLogin
          ? 'Precisa de uma conta? Cadastre-se'
          : 'Já tem uma conta? Faça login'}
      </button>
    </div>
  );
};
