<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue';
import { signalrProvider } from '../../api/signalrProvider';
import { useAuthStore } from '../../vue/stores/authStore';
import { loadChat } from '../../api/chatLoader';
import { ChatMessage } from '../../types/chatMessage';
import { useChatStore } from '../stores/chatStore';

const authStore = useAuthStore();
const chatStore = useChatStore();
const newMessage = ref('');

const username = computed(() => {
    return authStore.player.name || 'Anon';
});

function onReceiveMessage(message: ChatMessage) {
    chatStore.messages.push(message);
}

async function sendMessage() {
    if (!newMessage.value.trim()) return;
    await signalrProvider.start();
    await signalrProvider.getConnection().invoke('SendChatMessage', username.value, newMessage.value);
    newMessage.value = '';
}

onMounted(async () => {
    signalrProvider.getConnection().on('ReceiveChatMessage', onReceiveMessage);
    await loadChat();
    // await signalrProvider.start();
});

onUnmounted(() => {
    signalrProvider.getConnection().off('ReceiveChatMessage', onReceiveMessage);
});
</script>

<template>
    <div class="chat-panel">
        <div class="chat-header">Chat</div>
        <div class="chat-messages">
            <div v-for="(msg, index) in chatStore.messages" :key="index" class="chat-message">
                <span class="chat-user">{{ msg.senderName }}:</span>
                <span class="chat-text">{{ msg.content }}</span>
            </div>
            <div v-if="chatStore.messages.length === 0" class="chat-empty">No messages yet</div>
        </div>
        <div class="chat-input">
            <input
                v-model="newMessage"
                type="text"
                placeholder="Type a message..."
                @keyup.enter="sendMessage"
            />
            <button @click="sendMessage">Send</button>
        </div>
    </div>
</template>

<style scoped>
.chat-panel {
    position: fixed;
    top: 20px;
    right: 20px;
    width: 320px;
    height: 400px;
    display: flex;
    flex-direction: column;
    background: rgba(20, 20, 40, 0.7);
    backdrop-filter: blur(12px);
    border: 1px solid rgba(100, 100, 255, 0.2);
    border-radius: 16px;
    overflow: hidden;
    z-index: 9;
    pointer-events: auto;
}

.chat-header {
    padding: 12px 16px;
    background: rgba(30, 30, 60, 0.8);
    color: #e0e0ff;
    font-size: 14px;
    font-weight: 600;
    border-bottom: 1px solid rgba(100, 100, 255, 0.2);
}

.chat-messages {
    flex: 1;
    overflow-y: auto;
    padding: 12px;
    display: flex;
    flex-direction: column;
    gap: 8px;
}

.chat-message {
    color: #e0e0ff;
    font-size: 13px;
    line-height: 1.4;
    word-break: break-word;
}

.chat-user {
    font-weight: 600;
    margin-right: 6px;
    color: #a0a0ff;
}

.chat-text {
    color: #e0e0ff;
}

.chat-empty {
    color: rgba(200, 200, 255, 0.4);
    font-size: 13px;
    text-align: center;
    margin-top: 20px;
}

.chat-input {
    display: flex;
    padding: 8px;
    gap: 8px;
    border-top: 1px solid rgba(100, 100, 255, 0.2);
    background: rgba(30, 30, 60, 0.8);
}

.chat-input input {
    flex: 1;
    padding: 8px 12px;
    border: 1px solid rgba(100, 100, 255, 0.3);
    border-radius: 8px;
    background: rgba(20, 20, 40, 0.8);
    color: #e0e0ff;
    font-size: 13px;
    outline: none;
}

.chat-input input::placeholder {
    color: rgba(200, 200, 255, 0.4);
}

.chat-input button {
    padding: 8px 16px;
    border: 1px solid rgba(100, 100, 255, 0.3);
    border-radius: 8px;
    background: linear-gradient(135deg, rgba(80, 80, 140, 0.6), rgba(60, 60, 120, 0.8));
    color: #e0e0ff;
    font-size: 13px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.25s ease;
}

.chat-input button:hover {
    background: linear-gradient(135deg, rgba(100, 100, 180, 0.7), rgba(80, 80, 160, 0.9));
    border-color: rgba(130, 130, 255, 0.5);
}

.chat-input button:active {
    transform: scale(0.95);
}
</style>
