import { defineStore } from 'pinia';
import { ref } from 'vue';
import { Spaceship } from '../../types/spaceship';

export const useSpaceshipStore = defineStore('spaceship', () => {
    const spaceship = ref<Spaceship>({
        ownerId: "unknown",
        galaxyVersion: 0,
        locatedAngleMilliradians: 0,
        locatedRadius: 0,
    });
    const hasSpaceship = ref(false);

    function reset() {
        spaceship.value = {
            ownerId: "unknown",
            galaxyVersion: 0,
            locatedAngleMilliradians: 0,
            locatedRadius: 0,
        };
        hasSpaceship.value = false;
    }

    return { spaceship, hasSpaceship, reset };
});
