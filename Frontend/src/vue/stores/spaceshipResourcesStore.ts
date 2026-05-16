import { defineStore } from 'pinia';
import { ref } from 'vue';
import { Balance, SpaceshipCondition } from '../../types/spaceshipResources';
import { UpgradeCost } from '../../types/upgradeCost';

export const useSpaceshipResourcesStore = defineStore('spaceshipResources', () => {
    const balance = ref<Balance>({
        currency: 0,
        quants: 0
    });
    const condition = ref<SpaceshipCondition>({
        durability: 0,
        maxDurability: 0,
        energy: 0,
        maxEnergy: 0,
        antimatter: 0,
        maxAntimatter: 0,
        power: 0,
        maxDistance: 0,
        speed: 0
    });
    const upgradeCost = ref<UpgradeCost>();

    function reset() {
        balance.value = { currency: 0, quants: 0 };
        condition.value = { durability: 0, maxDurability: 0, energy: 0, maxEnergy: 0, antimatter: 0, maxAntimatter: 0, power: 0, maxDistance: 0, speed: 0 };
    }

    return { balance, condition, upgradeCost, reset };
});
