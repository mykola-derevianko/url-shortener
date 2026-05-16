export interface MeResponse {
  userId?: string;
  email?: string;
  role?: string;
}

export interface CurrentUser {
  id: string;
  email: string;
  role: 'Admin' | 'User' | null;
}
