import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useOverlayVisibilityStore = defineStore('overlayVisibility', () => {
    const isBattlePanelVisible = ref(true);
    const isTopSpaceshipsPanelVisible = ref(false);
    const isHubPanelVisible = ref(false);
    const isGuidePanelVisible = ref(false);
    const isQuestPanelVisible = ref(false);

    function reset() {
        isBattlePanelVisible.value = true;
        isTopSpaceshipsPanelVisible.value = false;
        isHubPanelVisible.value = false;
        isGuidePanelVisible.value = false;
        isQuestPanelVisible.value = false;
    }

    return { isBattlePanelVisible, isTopSpaceshipsPanelVisible, isHubPanelVisible, isGuidePanelVisible, isQuestPanelVisible, reset };
});
