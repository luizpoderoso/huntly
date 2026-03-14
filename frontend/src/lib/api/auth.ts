import { apiFetch } from './client';
import type { TokenResponse, RegisterRequest, LoginRequest } from '$lib/types/auth';

export function register(request: RegisterRequest) {
    return apiFetch<TokenResponse>('/auth/register', {
        method: 'POST',
        body: JSON.stringify(request)
    });
}

export function login(request: LoginRequest) {
    return apiFetch<TokenResponse>('/auth/login', {
        method: 'POST',
        body: JSON.stringify(request)
    });
}