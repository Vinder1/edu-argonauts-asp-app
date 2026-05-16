<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { useOverlayVisibilityStore } from '../stores/overlayVisibilityStore';
import { loadGuideKeys, loadGuideContent } from '../../api/guideLoader';

const overlayStore = useOverlayVisibilityStore();

const guideKeys = ref<string[]>([]);
const selectedGuide = ref<string | null>(null);
const guideText = ref<string>('');

const guideLabels: Record<string, string> = {
    hello: 'Введение',
    movement: 'Перемещение',
    exploration: 'Исследование',
    combat: 'Бой',
    spaceship: 'Корабль',
    resources: 'Ресурсы',
    galaxy: 'Галактика',
    hub: 'Хаб',
    rating: 'Рейтинг'
};

async function selectGuide(key: string) {
    selectedGuide.value = key;
    const entry = await loadGuideContent(key);
    guideText.value = entry?.text ?? 'Гайд не найден';
}

async function loadKeys() {
    guideKeys.value = await loadGuideKeys();
    if (guideKeys.value.length > 0) {
        await selectGuide(guideKeys.value[0]);
    }
}

watch(() => overlayStore.isGuidePanelVisible, async (visible) => {
    if (visible) {
        await loadKeys();
    }
});

onMounted(async () => {
    if (overlayStore.isGuidePanelVisible) {
        await loadKeys();
    }
});
</script>

<template>
    <div v-if="overlayStore.isGuidePanelVisible" class="overlay-backdrop"
        @click.self="overlayStore.isGuidePanelVisible = false">
        <div class="panel">
            <div class="panel-header">
                <h3>📖 Гайд по игре</h3>
                <button class="close-btn" @click="overlayStore.isGuidePanelVisible = false">&times;</button>
            </div>
            <div class="panel-body">
                <div class="guide-sidebar">
                    <button v-for="key in guideKeys" :key="key"
                        :class="['guide-tab', { active: selectedGuide === key }]"
                        @click="selectGuide(key)">
                        {{ guideLabels[key] ?? key }}
                    </button>
                </div>
                <div class="guide-content-area">
                    <div v-if="guideText" class="guide-content">
                        {{ guideText }}
                    </div>
                    <div v-else class="guide-empty">
                        Загрузка...
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
    background: rgba(0, 0, 0, 0.6);
    backdrop-filter: blur(4px);
    z-index: 100;
    display: flex;
    justify-content: center;
    align-items: center;
}

.panel {
    background: rgba(20, 20, 45, 0.95);
    border: 2px solid rgba(180, 100, 255, 0.5);
    border-radius: 20px;
    width: 700px;
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
    border-bottom: 1px solid rgba(180, 100, 255, 0.3);
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
    padding: 20px 24px 24px;
    display: flex;
    flex-direction: row;
    gap: 16px;
    overflow: hidden;
    flex: 1;
}

.guide-sidebar {
    display: flex;
    flex-direction: column;
    gap: 6px;
    min-width: 140px;
    flex-shrink: 0;
}

.guide-content-area {
    flex: 1;
    overflow-y: auto;
    min-height: 0;
}

.guide-tab {
    cursor: pointer;
    color: #e0e0ff;
    background: linear-gradient(135deg, rgba(80, 80, 140, 0.6), rgba(60, 60, 120, 0.8));
    border: 1px solid rgba(100, 100, 255, 0.3);
    border-radius: 8px;
    padding: 10px 14px;
    font-size: 13px;
    font-weight: 600;
    text-align: left;
    transition: all 0.25s ease;
}

.guide-tab:hover {
    background: linear-gradient(135deg, rgba(100, 100, 180, 0.7), rgba(80, 80, 160, 0.9));
    border-color: rgba(130, 130, 255, 0.5);
}

.guide-tab.active {
    background: linear-gradient(135deg, rgba(140, 80, 180, 0.7), rgba(120, 60, 160, 0.9));
    border-color: rgba(180, 100, 255, 0.6);
    box-shadow: 0 0 12px rgba(180, 100, 255, 0.3);
}

.guide-content {
    color: #c0c0e0;
    font-size: 14px;
    line-height: 1.7;
    white-space: pre-wrap;
    background: rgba(30, 30, 60, 0.5);
    border: 1px solid rgba(100, 100, 200, 0.15);
    border-radius: 12px;
    padding: 20px;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.guide-empty {
    color: #667;
    text-align: center;
    padding: 40px;
    font-size: 14px;
}
</style>
