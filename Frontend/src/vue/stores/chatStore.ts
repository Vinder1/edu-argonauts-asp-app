import { defineStore } from "pinia";
import { ChatMessage } from "../../types/chatMessage";
import { ref } from "vue";

export const useChatStore = defineStore('chat', () => {
    const messages = ref<ChatMessage[]>([]);

    return { messages };
});