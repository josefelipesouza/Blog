// src/services/api.ts

import axios from 'axios';

// 1. Define a URL Base usando Variáveis de Ambiente do Vite.
//    - No Docker Compose, VITE_API_URL será 'http://blogapi:8080/api'.
//    - Fora do Docker, ele usará a URL local ('http://localhost:5000/api').
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_URL,
  timeout: 10000,

  // Linha adicionada:
  // Garante que TODAS as requisições usem JSON e não application/x-www-form-urlencoded.
  headers: {
    'Content-Type': 'application/json',
  },
});

// 3. Interceptor para Injeção de Token (Autenticação)
api.interceptors.request.use(
  config => {
    const token = localStorage.getItem('authToken');

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

// 4. Interceptor para tratamento de respostas (Logout automático ou erros globais)
api.interceptors.response.use(
  response => response,
  error => {
    if (error.response && error.response.status === 401) {
      console.error("Sessão expirada ou não autorizada.");

      // Aqui você pode forçar logout ou redirecionamento, se desejar:
      // localStorage.removeItem('authToken');
      // window.location.href = '/login';
    }

    return Promise.reject(error);
  }
);

export default api;
