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

// Estrutura exata da resposta da API no /auth/login
export interface LoginResponse {
    token: string;
    username: string;
    email: string;
    roles: string[]; // <-- IMPORTANTE: Agora é uma lista (como vem do backend)
}

// Estrutura do usuário que armazenamos no estado (User | null)
export interface User {
    id: string;
    email: string;
    userName: string;
    roles: string[]; // <-- MUDANÇA: Usaremos a lista de roles
}

// Estrutura para o registro
export interface RegisterRequest {
    username: string;
    email: string;
    password: string;
}
