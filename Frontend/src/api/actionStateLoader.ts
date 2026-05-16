import { ExplorationStatus } from "../types/explorationStatus";
import { MovementStatus } from "../types/movementStatus";
import { useSpaceshipActionsStore } from "../vue/stores/spaceshipActionsStore";
import { fetchWithAuth } from "./fetchWithAuth";

export async function loadActionState() {
    try {
        const actionsStore = useSpaceshipActionsStore();
        const response = await fetchWithAuth('/api/spaceship/state');
        if (response.ok) {
            const data = await response.json();
            if (data === 1) {
                actionsStore.isMoving = true;
            } else if (data === 2) {
                actionsStore.isExploring = true;
            } else if (data === 3) {
                actionsStore.isBattling = true;
            }
        }
    } catch { }
}

export async function loadExplorationStatus() {
    try {
        const actionsStore = useSpaceshipActionsStore();
        const response = await fetchWithAuth('/api/explore/state');
        if (response.ok) {
            const data = await response.json() as ExplorationStatus;
            actionsStore.explorationStatus = data;
        }
    } catch { }
}

export async function loadMovementStatus() {
    try {
        const actionsStore = useSpaceshipActionsStore();
        const response = await fetchWithAuth('/api/spaceship/movement-status');
        if (response.ok) {
            const data = await response.json() as MovementStatus;
            actionsStore.movementStatus = data;
            actionsStore.isMoving = true;
        }
    } catch { }
}