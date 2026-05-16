import { gameWindow } from '../state/gameWindowState';
import { requestDraw } from '../render/renderScheduler';
import { MAX_SCALE } from '../config/constants';
import { camera } from '../state/cameraState';
import { interaction } from '../state/interactionState';
import { calculateCoordinates } from '../math/calculate_coordinates';
import { moveSpaceship } from '../api/spaceshipLoader';

let isDragging: boolean = false;
let lastMouseX: number = 0;
let lastMouseY: number = 0;

export function initMouse(canvas: HTMLCanvasElement): void {
    canvas.addEventListener('mousedown', (e) => {
        isDragging = true;
        lastMouseX = e.clientX;
        lastMouseY = e.clientY;
    });

    canvas.addEventListener('wheel', (e) => {
        e.preventDefault();
        let delta = e.deltaY > 0 ? -1 : 1;
        delta *= camera.scale / 15;
        delta = Math.round(delta);

        const prevScale = camera.scale;
        camera.scale = Math.max(10, Math.min(MAX_SCALE, camera.scale + delta));

        let dx = camera.offsetX + (gameWindow.width / 2);
        let dy = camera.offsetY + (gameWindow.height / 2);
        dx *= camera.scale / prevScale;
        dy *= camera.scale / prevScale;
        camera.offsetX = - (gameWindow.width / 2) + dx;
        camera.offsetY = - (gameWindow.height / 2) + dy;

        requestDraw();
    });

    window.addEventListener('mousemove', (e) => {
        identifyStarUnderMouse(canvas, e.offsetX, e.offsetY);
        dragMap(e.clientX, e.clientY);
        requestDraw();
    });

    window.addEventListener('mouseup', () => {
        isDragging = false;
    });

    canvas.addEventListener('click', (e: MouseEvent) => {
        const rect = canvas.getBoundingClientRect();
        const mouseX = e.clientX - rect.left;
        const mouseY = e.clientY - rect.top;

        if (interaction.hoveredStar != null) {
            const star = interaction.hoveredStar;
            const coords = calculateCoordinates(star.star);
            const dx = mouseX - coords.x;
            const dy = mouseY - coords.y;
            if (Math.sqrt(dy * dy + dx * dx) <= interaction.HIT_RADIUS) {
                moveSpaceship(star.star)
                    .then(() => requestDraw());
            }
        }
    });
}

function identifyStarUnderMouse(canvas: HTMLCanvasElement, mouseX: number, mouseY: number) {
    let found: typeof interaction.hoveredStar = null;
    let minDist = Infinity;
    const bOp = 1 / 0.09;

    for (const star of interaction.interactiveStars) {
        const coords = calculateCoordinates(star.star);
        const dx = mouseX - coords.x;
        const dy = mouseY - coords.y;
        const dist = Math.sqrt(dx * dx + dy * dy * bOp);

        if (dist < interaction.HIT_RADIUS && dist < minDist) {
            minDist = dist;
            found = star;
        }
    }

    interaction.hoveredStar = found;
    canvas.style.cursor = found ? 'pointer' : 'default';

    if (found) {
        requestDraw();
    }
}

function dragMap(mouseX: number, mouseY: number) {
    if (!isDragging) {
        return;
    }

    const deltaX = mouseX - lastMouseX;
    const deltaY = mouseY - lastMouseY;

    camera.offsetX -= deltaX;
    camera.offsetY -= deltaY;

    lastMouseX = mouseX;
    lastMouseY = mouseY;
}
