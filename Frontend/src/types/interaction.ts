import { Star } from "./star";

// src/types/interaction.ts
export interface InteractiveStar {
    star: Star;
}

export interface InteractionState {
    interactiveStars: InteractiveStar[];
    hoveredStar: InteractiveStar | null;
    HIT_RADIUS: number;
}