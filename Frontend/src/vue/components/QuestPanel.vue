<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { useOverlayVisibilityStore } from '../stores/overlayVisibilityStore';
import { loadQuest, type QuestData } from '../../api/questLoader';

const overlayStore = useOverlayVisibilityStore();

interface QuestProgress {
    level: number;
    killed: number;
    killsRequired: number;
    description: string;
}

const quest = ref<QuestProgress | null>(null);

async function refreshQuest() {
    const data = await loadQuest();
    if (data) {
        quest.value = {
            level: data.level,
            killed: data.killed,
            killsRequired: data.killsRequired,
            description: data.description
        };
    } else {
        quest.value = null;
    }
}

const progressPercent = ref(0);

watch(quest, (q) => {
    if (q && q.killsRequired > 0) {
        progressPercent.value = Math.round((q.killed / q.killsRequired) * 100);
    }
}, { immediate: true });

watch(() => overlayStore.isQuestPanelVisible, async (visible) => {
    if (visible) {
        await refreshQuest();
    }
});

onMounted(async () => {
    if (overlayStore.isQuestPanelVisible) {
        await refreshQuest();
    }
});
</script>

<template>
    <div v-if="overlayStore.isQuestPanelVisible" class="overlay-backdrop"
        @click.self="overlayStore.isQuestPanelVisible = false">
        <div class="panel">
            <div class="panel-header">
                <h3>🎯 Квесты</h3>
                <button class="close-btn" @click="overlayStore.isQuestPanelVisible = false">&times;</button>
            </div>
            <div class="panel-body">
                <div v-if="quest" class="quest-info">
                    <div class="quest-header">
                        <span class="quest-level">Уровень {{ quest.level }}</span>
                    </div>
                    <div class="quest-progress">
                        <div class="progress-bar-bg">
                            <div class="progress-bar-fill" :style="{ width: progressPercent + '%' }"></div>
                        </div>
                        <span class="progress-text">{{ quest.killed }} / {{ quest.killsRequired }}</span>
                    </div>
                    <div v-if="quest.description" class="quest-description">
                        {{ quest.description }}
                    </div>
                </div>
                <div v-else class="quest-empty">
                    Не удалось загрузить квест
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
    background: rgba(0, 0, 0, 0.6);
    backdrop-filter: blur(4px);
    z-index: 100;
    display: flex;
    justify-content: center;
    align-items: center;
}

.panel {
    background: rgba(20, 20, 45, 0.95);
    border: 2px solid rgba(255, 180, 50, 0.5);
    border-radius: 20px;
    width: 480px;
    max-height: 80vh;
    display: flex;
    flex-direction: column;
    box-shadow: 0 20px 60px rgba(0, 0, 0, 0.6);
    overflow: hidden;
}

.panel-header {
    display: flex;
    align-items: center;
    padding: 20px 24px;
    border-bottom: 1px solid rgba(255, 180, 50, 0.3);
}

.panel-header h3 {
    color: #e0e0ff;
    margin: 0;
    font-size: 20px;
}

.close-btn {
    margin-left: auto;
    background: none;
    border: none;
    color: #aab;
    font-size: 28px;
    cursor: pointer;
    padding: 4px 8px;
    border-radius: 8px;
    line-height: 1;
    transition: all 0.2s;
}

.close-btn:hover {
    background: rgba(255, 100, 100, 0.2);
    color: #ff6b6b;
}

.panel-body {
    padding: 24px;
    overflow-y: auto;
}

.quest-info {
    display: flex;
    flex-direction: column;
    gap: 20px;
}

.quest-header {
    text-align: center;
}

.quest-level {
    color: #ffcc44;
    font-size: 22px;
    font-weight: 700;
    text-shadow: 0 0 10px rgba(255, 200, 50, 0.4);
}

.quest-progress {
    display: flex;
    flex-direction: column;
    gap: 8px;
    align-items: center;
}

.progress-bar-bg {
    width: 100%;
    height: 20px;
    background: rgba(40, 40, 80, 0.6);
    border-radius: 10px;
    overflow: hidden;
    border: 1px solid rgba(255, 180, 50, 0.2);
}

.progress-bar-fill {
    height: 100%;
    background: linear-gradient(90deg, rgba(255, 180, 50, 0.6), rgba(255, 200, 80, 0.9));
    border-radius: 10px;
    transition: width 0.3s ease;
    box-shadow: 0 0 10px rgba(255, 180, 50, 0.3);
}

.progress-text {
    color: #e0e0ff;
    font-size: 14px;
    font-weight: 600;
    font-family: 'Courier New', monospace;
}

.quest-description {
    color: #c0c0e0;
    font-size: 14px;
    line-height: 1.7;
    white-space: pre-wrap;
    background: rgba(30, 30, 60, 0.5);
    border: 1px solid rgba(255, 180, 50, 0.15);
    border-radius: 12px;
    padding: 20px;
}

.quest-empty {
    color: #667;
    text-align: center;
    padding: 40px;
    font-size: 14px;
}
</style>
