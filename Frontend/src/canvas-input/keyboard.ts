import { ROTATION_SPEED, PAN_SPEED } from '../config/constants';
import { camera } from '../state/cameraState';
import { requestDraw } from '../render/renderScheduler';

let prevTime = Date.now();

let keysPressed: Record<string, boolean> = {};

export function initKeyboard(): void {
    window.addEventListener('keydown', (e) => {
        keysPressed[e.key.toLowerCase()] = true;
    });

    window.addEventListener('keyup', (e) => {
        keysPressed[e.key.toLowerCase()] = false;
    });
}

let speedX: number = 0;
let speedY: number = 0;
let speedRotRight: number = 0;

export function handleInput(): void {
    let d = (Date.now() - prevTime) / 1000;
    if (d > 0.1)
        d = 0.1;

    let oldSpeedX = speedX;
    let oldSpeedY = speedY;
    let oldSpeedRot = speedRotRight;

    if (keysPressed['q']) { speedRotRight -= ROTATION_SPEED * d / 30; }
    if (keysPressed['e']) { speedRotRight += ROTATION_SPEED * d / 30; }
    if (keysPressed['w']) { speedY -= PAN_SPEED * d / 30; }
    if (keysPressed['s']) { speedY += PAN_SPEED * d / 30; }
    if (keysPressed['a']) { speedX -= PAN_SPEED * d / 30; }
    if (keysPressed['d']) { speedX += PAN_SPEED * d / 30; }

    speedRotRight = reduceIfNotChanged(speedRotRight, oldSpeedRot);
    speedX = reduceIfNotChanged(speedX, oldSpeedX);
    speedY = reduceIfNotChanged(speedY, oldSpeedY);

    speedX = clamp(speedX, -PAN_SPEED, PAN_SPEED);
    speedY = clamp(speedY, -PAN_SPEED, PAN_SPEED);
    speedRotRight = clamp(speedRotRight, -ROTATION_SPEED, ROTATION_SPEED);

    camera.offsetX += speedX;
    camera.offsetY += speedY;
    camera.rotation += speedRotRight;

    const changed = speedX != 0 || speedY != 0 || speedRotRight != 0;

    if (changed)
        requestDraw();
    prevTime = Date.now();
}

function reduceIfNotChanged(value: number, oldValue: number): number {
    if (value == oldValue) {
        value *= 0.9;
        if (Math.abs(value) < 0.1)
            value = 0;
    }
    return value;
}

function clamp(value: number, min: number, max: number): number {
    return Math.min(Math.max(value, min), max);
}
