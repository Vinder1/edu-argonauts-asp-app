import { fetchWithAuth } from "./fetchWithAuth";

export interface QuestData {
    level: number;
    killed: number;
    killsRequired: number;
    isCompleted: boolean;
    description: string;
}

export async function loadQuest(): Promise<QuestData | null> {
    try {
        let response = await fetchWithAuth('/api/quest/');
        if (!response.ok) return null;
        return await response.json() as QuestData;
    } catch (error: unknown) {
        console.error('Ошибка загрузки квеста:', error);
        return null;
    }
}
