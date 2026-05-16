import { resizeCanvas } from '../render/canvas';

export function initWindowChange(canvas : HTMLCanvasElement) : void {
    const ctx = canvas.getContext('2d')!;
    window.addEventListener('resize', () => resizeCanvas(canvas, ctx));
}
