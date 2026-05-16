<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '../stores/authStore';
import { login } from '../../api/authLoader';

const authStore = useAuthStore();
const loginValue = ref('');
const password = ref('');
const error = ref('');

async function handleSubmit() {
    error.value = '';
    const result = await login(loginValue.value, password.value);
    if (result) {
        error.value = result;
    }
}

function close() {
    authStore.showLoginModal = false;
    error.value = '';
    loginValue.value = '';
    password.value = '';
}
</script>

<template>
    <Teleport to="body">
        <div v-if="authStore.showLoginModal" class="overlay" @click.self="close">
            <div class="modal">
                <button class="modal-close" @click="close">&times;</button>
                <h2>🔐 Вход в аккаунт</h2>
                <form @submit.prevent="handleSubmit">
                    <div class="form-group">
                        <label for="login-login">Email</label>
                        <input type="text" id="login-login" v-model="loginValue" required placeholder="Ваш логин">
                    </div>
                    <div class="form-group">
                        <label for="login-password">Пароль</label>
                        <input type="password" id="login-password" v-model="password" required placeholder="••••••••">
                    </div>
                    <div class="form-error">{{ error }}</div>
                    <button type="submit" class="btn-primary">Войти</button>
                </form>
                <p class="modal-hint">Нет аккаунта? <a href="#" @click.prevent="authStore.showLoginModal = false; authStore.showRegisterModal = true">Зарегистрироваться</a></p>
            </div>
        </div>
    </Teleport>
</template>
