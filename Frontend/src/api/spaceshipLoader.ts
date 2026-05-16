import { NamedSpaceship, Spaceship } from "../types/spaceship";
import { Star } from "../types/star";
import { fetchWithAuth } from "./fetchWithAuth";
import { useSpaceshipStore } from "../vue/stores/spaceshipStore";
import { useSpaceshipActionsStore } from "../vue/stores/spaceshipActionsStore";
import { updateCurrentStarInfo } from "./planetLoader";
import { signalrProvider } from "./signalrProvider";
import { requestDraw, startMovementAnimation, stopMovementAnimation } from "../render/renderScheduler";
import { isInAction } from "../vue/stores/stateFunctions";
import { loadAllResources } from "./spaceshipResourcesLoader";

export async function loadSpaceship(): Promise<void> {
    try {
        let response = await fetchWithAuth('/api/spaceship');
        if (!response.ok) {
            useSpaceshipStore().hasSpaceship = false;
            return;
        }

        const data = await response.json();
        useSpaceshipStore().spaceship = data as Spaceship;
        useSpaceshipStore().hasSpaceship = true;
        updateCurrentStarInfo();

    } catch (error: unknown) {
        console.error('Ошибка загрузки:', error);
        useSpaceshipStore().hasSpaceship = false;
    }
}

export async function createSpaceship(): Promise<void> {
    try {
        if (useSpaceshipStore().hasSpaceship) {
            return;
        }

        let response = await fetchWithAuth('/api/spaceship/create', {
            method: 'POST'
        });
        if (!response.ok) {
            useSpaceshipStore().hasSpaceship = false;
            return;
        }
        await Promise.all([loadSpaceship(), loadAllResources()]);
    } catch (error: unknown) {
        console.error('Ошибка загрузки:', error);
        useSpaceshipStore().hasSpaceship = false;
    }
}

export async function moveSpaceship(destination: Star): Promise<void> {
    try {
        const actionsStore = useSpaceshipActionsStore();
        if (isInAction()) {
            return;
        }

        if (useSpaceshipStore().spaceship.locatedRadius == destination.radius
            && useSpaceshipStore().spaceship.locatedAngleMilliradians == destination.angleMilliradians) {
            return;
        }

        let response = await fetchWithAuth('/api/spaceship/move', {
            method: 'POST',
            body: JSON.stringify({ newRadius: destination.radius, newAngle: destination.angleMilliradians })
        });
        const data = await response.json();
        if (!response.ok) {
            console.debug(data.error);
            actionsStore.isMoving = false;
            return;
        }
        onMoveStarted(data.arrivalTime, destination);
        loadAllResources();
        await signalrProvider.getConnection().invoke('MoveSpaceship',
            useSpaceshipStore().spaceship.locatedRadius, useSpaceshipStore().spaceship.locatedAngleMilliradians,
            destination.radius, destination.angleMilliradians);

    } catch (error: unknown) {
        console.error('Ошибка загрузки:', error);
        useSpaceshipActionsStore().isMoving = false;
    }
}

export function onMoveStarted(arrivalTime: Date | string, destination: Star) {
    const actionsStore = useSpaceshipActionsStore();
    const spaceship = useSpaceshipStore().spaceship;
    actionsStore.isMoving = true;
    actionsStore.movementStatus = {
        started: new Date(Date.now()).toISOString(),
        ends: new Date(arrivalTime).toISOString(),
        from: { radius: spaceship.locatedRadius, angleMilliradians: spaceship.locatedAngleMilliradians },
        to: destination
    };
    actionsStore.explorationStatus = null;
    startMovementAnimation();
}

export function onMoveConfirmed(radius: number, angle: number) {
    useSpaceshipStore().spaceship!.locatedRadius = radius;
    useSpaceshipStore().spaceship!.locatedAngleMilliradians = angle;
    updateCurrentStarInfo();
    useSpaceshipActionsStore().isMoving = false;
    useSpaceshipActionsStore().movementStatus = null;
    stopMovementAnimation();
    requestDraw();
    loadAllResources();
}

export async function getSpaceshipsOnStar(): Promise<NamedSpaceship[]> {
    try {
        let response = await fetchWithAuth('/api/spaceship/all-on-star');
        if (!response.ok) {
            if (response.status == 401) {
                useSpaceshipStore().hasSpaceship = false;
            }
            return [];
        }
        const data = await response.json();
        return data as NamedSpaceship[];

    } catch (error: unknown) {
        console.error('Ошибка загрузки кораблей в системе:', error);
        return [];
    }
}
