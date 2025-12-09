// src/services/api.ts

import axios from 'axios';

// URL base da sua API rodando no Docker
const API_URL = 'http://localhost:5000/api'; // Ajuste a porta se necessário

const api = axios.create({
  baseURL: API_URL,
});

api.interceptors.request.use(config => {
  const token = localStorage.getItem('authToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;