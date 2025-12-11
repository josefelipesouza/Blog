// src/components/Postagens.tsx

import React, { useState, useEffect } from 'react';
import api from '../services/api';
import type { Postagem } from '../types';
import { useAuth } from '../contexts/AuthContext';

export const PostagensList: React.FC = () => {
  const { isAuthenticated, user } = useAuth();
  const [posts, setPosts] = useState<Postagem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [isEditing, setIsEditing] = useState<Postagem | null>(null);
  const [newPost, setNewPost] = useState({ titulo: '', conteudo: '' });

  // --- Estilos de Tema ---
  const CARD_BG = '#333'; // Fundo dos cards de postagem
  const CONTAINER_BG = '#242424'; // Fundo do container principal (igual ao do body)
  const ACCENT_COLOR = '#673ab7'; // Cor de destaque (Roxo)
  const TEXT_COLOR = 'rgba(255, 255, 255, 0.87)'; // Cor do texto (Claro)

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

  const handleCreateOrEdit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (isEditing) {
        await api.put(`/postagens/${isEditing.id}`, {
          ...newPost,
          id: isEditing.id
        });
        setIsEditing(null);
      } else {
        await api.post('/postagens', newPost);
      }
      setNewPost({ titulo: '', conteudo: '' });
      fetchPosts();
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

  // --- Funções de Estilo ---

  const inputStyle: React.CSSProperties = {
    width: '100%',
    padding: '12px',
    marginBottom: '10px',
    // Forçando cores para aparecerem sobre fundo escuro
    backgroundColor: '#444', 
    color: TEXT_COLOR,
    border: '1px solid #555',
    borderRadius: '8px',
    boxSizing: 'border-box'
  };

  const buttonStyle: React.CSSProperties = {
    padding: '10px 15px',
    backgroundColor: ACCENT_COLOR,
    color: 'white',
    border: 'none',
    borderRadius: '6px',
    cursor: 'pointer',
    fontWeight: 'bold',
    transition: 'background-color 0.2s',
  };

  const buttonDangerStyle: React.CSSProperties = {
    ...buttonStyle,
    backgroundColor: '#dc3545',
  };

  const postCardStyle: React.CSSProperties = {
    backgroundColor: CARD_BG,
    border: '1px solid #444',
    padding: '20px',
    marginBottom: '20px',
    borderRadius: '10px',
    boxShadow: '0 2px 5px rgba(0, 0, 0, 0.5)',
    color: TEXT_COLOR
  };

  const formContainerStyle: React.CSSProperties = {
    ...postCardStyle, // Reutiliza o estilo de card
    border: `2px solid ${ACCENT_COLOR}`, // Borda de destaque para o formulário
    marginBottom: '30px',
  };

  // --- Renderização ---

  if (loading) return <p style={{ padding: '20px', color: TEXT_COLOR }}>Carregando postagens...</p>;
  if (error) return <p style={{ padding: '20px', color: '#ff6b6b' }}>{error}</p>;

  return (
    <div 
        // Container principal da lista de postagens
        style={{ 
            padding: '20px', 
            maxWidth: '900px', 
            margin: '0 auto', // Centraliza a lista
            backgroundColor: CONTAINER_BG, 
            color: TEXT_COLOR 
        }}
    >
      <h1>Postagens do Blog</h1>

      {isAuthenticated && (
        <div style={formContainerStyle}>
          <h2 style={{ color: ACCENT_COLOR, marginTop: 0 }}>
            {isEditing ? 'Editar Postagem' : 'Criar Nova Postagem'}
          </h2>

          <form onSubmit={handleCreateOrEdit}>
            <input
              type="text"
              placeholder="Título"
              value={newPost.titulo}
              onChange={(e) => setNewPost({ ...newPost, titulo: e.target.value })}
              required
              style={inputStyle}
            />

            <textarea
              placeholder="Conteúdo"
              value={newPost.conteudo}
              onChange={(e) => setNewPost({ ...newPost, conteudo: e.target.value })}
              required
              style={{ ...inputStyle, minHeight: '100px' }}
            />

            <button type="submit" style={buttonStyle}>
              {isEditing ? 'Salvar Edição' : 'Publicar'}
            </button>

            {isEditing && (
              <button
                type="button"
                onClick={() => {
                  setIsEditing(null);
                  setNewPost({ titulo: '', conteudo: '' }); // Limpa campos ao cancelar
                }}
                style={{ ...buttonStyle, marginLeft: '10px', backgroundColor: '#6c757d' }}
              >
                Cancelar
              </button>
            )}
          </form>
        </div>
      )}

      {posts.map(post => {
        const isOwner = user?.id === post.autorId;

        return (
          <div key={post.id} style={postCardStyle}>
            <h3 style={{ color: ACCENT_COLOR, marginTop: 0 }}>{post.titulo}</h3>
            <p>{post.conteudo}</p>
            <small style={{ color: '#aaa' }}>
              Publicado em: {new Date(post.dataCriacao).toLocaleDateString()}
            </small>

            {isAuthenticated && isOwner && (
              <div style={{ marginTop: '15px' }}>
                <button
                  onClick={() => {
                    setIsEditing(post);
                    setNewPost({ titulo: post.titulo, conteudo: post.conteudo });
                  }}
                  style={buttonStyle}
                >
                  Editar
                </button>

                <button
                  onClick={() => handleDelete(post.id)}
                  style={{
                    ...buttonDangerStyle,
                    marginLeft: '10px',
                  }}
                >
                  Excluir
                </button>
              </div>
            )}
          </div>
        );
      })}
    </div>
  );
};