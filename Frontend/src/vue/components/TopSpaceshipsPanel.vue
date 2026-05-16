<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { NamedSpaceship } from '../../types/spaceship';
import { loadTopSpaceships } from '../../api/topSpaceshipsLoader';
import { useOverlayVisibilityStore } from '../stores/overlayVisibilityStore';

const spaceships = ref<NamedSpaceship[]>([]);
const loading = ref(true);
const error = ref(false);

const overlayStore = useOverlayVisibilityStore();

onMounted(async () => {
    const result = await loadTopSpaceships();
    spaceships.value = result;
    loading.value = false;
    error.value = result.length === 0;
});
</script>

<template>
    <div v-if="overlayStore.isTopSpaceshipsPanelVisible" class="overlay-backdrop" @click.self="overlayStore.isTopSpaceshipsPanelVisible = false">
        <div class="panel">
            <div class="panel-header">
                <h3>Top 10 Spaceships</h3>
                <button class="close-btn" @click="overlayStore.isTopSpaceshipsPanelVisible = false">&times;</button>
            </div>
            <div class="panel-body">
                <div v-if="loading" class="loading">Loading...</div>
                <div v-else-if="error" class="empty">No spaceships found</div>
                <div v-else class="list">
                    <div class="list-header">
                        <span class="col-rank">#</span>
                        <span class="col-name">Name</span>
                        <span class="col-power">Battle Power</span>
                    </div>
                    <div v-for="(ship, index) in spaceships" :key="ship.id"
                        class="list-item" :class="{ 'top-three': index < 3, 'first': index === 0, 'second': index === 1, 'third': index === 2 }">
                        <span class="col-rank rank-badge" :class="'rank-' + (index + 1)">{{ index + 1 }}</span>
                        <span class="col-name">{{ ship.name }}</span>
                        <span class="col-power power-value">{{ ship.battlePower }}</span>
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
    border: 2px solid rgba(140, 100, 255, 0.5);
    border-radius: 20px;
    width: 460px;
    max-height: 80vh;
    display: flex;
    flex-direction: column;
    box-shadow: 0 20px 60px rgba(0, 0, 0, 0.6);
    overflow: hidden;
}

.panel-header {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 20px 24px;
    border-bottom: 1px solid rgba(140, 100, 255, 0.3);
}

.panel-header h3 {
    color: #e0e0ff;
    margin: 0;
    font-size: 20px;
}

.subtitle {
    color: #889;
    font-size: 12px;
    text-transform: uppercase;
    letter-spacing: 1px;
    flex: 1;
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
    padding: 16px 24px 24px;
    overflow-y: auto;
}

.loading, .empty {
    color: #889;
    font-size: 14px;
    text-align: center;
    padding: 24px;
}

.list {
    display: flex;
    flex-direction: column;
    gap: 6px;
}

.list-header {
    display: flex;
    align-items: center;
    padding: 8px 12px;
    color: #667;
    font-size: 11px;
    text-transform: uppercase;
    letter-spacing: 1px;
    border-bottom: 1px solid rgba(100, 100, 255, 0.15);
    margin-bottom: 4px;
}

.col-rank {
    width: 40px;
    text-align: center;
    flex-shrink: 0;
}

.col-name {
    flex: 1;
    text-align: left;
}

.col-power {
    width: 100px;
    text-align: right;
    flex-shrink: 0;
}

.list-item {
    display: flex;
    align-items: center;
    padding: 10px 12px;
    background: rgba(40, 40, 80, 0.4);
    border: 1px solid rgba(100, 100, 255, 0.15);
    border-radius: 10px;
    color: #d0d0ff;
    transition: all 0.2s;
}

.list-item:hover {
    background: rgba(50, 50, 100, 0.5);
    border-color: rgba(140, 100, 255, 0.3);
}

.list-item.top-three {
    border-width: 1.5px;
}

.list-item.first {
    border-color: rgba(255, 215, 0, 0.5);
    background: rgba(60, 50, 20, 0.4);
}

.list-item.second {
    border-color: rgba(192, 192, 192, 0.4);
    background: rgba(50, 50, 60, 0.4);
}

.list-item.third {
    border-color: rgba(205, 127, 50, 0.4);
    background: rgba(50, 40, 30, 0.4);
}

.rank-badge {
    font-weight: 700;
    font-size: 14px;
}

.rank-1 {
    color: #ffd700;
}

.rank-2 {
    color: #c0c0c0;
}

.rank-3 {
    color: #cd7f32;
}

.power-value {
    font-family: 'Courier New', monospace;
    font-weight: 600;
    color: #ffd700;
}
</style>
