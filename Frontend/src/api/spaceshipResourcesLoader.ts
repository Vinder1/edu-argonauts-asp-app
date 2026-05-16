import { Balance, SpaceshipCondition } from "../types/spaceshipResources";
import { fetchWithAuth } from "./fetchWithAuth";
import { useSpaceshipResourcesStore } from "../vue/stores/spaceshipResourcesStore";
import { UpgradeCost } from "../types/upgradeCost";

export async function loadBalance(): Promise<void> {
    try {
        let response = await fetchWithAuth('/api/balance/show');
        if (!response.ok) {
            return;
        }

        const data = await response.json();
        useSpaceshipResourcesStore().balance = data as Balance;

    } catch (error: unknown) {
        console.error('Ошибка загрузки баланса:', error);
    }
}

export async function loadSpaceshipCondition(): Promise<void> {
    try {
        let response = await fetchWithAuth('/api/condition/show');
        if (!response.ok) {
            return;
        }

        const data = await response.json();
        const store = useSpaceshipResourcesStore();
        store.condition = data as SpaceshipCondition;
        store.condition.speed /= 10;
        store.condition.maxDistance /= 10;
    } catch (error: unknown) {
        console.error('Ошибка загрузки состояния корабля:', error);
    }
}

export async function loadUpgradeCost(): Promise<void> {
    try {
        let response = await fetchWithAuth('/api/condition/upgrade-cost', {
            method: 'GET'
        });
        if (!response.ok) {
            return;
        }
        const data = await response.json();
        const store = useSpaceshipResourcesStore();
        store.upgradeCost = data as UpgradeCost;
    } catch (error: unknown) {
        console.error('Ошибка получения цены на улучшения:', error);
    }
}

export async function upgradeHull(): Promise<void> {
    try {
        let response = await fetchWithAuth('/api/condition/upgrade-hull', {
            method: 'POST'
        });
        if (!response.ok) {
            return;
        }
        await loadAllResources();
        await loadUpgradeCost();
    } catch (error: unknown) {
        console.error('Ошибка восстановления корабля:', error);
    }
}

export async function upgradeEngine(): Promise<void> {
    try {
        let response = await fetchWithAuth('/api/condition/upgrade-engine', {
            method: 'POST'
        });
        if (!response.ok) {
            return;
        }
        await loadAllResources();
        await loadUpgradeCost();
    } catch (error: unknown) {
        console.error('Ошибка восстановления корабля:', error);
    }
}

export async function upgradeBattery(): Promise<void> {
    try {
        let response = await fetchWithAuth('/api/condition/upgrade-battery', {
            method: 'POST'
        });
        if (!response.ok) {
            return;
        }
        await loadAllResources();
        await loadUpgradeCost();
    } catch (error: unknown) {
        console.error('Ошибка восстановления корабля:', error);
    }
}

export async function restoreCondition(): Promise<void> {
    try {
        let response = await fetchWithAuth('/api/condition/restore', {
            method: 'POST'
        });
        if (!response.ok) {
            return;
        }
        await loadSpaceshipCondition();
    } catch (error: unknown) {
        console.error('Ошибка восстановления корабля:', error);
    }
}

export async function loadAllResources(): Promise<void> {
    await Promise.all([loadBalance(), loadSpaceshipCondition()]);
}
