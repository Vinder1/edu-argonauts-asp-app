import { camera } from "../state/cameraState";
import { Star } from "../types/star";

export function calculateCoordinates(star: Star) : { x: number, y: number } {
    const angle = star.angleMilliradians / 1000;
    const rotatedAngle = angle + camera.rotation;

    const x = star.radius * camera.scale * Math.cos(rotatedAngle) - camera.offsetX;
    const y = star.radius * camera.scale * Math.sin(rotatedAngle) * 0.3 - camera.offsetY;

    return { x, y };
}