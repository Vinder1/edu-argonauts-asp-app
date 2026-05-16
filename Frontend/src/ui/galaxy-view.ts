import { loadStars } from '../api/starLoader';
import { toggleAnimation } from '../render/animate';
import { camera } from '../state/cameraState';
import { loadUser } from '../api/userLoader';
import { loadSpaceship } from '../api/spaceshipLoader';
import { loadAllResources } from '../api/spaceshipResourcesLoader';
import { requestDraw } from '../render/renderScheduler';
import { useGameStore } from '../vue/stores/gameStore';
import { useSpaceshipStore } from '../vue/stores/spaceshipStore';
import { gameWindow } from '../state/gameWindowState';

export function onReload() {
    loadUser();
    loadSpaceship();
    loadAllResources();
    loadStars();
}

export function focusCameraOnSpaceship() {
    const spaceshipStore = useSpaceshipStore();
    let x = 0;
    let y = 0;
    if (spaceshipStore.hasSpaceship && spaceshipStore.spaceship) {
        const angle = spaceshipStore.spaceship.locatedAngleMilliradians / 1000;
        const rotatedAngle = angle + camera.rotation;
        x = spaceshipStore.spaceship.locatedRadius * camera.scale * Math.cos(rotatedAngle);
        y = spaceshipStore.spaceship.locatedRadius * camera.scale * Math.sin(rotatedAngle);
    }

    camera.offsetX = x - gameWindow.width / 2;
    camera.offsetY = y * 0.3 - gameWindow.height / 2;
    requestDraw();
}

export function smoothTranslateCameraToSpaceship() {
    const spaceshipStore = useSpaceshipStore();
    let x = 0;
    let y = 0;
    if (spaceshipStore.hasSpaceship && spaceshipStore.spaceship) {
        const angle = spaceshipStore.spaceship.locatedAngleMilliradians / 1000;
        const rotatedAngle = angle + camera.rotation;
        x = spaceshipStore.spaceship.locatedRadius * camera.scale * Math.cos(rotatedAngle);
        y = spaceshipStore.spaceship.locatedRadius * camera.scale * Math.sin(rotatedAngle);
    }

    const targetX = x - gameWindow.width / 2;
    const targetY = y * 0.3 - gameWindow.height / 2;

    const startX = camera.offsetX;
    const startY = camera.offsetY;
    const duration = 500;
    const startTime = performance.now();

    function animate(currentTime: number) {
        const elapsed = currentTime - startTime;
        const t = Math.min(elapsed / duration, 1);
        const easeT = t * (2 - t);

        camera.offsetX = startX + (targetX - startX) * easeT;
        camera.offsetY = startY + (targetY - startY) * easeT;
        requestDraw();

        if (t < 1) {
            requestAnimationFrame(animate);
        }
    }

    requestAnimationFrame(animate);
}

export function onToggleAnimation() {
    const gameStore = useGameStore();
    toggleAnimation();
    gameStore.isAnimating = !gameStore.isAnimating;
}

export function onBrightnessChange(value: number) {
    camera.brightness = value;
    requestDraw();
}

export function onScaleChange(value: number) {
    camera.scale = value;
    requestDraw();
}