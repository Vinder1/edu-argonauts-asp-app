<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, watch } from 'vue';
import { useSpaceshipActionsStore } from '../stores/spaceshipActionsStore';
import { useOverlayVisibilityStore } from '../stores/overlayVisibilityStore';
import { loadBattleState, updateMemberMove, endBattle } from '../../api/battleLoader';
import { signalrProvider } from '../../api/signalrProvider';
import { useAuthStore } from '../stores/authStore';

const actionsStore = useSpaceshipActionsStore();
const overlayStore = useOverlayVisibilityStore();
const authStore = useAuthStore();

const moves = ['attack', 'defend', 'special'];
const chosenEnemyIndex = ref(0);
const timerSeconds = ref(10);
let timerInterval: ReturnType<typeof setInterval> | null = null;

const enemyMembers = computed(() => actionsStore.battleMembers.filter(m => m.id != authStore.player.id));

const handlePrevEnemy = () => {
    if (enemyMembers.value.length === 0) return;
    chosenEnemyIndex.value = (chosenEnemyIndex.value - 1 + enemyMembers.value.length) % enemyMembers.value.length;
};

const handleNextEnemy = () => {
    if (enemyMembers.value.length === 0) return;
    chosenEnemyIndex.value = (chosenEnemyIndex.value + 1) % enemyMembers.value.length;
};

function startTimer() {
    stopTimer();
    timerSeconds.value = 10;
    timerInterval = setInterval(() => {
        if (timerSeconds.value > 0) {
            timerSeconds.value--;
        }
    }, 1000);
}

function stopTimer() {
    if (timerInterval) {
        clearInterval(timerInterval);
        timerInterval = null;
    }
}

function resetTimer() {
    startTimer();
}

function OnBattleRoundEnd() {
    resetTimer();
    // loadBattleState();
}

onMounted(async () => {
    await loadBattleState();
    signalrProvider.getConnection().on('User_BattleRoundEnd', OnBattleRoundEnd);
});

onUnmounted(() => {
    stopTimer();
    signalrProvider.getConnection().off('User_BattleRoundEnd', OnBattleRoundEnd);
});

watch(() => actionsStore.isBattling, (isBattling) => {
    if (isBattling) {
        startTimer();
    } else {
        stopTimer();
    }
}, { immediate: true });

const playerHealthPercent = computed(() => {
    if (!actionsStore.playerMember) return 0;
    return (actionsStore.playerMember.health / actionsStore.playerMember.maxHealth) * 100;
});

const enemyHealthPercents = computed(() => {
    return enemyMembers.value.map(m => ({
        id: m.id,
        percent: (m.health / m.maxHealth) * 100
    }));
});

const isPlayerDead = computed(() => (actionsStore.playerMember?.health ?? 0) <= 0);

const handleSendMove = async () => {
    if (!actionsStore.playerMember) return;
    await updateMemberMove(actionsStore.playerMember.move, actionsStore.playerMember.targetId);
};

const handleEndBattle = async () => {
    await endBattle();
};
</script>

<template>
    <div v-if="actionsStore.isBattling && overlayStore.isBattlePanelVisible" class="battle-panel">
        <div class="battle-header">
            <span class="battle-title">⚔️ БОЙ!</span>
            <div class="battle-timer" :class="{ warning: timerSeconds <= 3 }">{{ timerSeconds }}с</div>
        </div>

        <div class="battle-arena">
            <div class="enemy-selector">
                <div v-if="enemyMembers.length > 1" class="enemy-preview left">
                    <div class="preview-name">{{ enemyMembers[(chosenEnemyIndex - 1 + enemyMembers.length) % enemyMembers.length]?.name || 'Unknown' }}</div>
                    <div class="preview-health" :style="{ width: (enemyHealthPercents[(chosenEnemyIndex - 1 + enemyMembers.length) % enemyMembers.length]?.percent ?? 0) + '%' }"></div>
                </div>
                
                <button class="nav-btn" @click="handlePrevEnemy" :disabled="enemyMembers.length <= 1 || isPlayerDead">◀</button>
                
                <div v-if="enemyMembers.length > 0" class="enemy-item is-focused">
                    <div class="enemy-name">{{ enemyMembers[chosenEnemyIndex].name || 'Unknown' }}</div>
                    <div class="health-bar-container">
                        <div class="health-bar enemy-health" :style="{ width: (enemyHealthPercents[chosenEnemyIndex]?.percent ?? 0) + '%' }"></div>
                    </div>
                    <div class="health-text">{{ enemyMembers[chosenEnemyIndex].health }} / {{ enemyMembers[chosenEnemyIndex].maxHealth }}</div>
                    <div class="power-text">⚔️ {{ enemyMembers[chosenEnemyIndex].power }}</div>
                    <div class="move-text" v-if="enemyMembers[chosenEnemyIndex].move">Ход: {{ enemyMembers[chosenEnemyIndex].move }}</div>
                    <button class="target-btn" :class="{ selected: actionsStore.playerMember?.targetId === enemyMembers[chosenEnemyIndex].id }"
                            @click="actionsStore.playerMember!.targetId = actionsStore.playerMember!.targetId === enemyMembers[chosenEnemyIndex].id ? '' : enemyMembers[chosenEnemyIndex].id"
                            :disabled="isPlayerDead">
                        {{ actionsStore.playerMember?.targetId === enemyMembers[chosenEnemyIndex].id ? '✓ Выбрано' : 'Выбрать' }}
                    </button>
                </div>
                
                <button class="nav-btn" @click="handleNextEnemy" :disabled="enemyMembers.length <= 1 || isPlayerDead">▶</button>
                
                <div v-if="enemyMembers.length > 1" class="enemy-preview right">
                    <div class="preview-name">{{ enemyMembers[(chosenEnemyIndex + 1) % enemyMembers.length]?.name || ('Враг ' + ((((chosenEnemyIndex + 1) % enemyMembers.length) + 1))) }}</div>
                    <div class="preview-health" :style="{ width: (enemyHealthPercents[(chosenEnemyIndex + 1) % enemyMembers.length]?.percent ?? 0) + '%' }"></div>
                </div>
            </div>
            
            <div class="enemy-counter">{{ chosenEnemyIndex + 1 }} / {{ enemyMembers.length }}</div>
        </div>

        <div class="player-stats">
            <div class="player-name">Вы</div>
            <div class="player-stats-row">
                <div class="health-bar-container">
                    <div class="health-bar player-health" :style="{ width: playerHealthPercent + '%' }"></div>
                </div>
                <span class="health-text">{{ actionsStore.playerMember?.health ?? 0 }} / {{ actionsStore.playerMember?.maxHealth ?? 0 }}</span>
            </div>
            <div class="power-text">⚔️ {{ actionsStore.playerMember?.power ?? 0 }}</div>
            <div class="move-text" v-if="actionsStore.playerMember?.move">Ход: {{ actionsStore.playerMember.move }} по {{ enemyMembers[chosenEnemyIndex]?.name || ('Враг ' + (chosenEnemyIndex + 1)) }}</div>
            <button v-if="actionsStore.playerMember?.move === 'defend'" class="self-target-btn"
                    :class="{ selected: actionsStore.playerMember.targetId === actionsStore.playerMember?.id }"
                    @click="actionsStore.playerMember.targetId = actionsStore.playerMember.targetId === actionsStore.playerMember?.id ? '' : (actionsStore.playerMember?.id ?? '')"
                    :disabled="isPlayerDead">
                🧑 {{ actionsStore.playerMember?.targetId === actionsStore.playerMember?.id ? '✓ Себя' : 'Себя' }}
            </button>
        </div>

        <div class="battle-controls">
            <div class="moves-section">
                <div class="moves-label">Выберите действие:</div>
                <div class="move-buttons">
                    <button v-for="move in moves" :key="move" 
                            class="move-btn" 
                            :class="{ selected: actionsStore.playerMember?.move === move }"
                            :disabled="isPlayerDead"
                            @click="actionsStore.playerMember!.move = move">
                        {{ move === 'attack' ? '⚔️ Атака' : move === 'defend' ? '🛡️ Защита' : '✨ Спец.ход' }}
                    </button>
                </div>
            </div>
            <button class="send-move-btn" :disabled="!actionsStore.playerMember?.move || isPlayerDead" @click="handleSendMove">
                Отправить ход
            </button>
            <button class="end-battle-btn" @click="handleEndBattle">
                Завершить бой
            </button>
        </div>
    </div>
</template>

<style scoped>
.battle-panel {
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    background: rgba(30, 20, 50, 0.95);
    border: 2px solid rgba(255, 100, 100, 0.5);
    border-radius: 20px;
    padding: 24px;
    min-width: 500px;
    z-index: 200;
    box-shadow: 0 0 40px rgba(255, 50, 50, 0.3);
}

.battle-header {
    text-align: center;
    margin-bottom: 20px;
    position: relative;
    z-index: 2;
}

.battle-title {
    color: #ff6b6b;
    font-size: 28px;
    font-weight: 800;
    text-shadow: 0 0 20px rgba(255, 100, 100, 0.8);
    letter-spacing: 4px;
}

.battle-timer {
    position: absolute;
    right: 0;
    top: 50%;
    transform: translateY(-50%);
    font-size: 24px;
    font-weight: 700;
    color: #4f4;
    padding: 4px 12px;
    background: rgba(0, 50, 0, 0.6);
    border-radius: 8px;
    border: 2px solid rgba(0, 200, 0, 0.5);
}

.battle-timer.warning {
    color: #ff4;
    background: rgba(80, 60, 0, 0.6);
    border-color: rgba(255, 200, 0, 0.5);
    animation: pulse 0.5s infinite;
}

@keyframes pulse {
    0%, 100% { opacity: 1; }
    50% { opacity: 0.6; }
}

.battle-arena {
    margin-bottom: 20px;
}

.enemy-selector {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 8px;
    position: relative;
}

.nav-btn {
    width: 40px;
    height: 40px;
    background: rgba(80, 80, 120, 0.8);
    border: 2px solid rgba(140, 140, 200, 0.4);
    border-radius: 8px;
    color: #e0e0ff;
    font-size: 18px;
    cursor: pointer;
    transition: all 0.2s;
}

.nav-btn:hover:not(:disabled) {
    background: rgba(100, 100, 150, 0.9);
    border-color: rgba(160, 160, 220, 0.6);
}

.nav-btn:disabled {
    opacity: 0.4;
    cursor: not-allowed;
}

.enemy-preview {
    width: 80px;
    padding: 6px;
    background: rgba(30, 15, 25, 0.7);
    border-radius: 8px;
    border: 1px solid rgba(255, 100, 100, 0.3);
    opacity: 0.6;
}

.enemy-preview.left {
    margin-right: auto;
}

.enemy-preview.right {
    margin-left: auto;
}

.preview-name {
    color: #ff8a8a;
    font-size: 11px;
    text-align: center;
    margin-bottom: 4px;
}

.preview-health {
    height: 6px;
    background: rgba(200, 60, 60, 0.8);
    border-radius: 3px;
}

.enemy-item {
    min-width: 160px;
    padding: 12px;
    background: rgba(40, 20, 30, 0.9);
    border-radius: 10px;
    border: 2px solid rgba(255, 100, 100, 0.5);
    transition: all 0.2s;
}

.enemy-item.is-focused {
    border-color: rgba(255, 200, 200, 0.8);
    box-shadow: 0 0 30px rgba(255, 100, 100, 0.6);
    transform: scale(1.05);
}

.enemy-item:hover {
    transform: scale(1.05);
    border-color: rgba(255, 120, 120, 0.8);
    box-shadow: 0 0 20px rgba(255, 100, 100, 0.4);
}

.enemy-item.is-focused:hover {
    transform: scale(1.1);
}

.enemy-counter {
    text-align: center;
    color: #888;
    font-size: 12px;
    margin-top: 8px;
}

.player-stats {
    text-align: center;
    padding: 16px;
    background: rgba(20, 20, 40, 0.8);
    border-radius: 12px;
    border: 1px solid rgba(100, 150, 255, 0.4);
    margin-bottom: 24px;
}

.player-name {
    color: #6b9fff;
    font-size: 18px;
    font-weight: 700;
    margin-bottom: 12px;
}

.player-stats-row {
    display: flex;
    align-items: center;
    gap: 12px;
    justify-content: center;
}

.player-stats-row .health-bar-container {
    flex: 1;
    max-width: 300px;
}

.combatant-name {
    color: #e0e0ff;
    font-size: 18px;
    font-weight: 700;
    margin-bottom: 12px;
}

.enemy-name {
    color: #ff6b6b;
    font-size: 14px;
    font-weight: 700;
    margin-bottom: 8px;
}

.health-bar-container {
    height: 20px;
    background: rgba(60, 60, 80, 0.8);
    border-radius: 10px;
    overflow: hidden;
    margin-bottom: 8px;
}

.health-bar {
    height: 100%;
    transition: width 0.3s ease;
    border-radius: 10px;
}

.player-health {
    background: linear-gradient(90deg, rgba(60, 180, 60, 0.9), rgba(80, 200, 80, 1));
}

.enemy-health {
    background: linear-gradient(90deg, rgba(200, 60, 60, 0.9), rgba(220, 80, 80, 1));
}

.target-btn {
    margin-top: 8px;
    width: 100%;
    padding: 6px 10px;
    background: rgba(60, 60, 100, 0.8);
    border: 1px solid rgba(140, 140, 200, 0.4);
    border-radius: 6px;
    color: #e0e0ff;
    font-size: 11px;
    cursor: pointer;
    transition: all 0.2s;
}

.target-btn:disabled {
    opacity: 0.4;
    cursor: not-allowed;
}

.target-btn:hover {
    background: rgba(80, 80, 140, 0.9);
}

.target-btn.selected {
    background: rgba(180, 60, 60, 0.9);
    border-color: rgba(255, 120, 120, 0.6);
    box-shadow: 0 0 10px rgba(255, 100, 100, 0.4);
}

.power-text {
    color: #ffaa00;
    font-size: 14px;
    margin-top: 8px;
}

.self-target-btn {
    margin-top: 8px;
    padding: 6px 14px;
    background: rgba(60, 120, 60, 0.8);
    border: 1px solid rgba(100, 200, 100, 0.4);
    border-radius: 6px;
    color: #e0ffe0;
    font-size: 13px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;
}

.self-target-btn:hover:not(:disabled) {
    background: rgba(80, 160, 80, 0.9);
}

.self-target-btn.selected {
    background: rgba(60, 180, 60, 0.9);
    border-color: rgba(120, 255, 120, 0.6);
    box-shadow: 0 0 10px rgba(100, 255, 100, 0.4);
}

.self-target-btn:disabled {
    opacity: 0.4;
    cursor: not-allowed;
}

.move-text {
    color: #8f8;
    font-size: 14px;
    font-weight: 600;
    margin-top: 8px;
}

.battle-controls {
    display: flex;
    flex-direction: column;
    gap: 16px;
}

.moves-section {
    text-align: center;
}

.moves-label {
    color: #889;
    font-size: 14px;
    margin-bottom: 12px;
}

.move-buttons {
    display: flex;
    gap: 12px;
    justify-content: center;
}

.move-btn {
    padding: 10px 20px;
    background: rgba(60, 60, 100, 0.8);
    border: 2px solid rgba(140, 140, 200, 0.4);
    border-radius: 10px;
    color: #e0e0ff;
    font-size: 14px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;
}

.move-btn:disabled {
    opacity: 0.4;
    cursor: not-allowed;
}

.move-btn:hover {
    background: rgba(80, 80, 140, 0.9);
    border-color: rgba(160, 160, 220, 0.6);
}

.move-btn.selected {
    background: rgba(100, 100, 180, 0.9);
    border-color: rgba(180, 180, 255, 0.8);
    box-shadow: 0 0 15px rgba(140, 140, 255, 0.4);
}

.send-move-btn {
    padding: 14px 28px;
    background: linear-gradient(135deg, rgba(180, 80, 80, 0.9), rgba(200, 100, 100, 1));
    border: 2px solid rgba(255, 120, 120, 0.5);
    border-radius: 12px;
    color: #fff;
    font-size: 16px;
    font-weight: 700;
    cursor: pointer;
    transition: all 0.2s;
}

.send-move-btn:hover:not(:disabled) {
    background: linear-gradient(135deg, rgba(200, 100, 100, 1), rgba(220, 120, 120, 1));
    transform: translateY(-2px);
    box-shadow: 0 6px 20px rgba(255, 100, 100, 0.4);
}

.send-move-btn:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}

.end-battle-btn {
    padding: 10px 20px;
    background: rgba(80, 80, 80, 0.8);
    border: 2px solid rgba(140, 140, 140, 0.4);
    border-radius: 10px;
    color: #ccc;
    font-size: 14px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;
}

.end-battle-btn:hover {
    background: rgba(100, 100, 100, 0.9);
    border-color: rgba(180, 180, 180, 0.6);
}
</style>