import { handleInput } from "../canvas-input/keyboard";
let isAnimating = false;
let animationId : number | null = null;

export function toggleAnimation() : void {
    if (isAnimating) {
        stopAnimation()
    } else {
        startAnimation()
    }
}

export function startAnimation() : void {
    animate();
    isAnimating = true;
}

export function stopAnimation() : void {
    if (animationId != null) {
        cancelAnimationFrame(animationId);
    }
    isAnimating = false;
}

function animate() : void {
    handleInput()
    animationId = requestAnimationFrame(animate);
}