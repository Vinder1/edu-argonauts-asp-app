<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { useSpaceshipResourcesStore } from '../stores/spaceshipResourcesStore';
import { useSpaceshipActionsStore } from '../stores/spaceshipActionsStore';
import { useOverlayVisibilityStore } from '../stores/overlayVisibilityStore';
import { useAuthStore } from '../stores/authStore';

const resourcesStore = useSpaceshipResourcesStore();
const actionsStore = useSpaceshipActionsStore();
const overlayStore = useOverlayVisibilityStore();
const authStore = useAuthStore();

const leftPanelRight = ref('50%');
const rightPanelLeft = ref('50%');

function toggleBattlePanel() {
    overlayStore.isBattlePanelVisible = !overlayStore.isBattlePanelVisible;
}

function toggleTopSpaceshipsPanel() {
    overlayStore.isTopSpaceshipsPanelVisible = !overlayStore.isTopSpaceshipsPanelVisible;
}

function updatePanelPositions() {
    const controls = document.querySelector('.controls') as HTMLElement;
    if (!controls) return;

    const controlsRect = controls.getBoundingClientRect();
    const gap = 16;

    leftPanelRight.value = (window.innerWidth - controlsRect.left + gap) + 'px';
    rightPanelLeft.value = (controlsRect.right + gap) + 'px';
}

onMounted(() => {
    setTimeout(updatePanelPositions, 100);
    window.addEventListener('resize', updatePanelPositions);
});

onUnmounted(() => {
    window.removeEventListener('resize', updatePanelPositions);
});
</script>

<template>
    <div v-if="authStore.isLoggedIn">
        <div class="balance-above-panel">
            <div class="balance-indicator">
                <span class="currency">{{ resourcesStore.balance.currency }}</span>
                <span class="quants">{{ resourcesStore.balance.quants }}</span>
            </div>
        </div>
        <div class="side-panel left-panel" :style="{ right: leftPanelRight }">
            <button class="round-button guide-btn"
                :class="{ active: overlayStore.isGuidePanelVisible }"
                @click="overlayStore.isGuidePanelVisible = !overlayStore.isGuidePanelVisible" title="Гайд">
                📖
            </button>
            <button class="round-button quest-btn"
                :class="{ active: overlayStore.isQuestPanelVisible }"
                @click="overlayStore.isQuestPanelVisible = !overlayStore.isQuestPanelVisible" title="Квест">
                🎯
            </button>
        </div>
        <div class="condition-above-panel" :style="{ left: rightPanelLeft }">
            <div class="condition-indicator">
                <span class="durability">{{ resourcesStore.condition.durability }}/{{
                    resourcesStore.condition.maxDurability }}</span>
                <span class="energy">{{ resourcesStore.condition.energy }}/{{ resourcesStore.condition.maxEnergy
                    }}</span>
                <span class="antimatter">{{ resourcesStore.condition.antimatter }}/{{
                    resourcesStore.condition.maxAntimatter }}</span>
                <span class="power">PWR: {{ resourcesStore.condition.power }}</span>
                <span class="max-distance">D: {{ resourcesStore.condition.maxDistance }}</span>
                <span class="speed">SPD: {{ resourcesStore.condition.speed }}</span>
            </div>
        </div>
        <div class="side-panel right-panel" :style="{ left: rightPanelLeft }">
            <button class="round-button battle-btn"
                :class="{ active: actionsStore.isBattling && overlayStore.isBattlePanelVisible }"
                :disabled="!actionsStore.isBattling" @click="toggleBattlePanel" title="Бой">
                ⚔️
            </button>
            <button class="round-button top-btn"
                :class="{ active: overlayStore.isTopSpaceshipsPanelVisible }"
                @click="toggleTopSpaceshipsPanel" title="Top 10">
                🏆
            </button>
        </div>
    </div>
</template>

<style scoped>
.balance-above-panel {
    position: fixed;
    left: 400px;
    bottom: 130px;
    display: flex;
    align-items: center;
    padding: 8px 16px;
    background: rgba(20, 20, 40, 0.7);
    backdrop-filter: blur(12px);
    border: 1px solid rgba(100, 100, 255, 0.2);
    border-radius: 12px;
    z-index: 10;
    pointer-events: auto;
}

.condition-above-panel {
    position: fixed;
    bottom: 130px;
    display: flex;
    align-items: center;
    padding: 8px 16px;
    background: rgba(20, 20, 40, 0.7);
    backdrop-filter: blur(12px);
    border: 1px solid rgba(100, 100, 255, 0.2);
    border-radius: 12px;
    z-index: 10;
    pointer-events: auto;
}

.condition-indicator {
    display: flex;
    gap: 12px;
    color: #e0e0ff;
    font-size: 14px;
    font-weight: 600;
}

.durability::before {
    content: '❤️ ';
}

.energy::before {
    content: '⚡ ';
}

.antimatter::before {
    content: '☢️ ';
}

.power {
    color: #ffaa00;
}

.power::before {
    content: '⚔️ ';
}

.max-distance {
    color: #66ccff;
}

.max-distance::before {
    content: '📡 ';
}

.speed {
    color: #88ff88;
}

.speed::before {
    content: '🚀 ';
}

.balance-indicator {
    display: flex;
    gap: 12px;
    color: #e0e0ff;
    font-size: 14px;
    font-weight: 600;
}

.currency::before {
    content: '💰 ';
}

.quants::before {
    content: 'Q ';
}

.side-panel {
    position: fixed;
    bottom: 20px;
    height: 80px;
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 12px;
    padding: 12px 16px;
    background: rgba(20, 20, 40, 0.7);
    backdrop-filter: blur(12px);
    border: 1px solid rgba(100, 100, 255, 0.2);
    border-radius: 16px;
    z-index: 9;
    pointer-events: auto;
}

.left-panel {
    left: 20px;
}

.right-panel {
    right: 20px;
}

.round-button {
    width: 56px;
    height: 56px;
    border-radius: 50%;
    border: 2px solid rgba(100, 100, 255, 0.3);
    background: linear-gradient(135deg, rgba(80, 80, 140, 0.6), rgba(60, 60, 120, 0.8));
    color: #e0e0ff;
    font-size: 18px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.25s ease;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
    backdrop-filter: blur(12px);
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0;
}

.round-button:hover {
    transform: scale(1.1);
    border-color: rgba(130, 130, 255, 0.5);
    background: linear-gradient(135deg, rgba(100, 100, 180, 0.7), rgba(80, 80, 160, 0.9));
    box-shadow: 0 6px 20px rgba(100, 100, 255, 0.3);
}

.round-button:active {
    transform: scale(0.95);
}

.round-button.guide-btn {
    font-size: 20px;
}

.round-button.guide-btn.active {
    background: linear-gradient(135deg, rgba(140, 80, 180, 0.7), rgba(120, 60, 160, 0.9));
    border-color: rgba(180, 100, 255, 0.6);
    box-shadow: 0 0 15px rgba(180, 100, 255, 0.4);
}

.round-button.quest-btn {
    font-size: 20px;
}

.round-button.quest-btn.active {
    background: linear-gradient(135deg, rgba(180, 140, 50, 0.7), rgba(160, 120, 30, 0.9));
    border-color: rgba(255, 180, 50, 0.6);
    box-shadow: 0 0 15px rgba(255, 180, 50, 0.4);
}

.round-button.battle-btn {
    font-size: 22px;
}

.round-button.top-btn {
    font-size: 22px;
}

.round-button.top-btn.active {
    background: linear-gradient(135deg, rgba(180, 160, 60, 0.7), rgba(140, 120, 40, 0.9));
    border-color: rgba(255, 215, 0, 0.6);
    box-shadow: 0 0 15px rgba(255, 215, 0, 0.4);
}

.round-button.battle-btn.active {
    background: linear-gradient(135deg, rgba(180, 60, 60, 0.7), rgba(140, 40, 40, 0.9));
    border-color: rgba(255, 100, 100, 0.6);
    box-shadow: 0 0 15px rgba(255, 80, 80, 0.4);
    animation: pulse 2s infinite;
}

@keyframes pulse {

    0%,
    100% {
        box-shadow: 0 0 15px rgba(255, 80, 80, 0.4);
    }

    50% {
        box-shadow: 0 0 25px rgba(255, 80, 80, 0.6);
    }
}
</style>
