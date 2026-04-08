import { PUBLIC_API_URL } from '$env/static/public';

const BASE_URL = PUBLIC_API_URL ?? '/api';

interface FetchOptions extends RequestInit {
	token?: string;
}

export async function apiFetch<T>(path: string, options: FetchOptions = {}): Promise<T> {
	const { token, ...fetchOptions } = options;

	const headers = new Headers(fetchOptions.headers);

	const method = fetchOptions.method?.toUpperCase() ?? 'GET';

	if (method !== 'GET' && method !== 'DELETE') {
		headers.set('Content-Type', 'application/json');
	}

	if (token) {
		headers.set('Authorization', `Bearer ${token}`);
	}

	const response = await fetch(`${BASE_URL}${path}`, {
		...fetchOptions,
		headers
	});

	if (!response.ok) {
		const body = await response.json().catch(() => null);
		console.error('API Error:', { path, method, status: response.status, body });
		throw body ?? { error: `HTTP ${response.status}` };
	}

	// 204 No Content — return empty
	if (response.status === 204 || response.headers.get('content-length') === '0') {
		return undefined as T;
	}

	try {
		return (await response.json()) as Promise<T>;
	} catch {
		return undefined as T;
	}
}
