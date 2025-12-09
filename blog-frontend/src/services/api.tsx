// src/services/api.ts

import axios from 'axios';

// 1. Define a URL Base usando Variáveis de Ambiente do Vite.
//    - No Docker Compose, VITE_API_URL será 'http://blogapi:8080/api'.
//    - Fora do Docker, ele usará a URL local de fallback ('http://localhost:5000/api').
//    O 'import.meta.env' é a forma padrão do Vite de acessar variáveis prefixadas com VITE_.
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_URL,
  // 2. Definir um timeout pode ser útil para requisições de API
  timeout: 10000, 
});

// 3. Interceptor para Injeção de Token (Lógica de Autenticação)
api.interceptors.request.use(config => {
  const token = localStorage.getItem('authToken');
  
  if (token) {
    // Adiciona o token no cabeçalho Authorization para rotas que exigem [Authorize]
    config.headers.Authorization = `Bearer ${token}`;
  }
  
  return config;
}, error => {
  // Lidar com erros de requisição antes de serem enviadas (opcional)
  return Promise.reject(error);
});

// 4. Interceptor para Respostas (Lógica de Logout/Erro Global)
api.interceptors.response.use(
  response => response,
  error => {
    // Exemplo de como lidar com status de erro 401 (Não Autorizado) globalmente
    if (error.response && error.response.status === 401) {
      console.error("Sessão expirada ou não autorizada.");
      // Opcional: Redirecionar para a página de login ou forçar logout
      // localStorage.removeItem('authToken');
      // window.location.href = '/login'; 
    }
    return Promise.reject(error);
  }
);

export default api;