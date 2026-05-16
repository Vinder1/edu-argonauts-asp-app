import { auth } from "../auth/auth";
import { UserDto } from "../types/player";
import { fetchWithAuth } from "./fetchWithAuth";
import { useAuthStore } from "../vue/stores/authStore";

export async function loadUser(): Promise<void> {
    try {
        const authStore = useAuthStore();
        let response = await fetchWithAuth('/api/player');
        if (!response.ok) {
            authStore.isLoggedIn = false;
            auth.clear();
            return;
        }

        const data = await response.json();
        authStore.player = data as UserDto;
        authStore.isLoggedIn = true;

    } catch (error: unknown) {
        console.error('Ошибка загрузки:', error);
        useAuthStore().isLoggedIn = false;
    }
}
