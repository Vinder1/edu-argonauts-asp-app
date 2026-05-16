<script setup lang="ts">
import { watch } from 'vue';
import { useGameStore } from '../stores/gameStore';
import { useAuthStore } from '../stores/authStore';
import { logout } from '../../api/authLoader';
import { useSpaceshipStore } from '../stores/spaceshipStore';

const game = useGameStore();
const authStore = useAuthStore();
const spaceshipStore = useSpaceshipStore();

const emit = defineEmits<{
    reload: [];
    centerOnSpaceship: [];
    toggleAnimation: [];
    brightnessChange: [value: number];
    scaleChange: [value: number];
}>();

watch(() => game.brightness, (v) => emit('brightnessChange', v));
watch(() => game.scale, (v) => emit('scaleChange', v));
</script>

<template>
    <div class="controls">
        <div class="controls-section">
            <button v-if="!authStore.isLoggedIn" @click="authStore.showLoginModal = true" class="btn-auth">Войти</button>
            <button v-if="!authStore.isLoggedIn" @click="authStore.showRegisterModal = true" class="btn-auth">Регистрация</button>
            <button v-if="authStore.isLoggedIn" @click="logout()" class="btn-auth btn-logout">Выйти</button>
            <span v-if="authStore.isLoggedIn" class="user-badge">Авторизован</span>
        </div>

        <div class="controls-section">
            <button @click="emit('centerOnSpaceship')" class="btn-action">
                <span v-if="spaceshipStore.spaceship && spaceshipStore.hasSpaceship">Корабль</span>
                <span v-else>Центр</span>
            </button>
            <button @click="emit('toggleAnimation')" class="btn-action">
                {{ game.isAnimating ? 'Остановить' : 'Движение клавиатурой' }}
            </button>
            <button @click="emit('reload')" class="btn-action">Обновить</button>
        </div>

        <div class="controls-section sliders">
            <div class="slider-item">
                <label>Яркость</label>
                <input type="range" v-model.number="game.brightness" min="0.2" max="1" step="0.1">
            </div>
            <div class="slider-item">
                <label>Масштаб</label>
                <input type="range" v-model.number="game.scale" min="10" max="60" step="2">
            </div>
        </div>
    </div>
</template>

<style scoped>
.controls {
    position: absolute;
    bottom: 20px;
    left: 50%;
    transform: translateX(-50%);
    z-index: 10;
    display: flex;
    align-items: center;
    gap: 16px;
    padding: 12px 20px;
    background: rgba(20, 20, 40, 0.85);
    backdrop-filter: blur(12px);
    border: 1px solid rgba(100, 100, 255, 0.3);
    border-radius: 16px;
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.4), inset 0 1px 0 rgba(255, 255, 255, 0.1);
}

.controls-section {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 0 12px;
    border-right: 1px solid rgba(255, 255, 255, 0.1);
}

.controls-section:last-child {
    border-right: none;
}

button {
    cursor: pointer;
    color: #e0e0ff;
    background: linear-gradient(135deg, rgba(80, 80, 140, 0.6), rgba(60, 60, 120, 0.8));
    border: 1px solid rgba(100, 100, 255, 0.3);
    border-radius: 8px;
    padding: 8px 14px;
    font-size: 13px;
    font-weight: 600;
    letter-spacing: 0.5px;
    transition: all 0.25s ease;
    white-space: nowrap;
}

button:hover {
    background: linear-gradient(135deg, rgba(100, 100, 180, 0.7), rgba(80, 80, 160, 0.9));
    border-color: rgba(130, 130, 255, 0.5);
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(100, 100, 255, 0.3);
}

button:active {
    transform: translateY(0);
}

.btn-auth {
    background: linear-gradient(135deg, rgba(60, 100, 140, 0.6), rgba(40, 80, 120, 0.8));
    border-color: rgba(80, 150, 255, 0.3);
}

.btn-auth:hover {
    background: linear-gradient(135deg, rgba(80, 120, 180, 0.7), rgba(60, 100, 160, 0.9));
    border-color: rgba(100, 180, 255, 0.5);
    box-shadow: 0 4px 12px rgba(80, 150, 255, 0.3);
}

.btn-logout {
    background: linear-gradient(135deg, rgba(140, 60, 60, 0.6), rgba(120, 40, 40, 0.8));
    border-color: rgba(255, 80, 80, 0.3);
}

.btn-logout:hover {
    background: linear-gradient(135deg, rgba(180, 80, 80, 0.7), rgba(160, 60, 60, 0.9));
    border-color: rgba(255, 100, 100, 0.5);
    box-shadow: 0 4px 12px rgba(255, 80, 80, 0.3);
}

.user-badge {
    color: #8f8;
    font-size: 12px;
    padding: 6px 10px;
    background: rgba(100, 255, 100, 0.1);
    border: 1px solid rgba(100, 255, 100, 0.3);
    border-radius: 6px;
}

.sliders {
    display: flex;
    gap: 16px;
}

.slider-item {
    display: flex;
    flex-direction: column;
    gap: 4px;
}

.slider-item label {
    color: #aab;
    font-size: 11px;
    text-transform: uppercase;
    letter-spacing: 1px;
}

input[type="range"] {
    -webkit-appearance: none;
    appearance: none;
    width: 100px;
    height: 4px;
    background: linear-gradient(90deg, rgba(80, 80, 140, 0.6), rgba(140, 100, 255, 0.6));
    border-radius: 2px;
    outline: none;
}

input[type="range"]::-webkit-slider-thumb {
    -webkit-appearance: none;
    appearance: none;
    width: 14px;
    height: 14px;
    background: #fff;
    border: 2px solid rgba(140, 100, 255, 0.8);
    border-radius: 50%;
    cursor: pointer;
    box-shadow: 0 0 8px rgba(140, 100, 255, 0.5);
    transition: all 0.2s ease;
}

input[type="range"]::-webkit-slider-thumb:hover {
    transform: scale(1.2);
    box-shadow: 0 0 12px rgba(140, 100, 255, 0.8);
}
</style>
