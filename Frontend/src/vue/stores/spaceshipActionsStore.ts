import { defineStore } from 'pinia';
import { ref } from 'vue';
import { ExplorationStatus } from '../../types/explorationStatus';
import { MovementStatus } from '../../types/movementStatus';
import { BattleMember } from '../../types/battleStatus';

export const useSpaceshipActionsStore = defineStore('spaceshipActions', () => {
    const isMoving = ref(false);
    const isExploring = ref(false);
    const isBattling = ref(false);
    const ExploringUntil = ref(new Date());
    const explorationStatus = ref<ExplorationStatus | null>(null);
    const movementStatus = ref<MovementStatus | null>(null);
    const battleMembers = ref<BattleMember[]>([]);
    const playerMember = ref<BattleMember | null>(null);

    function reset() {
        isMoving.value = false;
        isExploring.value = false;
        isBattling.value = false;
        ExploringUntil.value = new Date();
        explorationStatus.value = null;
        movementStatus.value = null;
        battleMembers.value = [];
        playerMember.value = null;
    }

    return { isMoving, isExploring, isBattling, ExploringUntil, explorationStatus, movementStatus, battleMembers, playerMember, reset };
});
