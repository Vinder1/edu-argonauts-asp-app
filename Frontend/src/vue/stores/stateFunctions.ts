import { useSpaceshipActionsStore } from "./spaceshipActionsStore";

export function isInAction(): boolean {
    const store = useSpaceshipActionsStore();
    return store.isMoving || store.isExploring;
}