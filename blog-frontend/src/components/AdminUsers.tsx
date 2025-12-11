// src/components/AdminUsers.tsx

import React, { useState, useEffect } from 'react';
import api from '../services/api';
import type { User } from '../types';
import { useAuth } from '../contexts/AuthContext';

export const AdminUsersList: React.FC = () => {
  const { token, isAuthenticated, user } = useAuth();
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchUsers = async () => {
    try {
      setLoading(true);

      if (!isAuthenticated || !token) {
        throw new Error('Usuário não autenticado');
      }

      if (!user || user.role !== 'Admin') {
        throw new Error('Acesso negado. Necessita permissão de Admin.');
      }

      // Faz requisição para o endpoint protegido com o token JWT
      const response = await api.get<User[]>('/auth/users', {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });

      setUsers(response.data);
    } catch (err: any) {
      setError(
        err.message === 'Usuário não autenticado'
          ? 'Usuário não autenticado. Faça login.'
          : err.message === 'Acesso negado. Necessita permissão de Admin.'
          ? 'Acesso negado. Necessita permissão de Admin.'
          : 'Erro ao carregar usuários.'
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, [token, isAuthenticated, user]); // Reexecuta quando token, autenticação ou user mudar

  if (loading) return <p>Carregando usuários...</p>;
  if (error) return <p style={{ color: 'red' }}>{error}</p>;

  return (
    <div style={{ padding: '20px' }}>
      <h1>🛠️ Painel Administrativo - Usuários</h1>
      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr style={{ backgroundColor: '#f2f2f2' }}>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>ID</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Email</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Role</th>
          </tr>
        </thead>
        <tbody>
          {users.map(u => (
            <tr key={u.id}>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{u.id}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{u.email}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{u.role || '-'}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
