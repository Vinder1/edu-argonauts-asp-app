import { BattleMember } from "../types/battleStatus";
import { fetchWithAuth } from "./fetchWithAuth";
import { useSpaceshipActionsStore } from "../vue/stores/spaceshipActionsStore";
import { useAuthStore } from "../vue/stores/authStore";

export async function loadBattleState() {
    try {
        const actionsStore = useSpaceshipActionsStore();
        const response = await fetchWithAuth('/api/battle/state');
        if (response.ok) {
            const data = await response.json() as BattleMember[];
            const playerId = useAuthStore().player.id;
            actionsStore.battleMembers = data.filter(m => m.id != playerId);
            actionsStore.isBattling = data.length > 0;
            const userFighter = data.find(f => f.id == useAuthStore().player.id);
            actionsStore.playerMember = userFighter || null;
        } else if (response.status === 404) {
            actionsStore.battleMembers = [];
            actionsStore.isBattling = false;
        }
    } catch { }
}

export async function attackPlayer(targetOwnerId: string) {
    try {
        const response = await fetchWithAuth(`/api/battle/attack-player?targetOwnerId=${targetOwnerId}`, {
            method: 'POST'
        });
        if (response.ok) {
            await loadBattleState();
        } else {
            const data = await response.json();
            console.debug(data.error);
        }
    } catch { }
}

export async function createBattle() {
    try {
        const response = await fetchWithAuth('/api/battle/create', {
            method: 'POST'
        });
        if (response.ok) {
            await loadBattleState();
        } else {
            const data = await response.json();
            console.debug(data.error);
        }
    } catch { }
}

export async function updateMemberMove(move: string, targetId?: string) {
    try {
        const url = targetId 
            ? `/api/battle/move?&move=${move}&targetId=${targetId}` 
            : `/api/battle/move?&move=${move}`;
        await fetchWithAuth(url, {
            method: 'POST'
        });
    } catch { }
}

export async function endBattle() {
    try {
        await fetchWithAuth('/api/battle/end', {
            method: 'POST'
        });
        const actionsStore = useSpaceshipActionsStore();
        actionsStore.battleMembers = [];
        actionsStore.isBattling = false;
    } catch { }
}