import { loadStars } from './api/starLoader';
import { resizeCanvas } from './render/canvas';
import { initKeyboard } from './canvas-input/keyboard';
import { initMouse } from './canvas-input/mouse';
import { loadUser } from './api/userLoader';
import { loadSpaceship } from './api/spaceshipLoader';
import { setRenderContext } from './render/renderScheduler';
import { mountVueApp } from './vue';
import { useAuthStore } from './vue/stores/authStore';
import { onBrightnessChange, focusCameraOnSpaceship, onReload, onScaleChange, onToggleAnimation, smoothTranslateCameraToSpaceship } from './ui/galaxy-view';
import { initWindowChange } from './canvas-input/window';
import { updateCurrentStarInfo, loadStarVisit } from './api/planetLoader';
import { loadAllResources } from './api/spaceshipResourcesLoader';

let ctx: CanvasRenderingContext2D;

async function init() {
    const canvas = document.getElementById('galaxyCanvas') as HTMLCanvasElement;
    ctx = canvas.getContext('2d')!;

    setRenderContext(ctx);

    const { app, pinia } = mountVueApp({
        onReload,
        focusCameraOnSpaceship: smoothTranslateCameraToSpaceship,
        onToggleAnimation,
        onBrightnessChange,
        onScaleChange,
    });
    app.mount('#vue-app');

    const authStore = useAuthStore(pinia);
    authStore.initialize();

    initMouse(canvas);
    initKeyboard();
    initWindowChange(canvas);

    resizeCanvas(canvas, ctx);
    
    await loadUser();
    await loadSpaceship();
    focusCameraOnSpaceship();
    await loadStars();
    updateCurrentStarInfo();
    await loadStarVisit();
    await loadAllResources();
}

document.addEventListener('DOMContentLoaded', init);
