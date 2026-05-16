<script setup lang="ts">
import { ref, onMounted, computed, onUnmounted, watch } from 'vue';
import { useStarInfoStore } from '../stores/starInfoStore';
import { useSpaceshipStore } from '../stores/spaceshipStore';
import { useSpaceshipActionsStore } from '../stores/spaceshipActionsStore';
import { loadStarVisit, startExploration, harvest } from '../../api/planetLoader';
import { getSpaceshipsOnStar } from '../../api/spaceshipLoader';
import { NamedSpaceship } from '../../types/spaceship';
import { signalrProvider } from '../../api/signalrProvider';
import { createBattle, attackPlayer } from '../../api/battleLoader';

const starInfoStore = useStarInfoStore();
const spaceshipStore = useSpaceshipStore();
const actionsStore = useSpaceshipActionsStore();

const emit = defineEmits<{
    close: [];
}>();

const spaceships = ref<NamedSpaceship[]>([]);

const timeLeft = computed(() => {
    if (!starInfoStore.starInfo.visit.exists || !starInfoStore.starInfo.visit.visitedAt) return null;

    const visitedAt = new Date(starInfoStore.starInfo.visit.visitedAt);
    const expiresAt = new Date(visitedAt.getTime() + 24 * 60 * 60 * 1000);
    const now = new Date();
    const diff = expiresAt.getTime() - now.getTime();

    if (diff <= 0) return null;

    const hours = Math.floor(diff / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
    return `${hours}ч ${minutes}м`;
});

let exploreTimeLeft = ref("");
let exploreTimer: number | null = null;

watch(() => actionsStore.isExploring, (exploring) => {
    if (exploring) {
        updateExploreTimeLeft();
        exploreTimer = window.setInterval(() => { updateExploreTimeLeft() }, 1000);
    } else if (exploreTimer) {
        clearInterval(exploreTimer);
        exploreTimer = null;
    }
}, { immediate: true });

function updateExploreTimeLeft() {
    if (!actionsStore.isExploring) return null;

    const diff = actionsStore.ExploringUntil.getTime() - new Date().getTime();
    if (diff <= 0) return null;

    const seconds = Math.ceil(diff / 1000);
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    exploreTimeLeft.value = minutes > 0 ? `${minutes}м ${remainingSeconds}с` : `${remainingSeconds}с`;
}

const isVisitExpired = computed(() => {
    if (!starInfoStore.starInfo.visit.exists || !starInfoStore.starInfo.visit.visitedAt) return true;

    const visitedAt = new Date(starInfoStore.starInfo.visit.visitedAt);
    const expiresAt = new Date(visitedAt.getTime() + 24 * 60 * 60 * 1000);
    return new Date() > expiresAt;
});

function OnMove() {
    getSpaceshipsOnStar().then(s => spaceships.value = s);
}

onMounted(async () => {
    signalrProvider.getConnection().on('Loc_IncomeShip', OnMove);
    await loadStarVisit();
    spaceships.value = await getSpaceshipsOnStar();
});

onUnmounted(() => {
    signalrProvider.getConnection().off('Loc_IncomeShip', OnMove);
});

const canExplore = computed(() => {
    const type = starInfoStore.starInfo.star.type;
    return !type || type === '-' || type === '';
});

const handleCreateVisit = async () => {
    await startExploration();
};

const handleStartBattle = async () => {
    await createBattle();
    emit('close');
};

const playerBattlePower = computed(() => {
    const playerShip = spaceships.value.find(s => s.id === spaceshipStore.spaceship.ownerId);
    return playerShip?.battlePower ?? 0;
});

const handleAttack = async (targetOwnerId: string) => {
    await attackPlayer(targetOwnerId);
    emit('close');
};
</script>

<template>
    <div class="overlay-backdrop" @click.self="emit('close')">
        <div class="overlay-content">
            <div class="overlay-header">
                <h3>📡 Связь с планетой</h3>
                <button class="close-btn" @click="emit('close')">✕</button>
            </div>

            <div class="star-info">
                <div class="info-row">
                    <span class="label">Радиус:</span>
                    <span class="value">{{ starInfoStore.starInfo.star.radius.toFixed(2) }}</span>
                </div>
                <div class="info-row">
                    <span class="label">Угол (мрад):</span>
                    <span class="value">{{ starInfoStore.starInfo.star.angleMilliradians.toFixed(2) }}</span>
                </div>
                <div v-if="actionsStore.isExploring" class="info-row exploring-row">
                    <span class="label">Процесс изучения:</span>
                    <span class="value exploring-value">{{ exploreTimeLeft }}</span>
                </div>
                <div v-if="starInfoStore.starInfo.visit.exists && !isVisitExpired" class="info-row visit-active">
                    <span class="label">Вы тут уже были:</span>
                    <span class="value">{{ timeLeft }}</span>
                </div>
                <div v-else-if="!actionsStore.isExploring && !actionsStore.explorationStatus && canExplore" class="visit-inactive">
                    <span>Изучить планету:</span>
                    <button class="visit-btn" @click="handleCreateVisit">Начать экспедицию</button>
                </div>
                <div v-else-if="!canExplore && !actionsStore.isExploring" class="visit-inactive">
                    <span>Эта звезда не подходит для исследования</span>
                </div>
            </div>

            <div v-if="actionsStore.explorationStatus" class="exploration-result">
                <h4>Результат исследования</h4>
                <div v-if="actionsStore.explorationStatus.enemy" class="enemy-info">
                    <span class="enemy-icon">⚔️</span>
                    <span class="enemy-name">{{ actionsStore.explorationStatus.enemy.name }}</span>
                    <span class="enemy-level">Уровень {{ actionsStore.explorationStatus.enemy.level }}</span>
                    <span v-if="actionsStore.explorationStatus.enemy.alive" class="enemy-alive">
                        <button class="battle-btn" @click="handleStartBattle">⚔️ Бой!</button>
                    </span>
                    <span v-else class="enemy-dead">Побежден</span>
                </div>
                <div v-if="actionsStore.explorationStatus.enemy == null || !actionsStore.explorationStatus.enemy.alive" class="no-enemy">
                    <span>Ничего опасного не обнаружено!</span>
                    <button class="harvest-btn" @click="harvest">Добыть ресурсы</button>
                </div>
            </div>

            <div class="spaceships-section">
                <h4>Корабли в системе</h4>
                <div v-if="spaceships.length === 0" class="empty">Нет кораблей</div>
                <div v-else class="spaceship-list">
                    <div v-for="(ship, index) in spaceships" :key="index" class="spaceship-item">
                        <span class="ship-icon">🚀</span>
                        <div class="ship-info">
                            <span v-if="ship.id === spaceshipStore.spaceship.ownerId" class="ship-name">Ваш
                                корабль</span>
                            <span v-else class="ship-name">{{ ship.name }}</span>
                            <span class="ship-power">⚡ {{ ship.battlePower }}</span>
                        </div>
                        <button v-if="ship.id !== spaceshipStore.spaceship.ownerId && playerBattlePower > 0 &&
                            ship.battlePower >= Math.floor(playerBattlePower * 0.8) &&
                            ship.battlePower <= Math.ceil(playerBattlePower * 1.25)"
                            class="attack-btn" @click="handleAttack(ship.id)">⚔️ Атака</button>
                    </div>
                </div>
            </div>

        </div>
    </div>
</template>

<style scoped>
.overlay-backdrop {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.7);
    backdrop-filter: blur(4px);
    z-index: 100;
    display: flex;
    justify-content: center;
    align-items: center;
}

.overlay-content {
    background: rgba(25, 25, 50, 0.95);
    border: 2px solid rgba(140, 100, 255, 0.5);
    border-radius: 20px;
    width: 500px;
    max-height: 600px;
    display: flex;
    flex-direction: column;
    box-shadow: 0 20px 60px rgba(0, 0, 0, 0.6);
}

.star-info {
    padding: 16px 24px;
    background: rgba(40, 40, 80, 0.5);
    border-bottom: 1px solid rgba(140, 100, 255, 0.3);
    display: flex;
    flex-direction: column;
    gap: 8px;
}

.info-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 14px;
}

.info-row .label {
    color: #889;
    font-size: 12px;
    text-transform: uppercase;
    letter-spacing: 1px;
}

.info-row .value {
    color: #e0e0ff;
    font-weight: 600;
    font-family: 'Courier New', monospace;
}

.visit-active .value {
    color: #8f8;
}

.exploration-row {
    background: rgba(20, 60, 40, 0.5);
    border-radius: 8px;
    padding: 8px 12px;
    margin-bottom: 4px;
}

.exploration-result {
    padding: 16px 24px;
    background: rgba(60, 40, 80, 0.5);
    border-top: 1px solid rgba(140, 100, 255, 0.3);
}

.exploration-result h4 {
    color: #e0e0ff;
    margin: 0 0 12px 0;
    font-size: 16px;
}

.enemy-info {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 12px 16px;
    background: rgba(80, 40, 40, 0.5);
    border: 1px solid rgba(255, 100, 100, 0.4);
    border-radius: 10px;
}

.enemy-icon {
    font-size: 24px;
}

.enemy-name {
    color: #ff8080;
    font-weight: 600;
    font-size: 15px;
}

.enemy-level {
    color: #aaa;
    font-size: 13px;
}

.enemy-alive {
    color: #ff6b6b;
    font-weight: 600;
    font-size: 12px;
    text-transform: uppercase;
    letter-spacing: 1px;
    margin-left: auto;
}

.battle-btn {
    cursor: pointer;
    background: linear-gradient(135deg, rgba(200, 60, 60, 0.9), rgba(220, 80, 80, 1));
    border: 2px solid rgba(255, 120, 120, 0.6);
    border-radius: 8px;
    padding: 8px 16px;
    color: #fff;
    font-weight: 700;
    font-size: 14px;
    transition: all 0.2s;
}

.battle-btn:hover {
    background: linear-gradient(135deg, rgba(220, 80, 80, 1), rgba(240, 100, 100, 1));
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(255, 80, 80, 0.5);
}

.enemy-dead {
    color: #6bff6b;
    font-weight: 600;
    font-size: 12px;
    text-transform: uppercase;
    letter-spacing: 1px;
    margin-left: auto;
}

.no-enemy {
    padding: 12px 16px;
    background: rgba(40, 80, 40, 0.5);
    border: 1px solid rgba(100, 255, 100, 0.3);
    border-radius: 10px;
    color: #8f8;
    text-align: center;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 12px;
}

.harvest-btn {
    cursor: pointer;
    background: linear-gradient(135deg, rgba(80, 160, 80, 0.8), rgba(100, 180, 100, 0.9));
    border: 1px solid rgba(100, 255, 100, 0.5);
    border-radius: 8px;
    padding: 8px 20px;
    color: #fff;
    font-weight: 600;
    font-size: 14px;
    transition: all 0.2s;
}

.harvest-btn:hover {
    background: linear-gradient(135deg, rgba(100, 180, 100, 0.9), rgba(120, 200, 120, 1));
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(80, 200, 80, 0.4);
}

.attack-btn {
    cursor: pointer;
    background: linear-gradient(135deg, rgba(200, 60, 60, 0.9), rgba(220, 80, 80, 1));
    border: 2px solid rgba(255, 120, 120, 0.6);
    border-radius: 8px;
    padding: 6px 14px;
    color: #fff;
    font-weight: 700;
    font-size: 12px;
    margin-left: 8px;
    transition: all 0.2s;
    flex-shrink: 0;
}

.attack-btn:hover {
    background: linear-gradient(135deg, rgba(220, 80, 80, 1), rgba(240, 100, 100, 1));
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(255, 80, 80, 0.5);
}

.exploring-value {
    color: #8f8;
    text-shadow: 0 0 8px rgba(100, 255, 150, 0.5);
}

.visit-inactive {
    display: flex;
    justify-content: space-between;
    align-items: center;
    color: #aab;
    font-size: 13px;
}

.visit-btn {
    cursor: pointer;
    background: linear-gradient(135deg, rgba(60, 180, 60, 0.8), rgba(80, 200, 80, 0.9));
    border: 1px solid rgba(100, 255, 100, 0.5);
    border-radius: 8px;
    padding: 8px 16px;
    color: #fff;
    font-weight: 600;
    font-size: 13px;
    transition: all 0.2s;
}

.visit-btn:hover {
    background: linear-gradient(135deg, rgba(80, 200, 80, 0.9), rgba(100, 220, 100, 1));
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(80, 200, 80, 0.4);
}

.spaceships-section {
    padding: 16px 24px;
    border-top: 1px solid rgba(140, 100, 255, 0.3);
}

.spaceships-section h4 {
    color: #e0e0ff;
    margin: 0 0 12px 0;
    font-size: 16px;
}

.spaceship-list {
    display: flex;
    flex-direction: column;
    gap: 8px;
}

.spaceship-item {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 8px 12px;
    background: rgba(40, 40, 80, 0.5);
    border: 1px solid rgba(140, 100, 255, 0.3);
    border-radius: 10px;
    color: #d0d0ff;
}

.ship-icon {
    font-size: 18px;
}

.ship-info {
    display: flex;
    align-items: center;
    gap: 12px;
    flex: 1;
}

.ship-name {
    font-family: 'Courier New', monospace;
    font-size: 13px;
    color: #e0e0ff;
}

.ship-power {
    margin-left: auto;
    font-size: 12px;
    color: #ffd700;
}

.empty {
    color: #889;
    font-size: 13px;
    text-align: center;
    padding: 8px;
}

.overlay-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 20px 24px;
    border-bottom: 1px solid rgba(140, 100, 255, 0.3);
}

.overlay-header h3 {
    color: #e0e0ff;
    margin: 0;
    font-size: 20px;
}

.close-btn {
    background: none;
    border: none;
    color: #aab;
    font-size: 24px;
    cursor: pointer;
    padding: 4px 8px;
    border-radius: 8px;
    transition: all 0.2s;
}

.close-btn:hover {
    background: rgba(255, 100, 100, 0.2);
    color: #ff6b6b;
}

</style>
