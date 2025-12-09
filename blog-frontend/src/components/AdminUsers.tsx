// src/components/AdminUsers.tsx

import React, { useState, useEffect } from 'react';
import api from '../services/api';
import type { User } from '../types';

export const AdminUsersList: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // ESTE ENDPOINT PRECISA SER IMPLEMENTADO NA API (.NET)
  const fetchUsers = async () => {
    try {
      setLoading(true);
      // **Atenção: Assumindo um endpoint /auth/users que só Admins podem acessar**
      const response = await api.get<User[]>('/auth/users');
      setUsers(response.data);
    } catch (err: any) {
      // O erro 401 ou 403 ocorrerá se o usuário não for Admin ou não estiver logado
      setError(err.response?.status === 403 ? 'Acesso negado. Necessita permissão de Admin.' : 'Erro ao carregar usuários.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

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
            {/* Adicionar coluna de Role/Permissão se disponível */}
          </tr>
        </thead>
        <tbody>
          {users.map(user => (
            <tr key={user.id}>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{user.id}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{user.email}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};