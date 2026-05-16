<script setup lang="ts">
import { ref, watch, onUnmounted, onMounted } from 'vue';
import { useSpaceshipActionsStore } from '../stores/spaceshipActionsStore';
import { loadActionState, loadExplorationStatus, loadMovementStatus } from '../../api/actionStateLoader';

const actionsStore = useSpaceshipActionsStore();

onMounted(async () => {
    await loadActionState();
    await loadExplorationStatus();
    await loadMovementStatus();
});

let movementTimer: number | null = null;
let explorationTimer: number | null = null;
let movementTimeLeft = ref("");
let explorationTimeLeft = ref("");
let animationChar = ref('|');
let animationInterval: number | null = null;

const animChars = ['|', '/', '-', '\\'];

watch(() => actionsStore.isMoving, (moving) => {
    if (moving) {
        updateMovementTimeLeft();
        movementTimer = window.setInterval(() => { updateMovementTimeLeft() }, 1000);
    } else if (movementTimer) {
        clearInterval(movementTimer);
        movementTimer = null;
    }
}, { immediate: true });

watch(() => actionsStore.isExploring, (exploring) => {
    if (exploring) {
        updateExplorationTimeLeft();
        explorationTimer = window.setInterval(() => { updateExplorationTimeLeft() }, 1000);
    } else if (explorationTimer) {
        clearInterval(explorationTimer);
        explorationTimer = null;
    }
}, { immediate: true });

onUnmounted(() => {
    if (movementTimer) clearInterval(movementTimer);
    if (explorationTimer) clearInterval(explorationTimer);
    stopAnimation();
});

function updateMovementTimeLeft() {
    if (!actionsStore.isMoving || !actionsStore.movementStatus) {
        movementTimeLeft.value = '';
        return;
    }
    const diff = new Date(actionsStore.movementStatus.ends).getTime() - new Date().getTime();
    checkAnimationState(movementTimeLeft, diff);
}

function updateExplorationTimeLeft() {
    if (!actionsStore.isExploring) {
        explorationTimeLeft.value = '';
        return;
    }
    const diff = actionsStore.ExploringUntil.getTime() - new Date().getTime();
    checkAnimationState(explorationTimeLeft, diff);
}

function checkAnimationState(timeLeftRef: typeof movementTimeLeft, diff: number) {
    if (diff <= 0) {
        timeLeftRef.value = '0s';
        startAnimation(() => onAnimationComplete(timeLeftRef));
    } else {
        const seconds = Math.ceil(diff / 1000);
        const minutes = Math.floor(seconds / 60);
        const remainingSeconds = seconds % 60;
        timeLeftRef.value = minutes > 0 ? `${minutes}m ${remainingSeconds}s` : `${remainingSeconds}s`;
    }
}

function startAnimation(onComplete: () => void) {
    if (animationInterval) return;
    let index = 0;
    animationChar.value = animChars[index];
    animationInterval = window.setInterval(() => {
        index = (index + 1) % animChars.length;
        animationChar.value = animChars[index];
    }, 100);
    setTimeout(() => {
        stopAnimation();
        onComplete();
    }, 2000);
}

function stopAnimation() {
    if (animationInterval) {
        clearInterval(animationInterval);
        animationInterval = null;
    }
    animationChar.value = '|';
}

function onAnimationComplete(timeLeftRef: typeof movementTimeLeft) {
    timeLeftRef.value = '';
}
</script>

<template>
    <div v-if="actionsStore.isMoving || actionsStore.isExploring" class="timer-panel">
        <div v-if="actionsStore.isMoving" class="timer movement-timer">
            <span v-if="movementTimeLeft === '0s'" class="animation-char">{{ animationChar }}</span>
            <span v-else>До прибытия: {{ movementTimeLeft }}
                <span v-if="actionsStore.movementStatus" class="movement-info">
                    (Радиус: {{ actionsStore.movementStatus.from.radius }}->{{ actionsStore.movementStatus.to.radius }})
                </span>
            </span>
        </div>
        <div v-if="actionsStore.isExploring" class="timer exploration-timer">
            <span v-if="explorationTimeLeft === '0s'" class="animation-char">{{ animationChar }}</span>
            <span v-else>Исследование: {{ explorationTimeLeft }}</span>
        </div>
    </div>
</template>

<style scoped>
.timer-panel {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 8px;
    padding: 12px 20px;
    background: rgba(20, 20, 40, 0.9);
    backdrop-filter: blur(12px);
    border: 1px solid rgba(100, 100, 255, 0.3);
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.4);
    min-width: 600px;
}

.timer {
    font-size: 15px;
    font-weight: 700;
    padding: 8px 20px;
    border-radius: 8px;
    white-space: nowrap;
}

.movement-timer {
    color: #d0d0ff;
    text-shadow: 0 0 10px rgba(150, 150, 255, 0.8);
    background: rgba(40, 40, 80, 0.8);
    border: 1px solid rgba(150, 150, 255, 0.4);
}

.movement-info {
    font-size: 12px;
    opacity: 0.7;
    margin-left: 4px;
}

.exploration-timer {
    color: #b0ffb0;
    text-shadow: 0 0 10px rgba(100, 255, 150, 0.8);
    background: rgba(20, 60, 40, 0.8);
    border: 1px solid rgba(100, 255, 150, 0.4);
}

.animation-char {
    display: inline-block;
    min-width: 24px;
    text-align: center;
}
</style>
