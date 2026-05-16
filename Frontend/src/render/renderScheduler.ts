import { drawGalaxy } from './draw';
import { useSpaceshipActionsStore } from '../vue/stores/spaceshipActionsStore';

let pendingDraw = false;
let renderContext: CanvasRenderingContext2D | null = null;
let animationId: number | null = null;

export function setRenderContext(ctx: CanvasRenderingContext2D): void {
    renderContext = ctx;
}

export function requestDraw(): void {
    if (pendingDraw) return;
    pendingDraw = true;
    requestAnimationFrame(() => {
        pendingDraw = false;
        if (renderContext) {
            drawGalaxy(renderContext);
        }
    });
}

function animationLoop(): void {
    requestDraw();
    const actionsStore = useSpaceshipActionsStore();
    if (actionsStore.isExploring && Date.now() > new Date(actionsStore.ExploringUntil).getTime() + 5000) {
        location.reload();
        return;
    }
    if (!actionsStore.movementStatus) {
        stopMovementAnimation();
        return;
    }
    const endsTime = new Date(actionsStore.movementStatus.ends).getTime();
    if (Date.now() > endsTime) {
        stopMovementAnimation();
        return;
    }
    if (Date.now() > endsTime + 5000) {
        location.reload();
        return;
    }
    animationId = requestAnimationFrame(animationLoop);
}

export function startMovementAnimation(): void {
    if (animationId !== null) return;
    animationId = requestAnimationFrame(animationLoop);
}

export function stopMovementAnimation(): void {
    if (animationId !== null) {
        cancelAnimationFrame(animationId);
        animationId = null;
    }
}
