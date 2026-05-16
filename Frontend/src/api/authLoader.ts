import { auth } from "../auth/auth";
import { showNotification } from "../utils/notifications";
import { signalrProvider } from "./signalrProvider";
import { loadUser } from "./userLoader";
import { loadSpaceship } from "./spaceshipLoader";
import { loadAllResources } from "./spaceshipResourcesLoader";
import { useAuthStore } from "../vue/stores/authStore";
import { useSpaceshipStore } from "../vue/stores/spaceshipStore";
import { useSpaceshipResourcesStore } from "../vue/stores/spaceshipResourcesStore";
import { useSpaceshipActionsStore } from "../vue/stores/spaceshipActionsStore";
import { useStarInfoStore } from "../vue/stores/starInfoStore";
import { useOverlayVisibilityStore } from "../vue/stores/overlayVisibilityStore";
import { useGameStore } from "../vue/stores/gameStore";

export async function login(loginValue: string, password: string): Promise<string | null> {
    const authStore = useAuthStore();
    try {
        const response = await fetch('/api/auth/log-in', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ login: loginValue, password })
        });

        if (!response.ok) {
            return 'Ошибка связи с сервером';
        }

        const data = await response.json();

        if (!data.success) {
            return data.errors?.join(', ') ?? 'Неизвестная ошибка';
        }

        if (data.token) {
            auth.setToken(data.token);
            authStore.isLoggedIn = true;
            authStore.showLoginModal = false;
            await Promise.all([loadUser(), loadSpaceship(), loadAllResources()]);
            showNotification('Добро пожаловать!');
        }
        return null;
    } catch {
        return 'Не удалось подключиться к серверу';
    }
}

export async function register(name: string, loginValue: string, email: string, password: string): Promise<string | null> {
    const authStore = useAuthStore();
    try {
        const response = await fetch('/api/auth/sign-in', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, login: loginValue, email, password })
        });

        const data = await response.json();

        if (!response.ok || !data.success) {
            return data.errors?.join(', ') ?? 'Неизвестная ошибка';
        }

        if (data.token) {
            auth.setToken(data.token);
            authStore.isLoggedIn = true;
            authStore.showRegisterModal = false;
            await loadUser();
            showNotification('Регистрация успешна!');
        }
        return null;
    } catch {
        return 'Не удалось подключиться к серверу';
    }
}

export function logout() {
    fetch('/api/auth/logout', { method: 'POST' }).catch(() => {});
    auth.clear();
    signalrProvider.stop().catch(() => {});
    useAuthStore().reset();
    useSpaceshipStore().reset();
    useSpaceshipResourcesStore().reset();
    useSpaceshipActionsStore().reset();
    useStarInfoStore().reset();
    useOverlayVisibilityStore().reset();
    useGameStore().reset();
    showNotification('Вы вышли из аккаунта');
}