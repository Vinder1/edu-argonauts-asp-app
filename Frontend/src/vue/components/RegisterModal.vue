<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '../stores/authStore';
import { register } from '../../api/authLoader';

const authStore = useAuthStore();
const name = ref('');
const loginValue = ref('');
const email = ref('');
const password = ref('');
const confirmPassword = ref('');
const error = ref('');

async function handleSubmit() {
    error.value = '';
    if (password.value !== confirmPassword.value) {
        error.value = 'Пароли не совпадают';
        return;
    }
    const result = await register(name.value, loginValue.value, email.value, password.value);
    if (result) {
        error.value = result;
    }
}

function close() {
    authStore.showRegisterModal = false;
    error.value = '';
    name.value = '';
    loginValue.value = '';
    email.value = '';
    password.value = '';
    confirmPassword.value = '';
}
</script>

<template>
    <Teleport to="body">
        <div v-if="authStore.showRegisterModal" class="overlay" @click.self="close">
            <div class="modal">
                <button class="modal-close" @click="close">&times;</button>
                <h2>📝 Регистрация</h2>
                <form @submit.prevent="handleSubmit">
                    <div class="form-group">
                        <label for="register-username">Имя пользователя</label>
                        <input type="text" id="register-username" v-model="name" required placeholder="Ваше имя" minlength="3">
                    </div>
                    <div class="form-group">
                        <label for="register-login">Логин</label>
                        <input type="text" id="register-login" v-model="loginValue" required placeholder="Ваш логин">
                    </div>
                    <div class="form-group">
                        <label for="register-email">Email</label>
                        <input type="email" id="register-email" v-model="email" required placeholder="your@email.com">
                    </div>
                    <div class="form-group">
                        <label for="register-password">Пароль</label>
                        <input type="password" id="register-password" v-model="password" required placeholder="••••••••" minlength="6">
                    </div>
                    <div class="form-group">
                        <label for="register-confirm">Подтвердите пароль</label>
                        <input type="password" id="register-confirm" v-model="confirmPassword" required placeholder="••••••••">
                    </div>
                    <div class="form-error">{{ error }}</div>
                    <button type="submit" class="btn-primary">Зарегистрироваться</button>
                </form>
                <p class="modal-hint">Уже есть аккаунт? <a href="#" @click.prevent="authStore.showRegisterModal = false; authStore.showLoginModal = true">Войти</a></p>
            </div>
        </div>
    </Teleport>
</template>
