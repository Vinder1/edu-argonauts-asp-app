import { auth } from "../auth/auth";

export async function fetchWithAuth(url: string, options: RequestInit = {}): Promise<Response> {
    let response = await fetch(url, {
        ...options,
        headers: {
            ...auth.headers(),
            ...options.headers,
        },
    });

    if (response.status === 401) {
        await auth.refresh();
        response = await fetch(url, {
            ...options,
            headers: {
                ...auth.headers(),
                ...options.headers,
            },
        });
    }

    return response;
}
