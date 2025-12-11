// blog-frontend/src/types.ts

// Tipos de Postagens
export interface Postagem {
  id: string;
  titulo: string;
  conteudo: string;
  dataCriacao: string;
  autorId: string;
  atualizadoEm?: string | null;
}

// Resposta de Login (AuthController)
export interface LoginResponse {
  token: string;
  expiration: string;
}

// Representação do usuário autenticado
export interface User {
  id: string;
  email: string;
  role: string;      // Agora sempre existe (não opcional)
}

// Requisição de cadastro
export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}
