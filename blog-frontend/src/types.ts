// blog-frontend/src/types.ts

// Tipos da Postagem (PostagensController)
export interface Postagem {
  id: string; // Guid
  titulo: string;
  conteudo: string;
  dataCriacao: string; // ISO Date String
  autorId: string;
  atualizadoEm?: string | null; // Opcional, como vimos
}

// Tipos de Autenticação (AuthController)
export interface LoginResponse {
  token: string;
  expiration: string; // Data de expiração do token
}

export interface User {
  id: string; // O 'sub' claim
  email: string; // Exemplo: usado para identificação no Header
  // Adicione a Role/Permissão do usuário aqui se for usado para a tela Admin
}

// Tipo de Requisição de Cadastro
export interface RegisterRequest {
  email: string;
  password: string;
  // Outros campos...
}