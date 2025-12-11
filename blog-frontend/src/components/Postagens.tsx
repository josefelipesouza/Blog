// src/components/Postagens.tsx - Versão Corrigida Final com Mapeamento de Usuários

import React, { useState, useEffect } from 'react';
import api from '../services/api';
import type { Postagem } from '../types';
import { useAuth } from '../contexts/AuthContext';

// Define o tipo de dado para a postagem (para garantir a tipagem)
interface PostagemComAutor extends Postagem {
    // Adicione outros campos necessários se sua API os retornar
    autorUsername?: string; 
}


export const PostagensList: React.FC = () => {
    const { isAuthenticated, user } = useAuth();
    const [posts, setPosts] = useState<PostagemComAutor[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    // NOVIDADE: Estado para mapear AutorId -> UserName
    const [userMap, setUserMap] = useState<Record<string, string>>({}); 

    const [isEditing, setIsEditing] = useState<Postagem | null>(null);
    const [newPost, setNewPost] = useState({ titulo: '', conteudo: '' });

    // --- Estilos de Tema ---
    const CARD_BG = '#333'; 
    const CONTAINER_BG = '#242424'; 
    const ACCENT_COLOR = '#673ab7'; 
    const TEXT_COLOR = 'rgba(255, 255, 255, 0.87)'; 

    // NOVIDADE: Função para buscar todos os usuários e criar o mapa
    const fetchUsers = async () => {
        try {
            // ASSUMÇÃO: Endpoint que retorna uma lista de usuários com Id e UserName
            const response = await api.get<Array<{ id: string; userName: string }>>('/usuarios'); 
            
            const map: Record<string, string> = {};
            response.data.forEach(u => {
                map[u.id] = u.userName;
            });
            setUserMap(map);
        } catch (err) {
            console.error('Erro ao carregar mapa de usuários.', err);
            // Em caso de falha, não impede o carregamento dos posts
        }
    };
    
    const fetchPosts = async () => {
        try {
            setLoading(true);
            const response = await api.get<PostagemComAutor[]>('/postagens');
            setPosts(response.data);
        } catch (err) {
            setError('Erro ao carregar postagens.');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchUsers(); // 1. Carrega o mapa de usuários primeiro
        fetchPosts(); // 2. Carrega as postagens
    }, []);

    // ... (handleCreateOrEdit e handleDelete permanecem os mesmos) ...
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

    // --- Funções de Estilo (permanecem as mesmas) ---
    // ... (inputStyle, buttonStyle, buttonDangerStyle, postCardStyle, formContainerStyle) ...
    const inputStyle: React.CSSProperties = {
        width: '100%',
        padding: '12px',
        marginBottom: '10px',
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
        color: TEXT_COLOR,
        width: '100%', 
    };

    const formContainerStyle: React.CSSProperties = {
        ...postCardStyle, 
        border: `2px solid ${ACCENT_COLOR}`, 
        marginBottom: '30px',
        width: '100%', 
    };

    // --- Renderização ---

    if (loading) return <p style={{ padding: '20px', color: TEXT_COLOR }}>Carregando postagens...</p>;
    if (error) return <p style={{ padding: '20px', color: '#ff6b6b' }}>{error}</p>;

    return (
        <div 
            style={{ 
                padding: '20px', 
                paddingTop: '80px', 
                maxWidth: '900px', 
                margin: '0 auto', 
                backgroundColor: CONTAINER_BG, 
                color: TEXT_COLOR 
            }}
        >
            <h1>Postagens do Blog</h1>

            {/* ... (Bloco de criação/edição permanece o mesmo) ... */}
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
                                    setNewPost({ titulo: '', conteudo: '' }); 
                                }}
                                style={{ ...buttonStyle, marginLeft: '10px', backgroundColor: '#6c757d' }}
                            >
                                Cancelar
                            </button>
                        )}
                    </form>
                </div>
            )}
            
            {/* As postagens são listadas abaixo do formulário */}
            {posts.map(post => {
                const isOwner = user?.id === post.autorId;
                
                // CORREÇÃO: Usa o userMap para encontrar o UserName.
                // Se o userMap não estiver carregado ou o ID não for encontrado, usa 'Desconhecido'.
                const autorUsername = userMap[post.autorId] || 'Desconhecido'; 

                return (
                    <div key={post.id} style={postCardStyle}>
                        <h3 style={{ color: ACCENT_COLOR, marginTop: 0 }}>{post.titulo}</h3>
                        
                        {/* Exibe o nome de usuário do autor com '@' */}
                        <p style={{ 
                            color: ACCENT_COLOR, 
                            fontSize: '0.9rem', 
                            marginBottom: '10px' 
                        }}>
                            Por: @{autorUsername}
                        </p>
                        
                        <p>{post.conteudo}</p>
                        
                        <small style={{ color: '#aaa' }}>
                            Publicado em: {new Date(post.dataCriacao).toLocaleDateString('pt-BR', { 
                                year: 'numeric', 
                                month: '2-digit', 
                                day: '2-digit', 
                                hour: '2-digit', 
                                minute: '2-digit' 
                            })}
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