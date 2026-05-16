import { galaxy } from '../state/galaxyState';
import { gameWindow } from '../state/gameWindowState';
import { colors, shipColor, enemyColor, hoverColor, connectionLineColor, movementLineColor, centerColor, centerGlowColor, shipOutlineColor } from '../config/colors';
import { camera } from '../state/cameraState';
import { Star } from '../types/star';
import { polarDistance } from '../math/polarDistance';
import { interaction } from '../state/interactionState';
import { calculateCoordinates } from '../math/calculate_coordinates';
import { useSpaceshipStore } from '../vue/stores/spaceshipStore';
import { useSpaceshipActionsStore } from '../vue/stores/spaceshipActionsStore';
import { useSpaceshipResourcesStore } from '../vue/stores/spaceshipResourcesStore';

export function drawGalaxy(ctx: CanvasRenderingContext2D) {
    ctx.clearRect(0, 0, gameWindow.width, gameWindow.height);

    interaction.interactiveStars = [];

    let shipCoords: { x: number, y: number } | null = null;

    const spaceshipStore = useSpaceshipStore();
    const actionsStore = useSpaceshipActionsStore();
    const resourcesStore = useSpaceshipResourcesStore();
    const isMoving = actionsStore.isMoving && actionsStore.movementStatus;
    if (spaceshipStore.hasSpaceship && spaceshipStore.spaceship && !isMoving) {
        shipCoords = calculateCoordinates({
            radius: spaceshipStore.spaceship.locatedRadius,
            angleMilliradians: spaceshipStore.spaceship.locatedAngleMilliradians,
        } as Star);

        drawStar(ctx, {
            radius: spaceshipStore.spaceship.locatedRadius,
            angleMilliradians: spaceshipStore.spaceship.locatedAngleMilliradians,
            type: '-'
        }, IconType.Ship);
    }
    const shipAngle = spaceshipStore.spaceship
        ? spaceshipStore.spaceship.locatedAngleMilliradians / 1000 : -1;

    // Рисуем звезды
    galaxy.stars.forEach(star => {
        const isInteractive = spaceshipStore.spaceship != null
            ? polarDistance(
                star.radius, star.angleMilliradians / 1000,
                spaceshipStore.spaceship.locatedRadius, shipAngle)
                    <= resourcesStore.condition.maxDistance
            : false;

        drawStar(ctx, star, isInteractive ? IconType.InteractiveStar : IconType.Star);

        if (isInteractive) {
            interaction.interactiveStars.push({star});
        }
    });

    // Рисуем линию от корабля до наведенной звезды
    if (shipCoords && interaction.hoveredStar && !actionsStore.isMoving) {
        const hoveredCoords = calculateCoordinates(interaction.hoveredStar.star);
        drawConnectionLine(ctx, shipCoords, hoveredCoords);
    }

    // Рисуем процесс движения
    if (actionsStore.movementStatus) {
        const fromCoords = calculateCoordinates({
            radius: actionsStore.movementStatus.from.radius,
            angleMilliradians: actionsStore.movementStatus.from.angleMilliradians
        } as Star);
        const toCoords = calculateCoordinates({
            radius: actionsStore.movementStatus.to.radius,
            angleMilliradians: actionsStore.movementStatus.to.angleMilliradians
        } as Star);

        drawMovementLine(ctx, fromCoords, toCoords);

        const started = new Date(actionsStore.movementStatus.started).getTime();
        const ends = new Date(actionsStore.movementStatus.ends).getTime();
        const now = Date.now();
        const progress = Math.min(1, Math.max(0, (now - started) / (ends - started)));

        const movingShipCoords = {
            x: fromCoords.x + (toCoords.x - fromCoords.x) * progress,
            y: fromCoords.y + (toCoords.y - fromCoords.y) * progress
        };
        drawMovingShip(ctx, movingShipCoords);
    }

    drawCenter(ctx);
}

function drawStar(ctx: CanvasRenderingContext2D, star: Star, type: IconType = IconType.Star) {
    const coords = calculateCoordinates(star);
    const x = coords.x;
    const y = coords.y;

    if (x < 0 || x > gameWindow.width || y < 0 || y > gameWindow.height) {
        return;
    }

    switch (type) {
        case IconType.InteractiveStar:
        case IconType.Star:
            // Цвет на основе радиуса
            const radiusRange = galaxy.maxRadius - galaxy.minRadius;
            const colorIndex = radiusRange > 0
                ? Math.floor((star.radius - galaxy.minRadius) / radiusRange * (colors.length - 1))
                : 0;
            const color = colors[Math.min(colorIndex, colors.length - 1)];

            // Размер звезды зависит от радиуса
            let mod = (star.radius / galaxy.maxRadius);
            mod *= camera.scale / 2;

            let size = 1 + mod;

            // Рисуем с прозрачностью
            ctx.beginPath();
            ctx.arc(x, y, size, 0, Math.PI * 2);
            ctx.fillStyle = color;
            ctx.globalAlpha = camera.brightness;
            ctx.fill();
            
            // Подсветка при наведении
            if (interaction.hoveredStar?.star === star) {
                ctx.strokeStyle = hoverColor;
                ctx.lineWidth = 2;
                ctx.stroke();
            }

            // Add glow to all stars based on size
            ctx.shadowColor = color;
            ctx.shadowBlur = 3 + mod * 10;
            ctx.fill();
            ctx.shadowBlur = 0;
            break;
        case IconType.Ship:
            const shipRadius = camera.scale / 5;

            ctx.save();
            ctx.translate(x, y);
            // ctx.rotate(shipAngleRad);

            ctx.beginPath();
            ctx.moveTo(shipRadius, 0);
            ctx.lineTo(-shipRadius * 0.7, -shipRadius * 0.6);
            ctx.lineTo(-shipRadius * 0.4, 0);
            ctx.lineTo(-shipRadius * 0.7, shipRadius * 0.6);
            ctx.closePath();
            ctx.fillStyle = shipColor;
            ctx.globalAlpha = camera.brightness;
            ctx.shadowColor = shipColor;
            ctx.shadowBlur = 15;
            ctx.fill();

            ctx.beginPath();
            ctx.moveTo(-shipRadius * 0.3, -shipRadius * 0.3);
            ctx.lineTo(-shipRadius * 0.5, -shipRadius * 0.15);
            ctx.lineTo(-shipRadius * 0.5, shipRadius * 0.15);
            ctx.lineTo(-shipRadius * 0.3, shipRadius * 0.3);
            ctx.closePath();
            ctx.fillStyle = enemyColor;
            ctx.fill();

            ctx.shadowBlur = 0;
            ctx.restore();

            const r = useSpaceshipResourcesStore().condition.maxDistance * camera.scale;
            ctx.beginPath();
            ctx.ellipse(x, y, r, r*0.3, 0, 0, Math.PI * 2);
            ctx.strokeStyle = shipOutlineColor;
            ctx.globalAlpha = camera.brightness;
            ctx.stroke()
            break;
    }
}

enum IconType {
    Star,
    InteractiveStar,
    Ship
}

function drawCenter(ctx: CanvasRenderingContext2D) {
    ctx.beginPath();
    ctx.arc(-camera.offsetX, -camera.offsetY, 10, 0, Math.PI * 2);
    const gradient = ctx.createRadialGradient(-camera.offsetX, -camera.offsetY, 0, -camera.offsetX, -camera.offsetY, 20);
    gradient.addColorStop(0, centerGlowColor);
    gradient.addColorStop(1, centerColor);
    ctx.fillStyle = gradient;
    ctx.shadowColor = centerColor;
    ctx.shadowBlur = 20;
    ctx.fill();
    ctx.shadowBlur = 0;
}

function drawConnectionLine(ctx: CanvasRenderingContext2D, from: { x: number, y: number }, to: { x: number, y: number }) {
    ctx.beginPath();
    ctx.moveTo(from.x, from.y);
    ctx.lineTo(to.x, to.y);
    ctx.strokeStyle = connectionLineColor;
    ctx.lineWidth = 2;
    ctx.globalAlpha = 0.6;
    ctx.setLineDash([5, 5]);
    ctx.stroke();
    ctx.setLineDash([]);
    ctx.globalAlpha = 1.0;
}

function drawMovementLine(ctx: CanvasRenderingContext2D, from: { x: number, y: number }, to: { x: number, y: number }) {
    ctx.beginPath();
    ctx.moveTo(from.x, from.y);
    ctx.lineTo(to.x, to.y);
    ctx.strokeStyle = movementLineColor;
    ctx.lineWidth = 3;
    ctx.globalAlpha = 0.8;
    ctx.shadowColor = movementLineColor;
    ctx.shadowBlur = 10;
    ctx.stroke();
    ctx.shadowBlur = 0;
    ctx.globalAlpha = 1.0;
}

function drawMovingShip(ctx: CanvasRenderingContext2D, coords: { x: number, y: number }) {
    const shipRadius = camera.scale / 5;

    ctx.save();
    ctx.translate(coords.x, coords.y);

    ctx.beginPath();
    ctx.moveTo(shipRadius, 0);
    ctx.lineTo(-shipRadius * 0.7, -shipRadius * 0.6);
    ctx.lineTo(-shipRadius * 0.4, 0);
    ctx.lineTo(-shipRadius * 0.7, shipRadius * 0.6);
    ctx.closePath();
    ctx.fillStyle = shipColor;
    ctx.shadowColor = shipColor;
    ctx.shadowBlur = 20;
    ctx.fill();

    ctx.beginPath();
    ctx.moveTo(-shipRadius * 0.3, -shipRadius * 0.3);
    ctx.lineTo(-shipRadius * 0.5, -shipRadius * 0.15);
    ctx.lineTo(-shipRadius * 0.5, shipRadius * 0.15);
    ctx.lineTo(-shipRadius * 0.3, shipRadius * 0.3);
    ctx.closePath();
    ctx.fillStyle = enemyColor;
    ctx.fill();

    ctx.shadowBlur = 0;
    ctx.restore();
}