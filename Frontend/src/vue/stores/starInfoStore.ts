import { defineStore } from 'pinia';
import { reactive } from 'vue';
import { StarInfo } from '../../types/star';

export const useStarInfoStore = defineStore('starInfo', () => {
    const starInfo = reactive<StarInfo>({
        star: { type: '-', radius: 0, angleMilliradians: 0 },
        visit: { exists: false, visitedAt: '' }
    });

    function reset() {
        starInfo.star = { type: '-', radius: 0, angleMilliradians: 0 };
        starInfo.visit = { exists: false, visitedAt: '' };
    }

    return { starInfo, reset };
});
