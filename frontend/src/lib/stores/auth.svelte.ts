import type { AuthUser, TokenResponse } from '$lib/types/auth';

const TOKEN_KEY = 'auth_token';
const USER_KEY = 'auth_user';

function loadFromStorage<T>(key: string): T | null {
    if (typeof localStorage === 'undefined') return null;
    try {
        const raw = localStorage.getItem(key);
        return raw ? (JSON.parse(raw) as T) : null;
    } catch {
        return null;
    }
}

function createAuthStore() {
    let token = $state<string | null>(loadFromStorage<string>(TOKEN_KEY));
    let user = $state<AuthUser | null>(loadFromStorage<AuthUser>(USER_KEY));

    return {
        get token() { return token; },
        get user() { return user; },
        get isAuthenticated() { return token !== null && user !== null; },
        get isExpired() {
            if (!user) return false;
            return new Date(user.expiresAt) < new Date();
        },

        login(response: TokenResponse) {
            const authUser: AuthUser = {
                id: response.userId,
                username: response.username,
                email: response.email,
                expiresAt: response.expiresAt
            };

            token = response.token;
            user = authUser;

            localStorage.setItem(TOKEN_KEY, JSON.stringify(token));
            localStorage.setItem(USER_KEY, JSON.stringify(authUser));
        },

        logout() {
            token = null;
            user = null;
            localStorage.removeItem(TOKEN_KEY);
            localStorage.removeItem(USER_KEY);
        }
    };
}

export const authStore = createAuthStore();