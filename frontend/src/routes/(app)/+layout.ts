import { resolve } from '$app/paths';
import { redirect } from '@sveltejs/kit';

export const ssr = false;

export const load = () => {
    const token = localStorage.getItem('auth_token');
    if (!token) {
        redirect(302, resolve('/login'));
    }
};
