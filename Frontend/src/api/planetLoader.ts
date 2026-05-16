import { StarVisit } from "../types/starVisit";
import { fetchWithAuth } from "./fetchWithAuth";
import { useStarInfoStore } from "../vue/stores/starInfoStore";
import { useSpaceshipActionsStore } from "../vue/stores/spaceshipActionsStore";
import { galaxy } from "../state/galaxyState";
import { isInAction } from "../vue/stores/stateFunctions";
import { useSpaceshipStore } from "../vue/stores/spaceshipStore";

export function updateCurrentStarInfo(): void {
    const starInfoStore = useStarInfoStore();
    const spaceshipStore = useSpaceshipStore();
    const radius = spaceshipStore.spaceship.locatedRadius;
    const angleMilliradians = spaceshipStore.spaceship.locatedAngleMilliradians;
    const found = galaxy.stars.find(s => s.radius === radius && s.angleMilliradians === angleMilliradians);
    if (found) {
        starInfoStore.starInfo.star = found;
    }
}

export async function loadStarVisit(): Promise<void> {
    try {
        const starInfoStore = useStarInfoStore();
        const { radius, angleMilliradians } = starInfoStore.starInfo.star;
        let response = await fetchWithAuth(`/api/star-visit/active?radius=${radius}&angleMilliradians=${angleMilliradians}`);
        if (!response.ok) {
            updateStarVisitStore({ exists: false, visitedAt: '' });
            return;
        }
        const data = await response.json();
        updateStarVisitStore(data as StarVisit);
    } catch (error: unknown) {
        console.error('Ошибка загрузки:', error);
        updateStarVisitStore({ exists: false, visitedAt: '' });
    }
}

export async function startExploration(): Promise<void> {
    try {
        if (isInAction()) {
            return;
        }

        let response = await fetchWithAuth('/api/explore/start', {
            method: 'POST'
        });
        const data = await response.json();
        if (!response.ok) {
            console.debug(data.error);
            return;
        }
        onExploreStart(data.arrivalTime);
    } catch (error: unknown) {
        console.error('Ошибка загрузки:', error);
        updateStarVisitStore({ exists: false, visitedAt: '' });
    }
}

export function onExploreStart(arrivalTime: Date | string) {
    const actionsStore = useSpaceshipActionsStore();
    actionsStore.isExploring = true;
    actionsStore.ExploringUntil = new Date(arrivalTime);
}

export function onExploreEnd(success: boolean, arrivalTime: Date | string) {
    updateStarVisitStore({
        exists: success,
        visitedAt: new Date(arrivalTime).toISOString()
    });
    useSpaceshipActionsStore().isExploring = false;
}

export function updateStarVisitStore(visit: StarVisit) {
    const starInfoStore = useStarInfoStore();
    starInfoStore.starInfo.visit = visit;
}

export async function harvest(): Promise<void> {
    try {
        let response = await fetchWithAuth('/api/explore/harvest', {
            method: 'POST'
        });
        if (!response.ok) {
            console.debug('Harvest failed');
            return;
        }
    } catch (error: unknown) {
        console.error('Ошибка загрузки:', error);
    }
}