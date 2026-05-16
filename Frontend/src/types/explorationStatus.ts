export interface Enemy {
    name: string;
    level: number;
    alive: boolean;
}

export interface ExplorationStatus {
    level: number;
    enemy: Enemy | null;
}