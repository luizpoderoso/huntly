export interface TokenResponse {
    token: string;
    userId: string;
    username: string;
    email: string;
    expiresAt: string;
}

export interface RegisterRequest {
    fullName: string;
    username: string;
    email: string;
    password: string;
    confirmPassword: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface AuthUser {
    id: string;
    username: string;
    email: string;
    expiresAt: string;
}