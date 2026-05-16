export interface Star {
    radius: number;
    angleMilliradians: number;
}

export interface MovementStatus {
    started: string;
    ends: string;
    from: Star;
    to: Star;
}