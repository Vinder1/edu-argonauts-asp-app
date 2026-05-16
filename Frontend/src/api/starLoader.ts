import { galaxy } from "../state/galaxyState";
import { Star } from "../types/star";
import { drawGalaxy } from "../render/draw";
import { setRenderContext } from "../render/renderScheduler";
import { useGameStore } from "../vue/stores/gameStore";

const BATCHES = [
    { min: 0, max: 40 },
    { min: 40, max: 80 },
    { min: 80, max: 120 },
];

export async function loadStars() : Promise<void> {
    const loadingEl = document.getElementById('loading');
    const errorEl = document.getElementById('error');

    if (loadingEl == null || errorEl == null) {
        console.debug("loading and error elements not found");
        return;
    }

    loadingEl.style.display = 'block';
    errorEl.style.display = 'none';

    try {
        const allStars: Star[] = [];

        for (let i = 0; i < BATCHES.length; i++) {
            const { min, max } = BATCHES[i];
            loadingEl.textContent = `Loading stars... batch ${i + 1}/${BATCHES.length}`;
            const batch = await loadStarsByRadiusRange(min, max);
            allStars.push(...batch);
        }

        galaxy.stars = allStars;

        galaxy.maxRadius = Math.max(...galaxy.stars.map(s => s.radius));
        galaxy.minRadius = Math.min(...galaxy.stars.map(s => s.radius));

        const gameStore = useGameStore();
        gameStore.maxRadius = galaxy.maxRadius;
        gameStore.minRadius = galaxy.minRadius;
        gameStore.starCount = galaxy.stars.length;

        const canvas = document.getElementById('galaxyCanvas') as HTMLCanvasElement;
        const ctx = canvas.getContext('2d');
        if (ctx) {
            setRenderContext(ctx);
            drawGalaxy(ctx);
        }

    } catch (error : unknown) {
        console.error('Ошибка загрузки:', error);
        const gameStore = useGameStore();
        gameStore.error = error instanceof Error ? error.message : 'Unknown error';
        if (errorEl) {
            errorEl.textContent = `Ошибка загрузки: ${gameStore.error}`;
            errorEl.style.display = 'block';
        }
    } finally {
        loadingEl.style.display = 'none';
    }
}

export async function loadStarsByRadiusRange(minRadius: number, maxRadius: number): Promise<Star[]> {
    const response = await fetch(`/api/stars/byRadiusRange?minRadius=${minRadius}&maxRadius=${maxRadius}`);
    if (!response.ok) {
        throw new Error(`HTTP error: ${response.status}`);
    }
    const data = await response.json();
    return (data.stars as Star[]).filter(s => s !== null);
}
