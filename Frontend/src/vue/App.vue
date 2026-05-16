<script setup lang="ts">
import { onMounted } from 'vue';
import { useAuthStore } from './stores/authStore';
import StatsPanel from './components/StatsPanel.vue';
import SpaceshipPanel from './components/SpaceshipPanel.vue';
import ControlsBar from './components/ControlsBar.vue';
import SidePanels from './components/SidePanels.vue';
import ChatPanel from './components/ChatPanel.vue';
import LoginModal from './components/LoginModal.vue';
import RegisterModal from './components/RegisterModal.vue';
import BattlePanel from './components/BattlePanel.vue';
import TopSpaceshipsPanel from './components/TopSpaceshipsPanel.vue';
import HubPanel from './components/HubPanel.vue';
import GuidePanel from './components/GuidePanel.vue';
import QuestPanel from './components/QuestPanel.vue';
import type { VueCallbacks } from './index';
import { signalrProvider } from '../api/signalrProvider';

defineProps<{
    callbacks: VueCallbacks;
}>();

const authStore = useAuthStore();

onMounted(async () => {
    console.log('[App] mounted, initializing...');
    authStore.initialize();
    console.log('[App] isLoggedIn:', authStore.isLoggedIn);
    try {
        await signalrProvider.start();
        console.log('[App] SignalR started');
    } catch (err) {
        console.warn('[App] SignalR start failed:', err);
    }
});
</script>

<template>
    <StatsPanel />
    <SpaceshipPanel />
    <ControlsBar
        @reload="callbacks.onReload()"
        @centerOnSpaceship="callbacks.focusCameraOnSpaceship()"
        @toggleAnimation="callbacks.onToggleAnimation()"
        @brightnessChange="callbacks.onBrightnessChange($event)"
        @scaleChange="callbacks.onScaleChange($event)"
    />
    <SidePanels />
    <BattlePanel />
    <TopSpaceshipsPanel />
    <HubPanel />
    <GuidePanel />
    <QuestPanel />
    <LoginModal />
    <RegisterModal />
    <ChatPanel />
</template>
