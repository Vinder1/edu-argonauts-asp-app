import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useGameStore = defineStore('game', () => {
    const starCount = ref(0);
    const minRadius = ref(0);
    const maxRadius = ref(0);
    const brightness = ref(0.8);
    const scale = ref(25);
    const isAnimating = ref(false);
    const error = ref<string | null>(null);

    function reset() {
        starCount.value = 0;
        minRadius.value = 0;
        maxRadius.value = 0;
        brightness.value = 0.8;
        scale.value = 25;
        isAnimating.value = false;
        error.value = null;
    }

    return {
        starCount,
        minRadius,
        maxRadius,
        brightness,
        scale,
        isAnimating,
        error,
        reset
    };
});
