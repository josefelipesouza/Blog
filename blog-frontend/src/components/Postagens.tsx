// src/components/Postagens.tsx

import React, { useState, useEffect } from 'react';
import api from '../services/api';
import type { Postagem } from '../types';
import { useAuth } from '../contexts/AuthContext';

export const PostagensList: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const [posts, setPosts] = useState<Postagem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  
  // Estados para Criação/Edição
  const [isEditing, setIsEditing] = useState<Postagem | null>(null);
  const [newPost, setNewPost] = useState({ titulo: '', conteudo: '' });

  const fetchPosts = async () => {
    try {
      setLoading(true);
      const response = await api.get<Postagem[]>('/postagens');
      setPosts(response.data);
    } catch (err) {
      setError('Erro ao carregar postagens.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPosts();
  }, []);

  // --- Funções CRUD ---

  const handleCreateOrEdit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (isEditing) {
        // PUT para editar
        await api.put(`/postagens/${isEditing.id}`, { ...newPost, id: isEditing.id });
        setIsEditing(null);
      } else {
        // POST para criar
        await api.post('/postagens', newPost);
      }
      setNewPost({ titulo: '', conteudo: '' });
      fetchPosts(); // Recarrega a lista
    } catch (err: any) {
      setError(err.response?.data?.detail || 'Erro ao salvar o post.');
    }
  };
  
  const handleDelete = async (id: string) => {
      if (window.confirm('Tem certeza que deseja excluir?')) {
          try {
              await api.delete(`/postagens/${id}`);
              fetchPosts();
          } catch (err: any) {
              setError(err.response?.data?.detail || 'Erro ao excluir o post.');
          }
      }
  };

  if (loading) return <p>Carregando postagens...</p>;
  if (error) return <p style={{ color: 'red' }}>{error}</p>;

  return (
    <div style={{ padding: '20px' }}>
      <h1>Postagens do Blog</h1>

      {isAuthenticated && (
        <div style={{ marginBottom: '30px', border: '1px solid #ddd', padding: '20px' }}>
          <h2>{isEditing ? 'Editar Postagem' : 'Criar Nova Postagem'}</h2>
          <form onSubmit={handleCreateOrEdit}>
            <input
              type="text"
              placeholder="Título"
              value={newPost.titulo}
              onChange={(e) => setNewPost({ ...newPost, titulo: e.target.value })}
              required
              style={{ width: '100%', padding: '8px', marginBottom: '10px' }}
            />
            <textarea
              placeholder="Conteúdo"
              value={newPost.conteudo}
              onChange={(e) => setNewPost({ ...newPost, conteudo: e.target.value })}
              required
              style={{ width: '100%', padding: '8px', marginBottom: '10px', minHeight: '100px' }}
            />
            <button type="submit">{isEditing ? 'Salvar Edição' : 'Publicar'}</button>
            {isEditing && <button type="button" onClick={() => setIsEditing(null)} style={{ marginLeft: '10px' }}>Cancelar</button>}
          </form>
        </div>
      )}

      {posts.map(post => (
        <div key={post.id} style={{ border: '1px solid #ccc', padding: '15px', marginBottom: '20px' }}>
          <h3>{post.titulo}</h3>
          <p>{post.conteudo}</p>
          <small>Publicado em: {new Date(post.dataCriacao).toLocaleDateString()}</small>
          
          {isAuthenticated && (
            <div style={{ marginTop: '10px' }}>
              <button onClick={() => { 
                  setIsEditing(post);
                  setNewPost({ titulo: post.titulo, conteudo: post.conteudo });
              }}>Editar</button>
              <button onClick={() => handleDelete(post.id)} style={{ marginLeft: '10px', backgroundColor: 'red', color: 'white' }}>Excluir</button>
            </div>
          )}
        </div>
      ))}
    </div>
  );
};