<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue';
import { useAuthStore } from '../stores/authStore';
import { useSpaceshipStore } from '../stores/spaceshipStore';
import { useSpaceshipActionsStore } from '../stores/spaceshipActionsStore';
import { useOverlayVisibilityStore } from '../stores/overlayVisibilityStore';
import { createSpaceship, loadSpaceship, onMoveConfirmed, onMoveStarted } from '../../api/spaceshipLoader';
import { loadBattleState } from '../../api/battleLoader';
import PlanetCommunication from './PlanetCommunication.vue';
import TimerPanel from './TimerPanel.vue';
import { signalrProvider } from '../../api/signalrProvider';
import { onExploreEnd, onExploreStart } from '../../api/planetLoader';
import { ExplorationStatus } from '../../types/explorationStatus';
import { useSpaceshipResourcesStore } from '../stores/spaceshipResourcesStore';
import { useStarInfoStore } from '../stores/starInfoStore';
import { Star } from '../../types/star';
import { Balance } from '../../types/spaceshipResources';
import { loadAllResources } from '../../api/spaceshipResourcesLoader';
import { showNotification } from '../../utils/notifications';

const authStore = useAuthStore();
const spaceshipStore = useSpaceshipStore();
const actionsStore = useSpaceshipActionsStore();
const overlayStore = useOverlayVisibilityStore();
const starInfoStore = useStarInfoStore();
const showPlanetOverlay = ref(false);

const funnyNotes = [
    'Любит забавы жестокий тиран, трону не ведом закон!',
    'Так точно, во имя Корпорации!',
    'Демон, космический демон, пусть навеки он останется сном...',
    'Победа наша!',
    'Храни меня, Корпорация!',
    'Знал, что без потерь прожить нельзя! Знал, и снова делал всё не так!',
    'Лишь вид небосвода столетиям не изменить...',
    'А космос, он точно такой же. Как если б ты не продался...',
    'Моё тело состоит из клеток... Я, походу, сам себе тюрьма.'
];

const randomNote = computed(() => {
    const index = Math.floor(Math.random() * funnyNotes.length);
    return funnyNotes[index];
});

const formatCoordinate = (value: number, decimals: number = 2) => {
    return value.toFixed(decimals);
};

const isAtHub = computed(() => {
    return spaceshipStore.hasSpaceship &&
        starInfoStore.starInfo.star.type === 'hub';
});

const handleCreateSpaceship = async () => {
    await createSpaceship();
};

function OnMove(ownerId: string, radius: number, angle: number) {
    if (ownerId !== spaceshipStore.spaceship.ownerId)
        return;
    onMoveConfirmed(radius, angle);
}

function OnStartMove(ownerId: string, arrivalTime: Date, destination: Star) {
    if (ownerId !== spaceshipStore.spaceship.ownerId)
        return;
    onMoveStarted(arrivalTime, destination);
}

function OnExploreResult(ownerId: string, exploreResult: ExplorationStatus) {
    if (ownerId !== spaceshipStore.spaceship.ownerId)
        return;
    actionsStore.explorationStatus = exploreResult;
    actionsStore.isExploring = false;
}

function OnExploreEnd(ownerId: string, success: boolean, newBalance: Balance, arrivalTime: Date) {
    if (ownerId !== spaceshipStore.spaceship.ownerId)
        return;
    onExploreEnd(success, arrivalTime);

    const balanceStore = useSpaceshipResourcesStore();
    balanceStore.balance = newBalance;

    actionsStore.explorationStatus = null;
}

function OnStartExplore(ownerId: string, arrivalTime: Date) {
    if (ownerId !== spaceshipStore.spaceship.ownerId)
        return;
    onExploreStart(arrivalTime);
}

function OnBattleStart(ownerId: string, battleId: string) {
    if (ownerId !== spaceshipStore.spaceship.ownerId)
        return;
    loadBattleState();
}

function OnBattleEnd(ownerId: string) {
    if (ownerId !== spaceshipStore.spaceship.ownerId)
        return;
    actionsStore.battleMembers = [];
    actionsStore.isBattling = false;
    loadSpaceship();
    loadAllResources();
}

function OnBattleRoundEnd(ownerId: string) {
    if (ownerId !== spaceshipStore.spaceship.ownerId)
        return;
    loadBattleState();
}

function OnDeath(ownerId: string) {
    if (ownerId !== spaceshipStore.spaceship.ownerId)
        return;
    loadSpaceship();
    loadAllResources();
    showNotification('Ваш корабль взорвался. Вы возродились в центре.');
}

onMounted(() => {
    signalrProvider.getConnection().on('User_ConfirmMove', OnMove);
    signalrProvider.getConnection().on('User_StartMove', OnStartMove);
    signalrProvider.getConnection().on('User_StartExplore', OnStartExplore);
    signalrProvider.getConnection().on('User_ExploreResult', OnExploreResult);
    signalrProvider.getConnection().on('User_ExploreEnd', OnExploreEnd);
    signalrProvider.getConnection().on('User_BattleStart', OnBattleStart);
    signalrProvider.getConnection().on('User_BattleEnd', OnBattleEnd);
    signalrProvider.getConnection().on('User_BattleRoundEnd', OnBattleRoundEnd);
    signalrProvider.getConnection().on('User_Death', OnDeath);
});
</script>

<template>
    <div v-if="authStore.isLoggedIn" class="panel-container">
        <TimerPanel />
        <div class="spaceship-panel" :class="{ disabled: actionsStore.isMoving }">
            <template v-if="spaceshipStore.hasSpaceship">
                <div class="panel-left">
                    <div class="coord-label">Радиус</div>
                    <div class="coord-value">{{ formatCoordinate(spaceshipStore.spaceship.locatedRadius) }}</div>
                    <div class="coord-label">Угол (мрад)</div>
                    <div class="coord-value">{{ formatCoordinate(spaceshipStore.spaceship.locatedAngleMilliradians) }}
                    </div>
                </div>

                <div class="panel-center">
                    <button v-if="!isAtHub" class="planet-btn" @click="showPlanetOverlay = true"
                        :disabled="actionsStore.isMoving">
                        📡 Связь с планетой
                    </button>
                    <button v-else class="hub-btn"
                        :class="{ active: overlayStore.isHubPanelVisible }"
                        @click="overlayStore.isHubPanelVisible = !overlayStore.isHubPanelVisible">
                        🏠 Хаб
                    </button>
                </div>

                <div class="panel-right">
                    <div class="funny-note">{{ randomNote }}</div>
                </div>
            </template>

            <template v-else>
                <button class="create-btn" @click="handleCreateSpaceship" :disabled="actionsStore.isMoving">
                    🚀 Создать корабль
                </button>
            </template>

            <div v-if="actionsStore.isMoving" class="loading-overlay">
                <div class="loading-text">Перемещение...</div>
            </div>
        </div>
    </div>

    <PlanetCommunication v-if="showPlanetOverlay" @close="showPlanetOverlay = false" />
</template>

<style scoped>
.panel-container {
    position: fixed;
    bottom: 90px;
    left: 50%;
    transform: translateX(-50%);
    z-index: 100;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 10px;
}

.spaceship-panel {
    display: flex;
    align-items: center;
    gap: 24px;
    padding: 16px 24px;
    background: rgba(20, 20, 40, 0.85);
    backdrop-filter: blur(12px);
    border: 1px solid rgba(100, 100, 255, 0.3);
    border-radius: 16px;
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.4), inset 0 1px 0 rgba(255, 255, 255, 0.1);
    min-width: 600px;
}

.panel-left {
    display: flex;
    flex-direction: column;
    gap: 4px;
    min-width: 140px;
}

.coord-label {
    color: #889;
    font-size: 11px;
    text-transform: uppercase;
    letter-spacing: 1px;
}

.coord-value {
    color: #e0e0ff;
    font-size: 18px;
    font-weight: 700;
    font-family: 'Courier New', monospace;
}

.panel-center {
    flex: 1;
    display: flex;
    justify-content: center;
}

.planet-btn {
    cursor: pointer;
    color: #fff;
    background: linear-gradient(135deg, rgba(100, 80, 200, 0.8), rgba(140, 100, 255, 0.9));
    border: 2px solid rgba(160, 140, 255, 0.5);
    border-radius: 12px;
    padding: 14px 28px;
    font-size: 16px;
    font-weight: 700;
    letter-spacing: 1px;
    transition: all 0.3s ease;
    box-shadow: 0 4px 20px rgba(140, 100, 255, 0.4);
    white-space: nowrap;
}

.planet-btn:hover:not(:disabled) {
    background: linear-gradient(135deg, rgba(120, 100, 220, 0.9), rgba(160, 120, 255, 1));
    border-color: rgba(180, 160, 255, 0.7);
    transform: translateY(-2px);
    box-shadow: 0 6px 28px rgba(140, 100, 255, 0.6);
}

.planet-btn:active:not(:disabled) {
    transform: translateY(0);
}

.planet-btn:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}

.hub-btn {
    cursor: pointer;
    color: #fff;
    background: linear-gradient(135deg, rgba(60, 160, 200, 0.8), rgba(80, 180, 220, 0.9));
    border: 2px solid rgba(100, 200, 255, 0.5);
    border-radius: 12px;
    padding: 14px 28px;
    font-size: 16px;
    font-weight: 700;
    letter-spacing: 1px;
    transition: all 0.3s ease;
    box-shadow: 0 4px 20px rgba(100, 200, 255, 0.4);
    white-space: nowrap;
}

.hub-btn:hover {
    background: linear-gradient(135deg, rgba(80, 180, 220, 0.9), rgba(100, 200, 240, 1));
    border-color: rgba(120, 220, 255, 0.7);
    transform: translateY(-2px);
    box-shadow: 0 6px 28px rgba(100, 200, 255, 0.6);
}

.hub-btn:active {
    transform: translateY(0);
}

.hub-btn.active {
    background: linear-gradient(135deg, rgba(40, 120, 180, 0.9), rgba(60, 140, 200, 1));
    border-color: rgba(100, 200, 255, 0.8);
    box-shadow: 0 0 20px rgba(100, 200, 255, 0.6);
}

.panel-right {
    min-width: 180px;
    display: flex;
    align-items: center;
    justify-content: flex-end;
}

.funny-note {
    color: #aab;
    font-size: 13px;
    font-style: italic;
    text-align: right;
    line-height: 1.4;
    max-width: 200px;
}

.create-btn {
    cursor: pointer;
    color: #fff;
    background: linear-gradient(135deg, rgba(60, 180, 60, 0.8), rgba(80, 200, 80, 0.9));
    border: 2px solid rgba(100, 255, 100, 0.5);
    border-radius: 12px;
    padding: 14px 32px;
    font-size: 18px;
    font-weight: 700;
    letter-spacing: 1px;
    transition: all 0.3s ease;
    box-shadow: 0 4px 20px rgba(80, 200, 80, 0.4);
    white-space: nowrap;
}

.create-btn:hover:not(:disabled) {
    background: linear-gradient(135deg, rgba(80, 200, 80, 0.9), rgba(100, 220, 100, 1));
    border-color: rgba(120, 255, 120, 0.7);
    transform: translateY(-2px);
    box-shadow: 0 6px 28px rgba(80, 200, 80, 0.6);
}

.create-btn:active:not(:disabled) {
    transform: translateY(0);
}

.create-btn:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}

.disabled {
    opacity: 0.5;
    pointer-events: none;
}

.loading-overlay {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    background: rgba(0, 0, 0, 0.9);
    border-radius: 16px;
    z-index: 5;
}

.loading-text {
    color: #aaf;
    font-size: 16px;
    font-weight: 700;
    text-shadow: 0 0 10px rgba(100, 100, 255, 0.8);
}
</style>
