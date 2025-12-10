// blog-frontend/src/types.ts

// Tipos da Postagem (PostagensController)
export interface Postagem {
  id: string;
  titulo: string;
  conteudo: string;
  dataCriacao: string;
  autorId: string;
  atualizadoEm?: string | null;
}

// Tipos de Autenticação (AuthController)
export interface LoginResponse {
  token: string;
  expiration: string;
}

export interface User {
  id: string;
  email: string;
}

// Tipo de Requisição de Cadastro
export interface RegisterRequest {
  username: string;   // Linha adicionada
  email: string;
  password: string;
}
