import { camera } from '../state/cameraState';
import { galaxy } from '../state/galaxyState';
import { gameWindow } from '../state/gameWindowState';
import { requestDraw } from './renderScheduler';

export function resizeCanvas(canvas : HTMLCanvasElement, ctx : CanvasRenderingContext2D) : void {
    const dpr = window.devicePixelRatio || 1;
    gameWindow.width = window.innerWidth;
    gameWindow.height = window.innerHeight;
    canvas.width = gameWindow.width * dpr;
    canvas.height = gameWindow.height * dpr;
    ctx.scale(dpr, dpr);
    canvas.style.width = `${gameWindow.width}px`;
    canvas.style.height = `${gameWindow.height}px`;
    
    if (galaxy.stars.length === 0) {
        camera.offsetX = -gameWindow.width / 2;
        camera.offsetY = -gameWindow.height / 2;
    }
    requestDraw();
}
