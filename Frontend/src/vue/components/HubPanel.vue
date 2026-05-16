<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useOverlayVisibilityStore } from '../stores/overlayVisibilityStore';
import { useSpaceshipResourcesStore } from '../stores/spaceshipResourcesStore';
import { restoreCondition, upgradeHull, upgradeEngine, upgradeBattery, loadUpgradeCost } from '../../api/spaceshipResourcesLoader';

const overlayStore = useOverlayVisibilityStore();
const resourcesStore = useSpaceshipResourcesStore();

const restoreCost = 10;

const canRestore = computed(() => {
    const c = resourcesStore.condition;
    return c.durability < c.maxDurability ||
        c.energy < c.maxEnergy ||
        c.antimatter < c.maxAntimatter;
});

const canAffordRestore = computed(() => resourcesStore.balance.currency >= restoreCost);

const handleRestore = async () => {
    await restoreCondition();
};

const canAffordHull = computed(() => resourcesStore.balance.quants >= (resourcesStore.upgradeCost?.hull ?? 0));
const canAffordEngine = computed(() => resourcesStore.balance.quants >= (resourcesStore.upgradeCost?.engine ?? 0));
const canAffordBattery = computed(() => resourcesStore.balance.quants >= (resourcesStore.upgradeCost?.battery ?? 0));

const handleUpgradeHull = async () => {
    if (!resourcesStore.upgradeCost)
        return;
    if (resourcesStore.upgradeCost.hull > resourcesStore?.balance.quants)
        return;
    await upgradeHull();
};

const handleUpgradeEngine = async () => {
    if (!resourcesStore.upgradeCost)
        return;
    if (resourcesStore.upgradeCost.engine > resourcesStore?.balance.quants)
        return;
    await upgradeEngine();
};

const handleUpgradeBatteries = async () => {
    if (!resourcesStore.upgradeCost)
        return;
    if (resourcesStore.upgradeCost.battery > resourcesStore?.balance.quants)
        return;
    await upgradeBattery();
};

onMounted(async () => {
    await loadUpgradeCost();
});

</script>

<template>
    <div v-if="overlayStore.isHubPanelVisible" class="overlay-backdrop"
        @click.self="overlayStore.isHubPanelVisible = false">
        <div class="panel">
            <div class="panel-header">
                <h3>🏠 Хаб</h3>
                <button class="close-btn" @click="overlayStore.isHubPanelVisible = false">&times;</button>
            </div>
            <div class="panel-body">
                <div class="hub-info">
                    <p class="hub-welcome">Добро пожаловать в центральный хаб!</p>
                    <p class="hub-hint">Здесь вы можете восстановить состояние вашего корабля.</p>
                </div>

                <div class="condition-section">
                    <div class="condition-row">
                        <span class="label">Прочность</span>
                        <span class="value">{{ resourcesStore.condition.durability }}/{{
                            resourcesStore.condition.maxDurability }}</span>
                    </div>
                    <div class="condition-row">
                        <span class="label">Энергия</span>
                        <span class="value">{{ resourcesStore.condition.energy }}/{{
                            resourcesStore.condition.maxEnergy }}</span>
                    </div>
                    <div class="condition-row">
                        <span class="label">Антиматерия</span>
                        <span class="value">{{ resourcesStore.condition.antimatter }}/{{
                            resourcesStore.condition.maxAntimatter }}</span>
                    </div>
                    <div class="condition-row">
                        <span class="label">Сила</span>
                        <span class="value power">{{ resourcesStore.condition.power }}</span>
                    </div>
                    <div class="condition-row">
                        <span class="label">Дальность полёта</span>
                        <span class="value">{{ resourcesStore.condition.maxDistance }}</span>
                    </div>
                    <div class="condition-row">
                        <span class="label">Скорость полёта</span>
                        <span class="value">{{ resourcesStore.condition.speed }}</span>
                    </div>
                </div>

                <div class="action-section">
                    <span class="restore-note" :class="{ 'restore-needed': canRestore }">
                        {{ canRestore ? 'Состояние корабля можно восстановить' : 'Восстановление не требуется' }}
                    </span>
                    <button @click="handleRestore" :disabled="!canRestore || !canAffordRestore" class="restore-btn">🔧 Восстановить ({{ restoreCost }} кредитов)</button>
                </div>

                <div class="upgrade-section">
                    <p class="upgrade-title">Модули улучшений</p>
                    <div class="upgrade-row">
                        <button @click="handleUpgradeHull" :disabled="!canAffordHull" class="upgrade-btn hull">
                            <span class="btn-icon">🛡️</span>
                            <span class="btn-name">Корпус</span>
                            <span class="btn-stats">+1 Прочн. <br> +0.1 Дальн.</span>
                            <span class="btn-cost">{{ resourcesStore.upgradeCost?.hull }} Q</span>
                        </button>
                        <button @click="handleUpgradeEngine" :disabled="!canAffordEngine" class="upgrade-btn engine">
                            <span class="btn-icon">⚙️</span>
                            <span class="btn-name">Двигатель</span>
                            <span class="btn-stats">+1 Сила <br> +0.1 Скор.</span>
                            <span class="btn-cost">{{ resourcesStore.upgradeCost?.engine }} Q</span>
                        </button>
                        <button @click="handleUpgradeBatteries" :disabled="!canAffordBattery" class="upgrade-btn battery">
                            <span class="btn-icon">🔋</span>
                            <span class="btn-name">Батареи</span>
                            <span class="btn-stats">+1 Антим. <br> +0.1 Энерг.</span>
                            <span class="btn-cost">{{ resourcesStore.upgradeCost?.battery }} Q</span>
                        </button>
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
    border: 2px solid rgba(100, 200, 255, 0.5);
    border-radius: 20px;
    width: 420px;
    display: flex;
    flex-direction: column;
    box-shadow: 0 20px 60px rgba(0, 0, 0, 0.6);
    overflow: hidden;
}

.panel-header {
    display: flex;
    align-items: center;
    padding: 20px 24px;
    border-bottom: 1px solid rgba(100, 200, 255, 0.3);
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
    flex-direction: column;
    gap: 20px;
}

.hub-info {
    text-align: center;
}

.hub-welcome {
    color: #e0e0ff;
    font-size: 16px;
    font-weight: 600;
    margin: 0 0 8px 0;
}

.hub-hint {
    color: #889;
    font-size: 13px;
    margin: 0;
}

.condition-section {
    background: rgba(40, 40, 80, 0.5);
    border: 1px solid rgba(100, 200, 255, 0.2);
    border-radius: 12px;
    padding: 16px;
    display: flex;
    flex-direction: column;
    gap: 10px;
}

.condition-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 14px;
}

.condition-row .label {
    color: #889;
    font-size: 12px;
    text-transform: uppercase;
    letter-spacing: 1px;
}

.condition-row .value {
    color: #e0e0ff;
    font-weight: 600;
    font-family: 'Courier New', monospace;
}

.condition-row .value.power {
    color: #ffaa00;
}

.action-section {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 12px;
}

.restore-note {
    font-size: 12px;
    color: #667;
}

.restore-note.restore-needed {
    color: #8f8;
}

.restore-btn {
    cursor: pointer;
    background: linear-gradient(135deg, rgba(80, 140, 80, 0.8), rgba(100, 160, 100, 0.9));
    border: 1px solid rgba(120, 200, 120, 0.5);
    border-radius: 10px;
    padding: 12px 24px;
    color: #fff;
    font-weight: 700;
    font-size: 14px;
    transition: all 0.2s;
    width: 100%;
}

.restore-btn:hover:not(:disabled) {
    background: linear-gradient(135deg, rgba(100, 160, 100, 0.9), rgba(120, 180, 120, 1));
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(80, 180, 80, 0.4);
}

.restore-btn:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}

.upgrade-section {
    display: flex;
    flex-direction: column;
    gap: 12px;
}

.upgrade-title {
    color: #889;
    font-size: 12px;
    text-transform: uppercase;
    letter-spacing: 1.5px;
    margin: 0;
    text-align: center;
}

.upgrade-row {
    display: flex;
    gap: 10px;
}

.upgrade-btn {
    flex: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 6px;
    background: rgba(30, 30, 60, 0.5);
    border: 1px solid rgba(100, 200, 255, 0.25);
    border-radius: 10px;
    padding: 12px 8px;
    color: #e0e0ff;
    cursor: pointer;
    transition: all 0.2s ease;
    text-align: center;
}

.upgrade-btn:hover:not(:disabled) {
    background: rgba(40, 40, 80, 0.7);
    transform: translateY(-2px);
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.4);
}

.upgrade-btn .btn-icon {
    font-size: 22px;
    line-height: 1;
}

.upgrade-btn .btn-name {
    font-size: 13px;
    font-weight: 600;
}

.upgrade-btn .btn-stats {
    font-size: 11px;
    color: #7a9;
    font-family: 'Courier New', monospace;
}

.upgrade-btn .btn-cost {
    font-size: 11px;
    color: #889;
    font-family: 'Courier New', monospace;
    margin-top: 2px;
    transition: color 0.2s;
}

.upgrade-btn.hull:hover:not(:disabled) {
    border-color: #4a9eff;
    box-shadow: 0 4px 15px rgba(74, 158, 255, 0.3);
}

.upgrade-btn.engine:hover:not(:disabled) {
    border-color: #ffaa00;
    box-shadow: 0 4px 15px rgba(255, 170, 0, 0.3);
}

.upgrade-btn.battery:hover:not(:disabled) {
    border-color: #00ffaa;
    box-shadow: 0 4px 15px rgba(0, 255, 170, 0.3);
}

.upgrade-btn:disabled {
    opacity: 0.45;
    cursor: not-allowed;
    filter: grayscale(0.6);
}

.upgrade-btn:disabled .btn-cost {
    color: #ff5555;
    font-weight: 600;
}
</style>