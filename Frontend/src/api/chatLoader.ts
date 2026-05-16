import { useChatStore } from "../vue/stores/chatStore";
import { fetchWithAuth } from "./fetchWithAuth";

export async function loadChat() {
    try {
        const response = await fetchWithAuth('/api/chat/get-10');
        if (response.ok) {
            const data = await response.json();
            useChatStore().messages = data.messages;
        }
    } catch { }
}