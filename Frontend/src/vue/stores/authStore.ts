import { defineStore } from 'pinia';
import { ref } from 'vue';
import { auth } from '../../auth/auth';
import { UserDto } from '../../types/player';

export const useAuthStore = defineStore('auth', () => {
    const isLoggedIn = ref(false);
    const showLoginModal = ref(false);
    const showRegisterModal = ref(false);
    const player = ref<UserDto>({
        id: "unknown",
        name: "unknown",
        login: "unknown",
        email: "unknown",
        role: "unknown",
    });

    function initialize() {
        isLoggedIn.value = auth.isSignedInLight();
    }

    function reset() {
        isLoggedIn.value = false;
        showLoginModal.value = false;
        showRegisterModal.value = false;
        player.value = {
            id: "unknown",
            name: "unknown",
            login: "unknown",
            role: "unknown",
        };
    }

    return {
        isLoggedIn,
        showLoginModal,
        showRegisterModal,
        player,
        initialize,
        reset
    };
});
