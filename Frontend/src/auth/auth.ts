export const auth = {
    isSignedInLight() {
        return this.getToken() != null;
    },

    getToken() {
        const token = localStorage.getItem("token");
        return token;
    },

    async refresh() {
        const response = await fetch('/api/auth/refresh/', {
            method: 'POST'
        });
        if (!response.ok) {
            this.clear();
        }
        const result = await response.json();
        this.setToken(result.token);
    },

    setToken(value: string) {
        localStorage.setItem("token", value);
    },

    headers() {
        const headers: Record<string,string> = { "Content-Type": "application/json" };
        const token = this.getToken();
        if (token) {
            headers["Authorization"] = `Bearer ${token}`;
        }
        return headers;
    },

    clear() {
        localStorage.removeItem("token");
    }
}