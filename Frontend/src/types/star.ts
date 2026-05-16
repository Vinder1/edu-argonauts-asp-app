import { StarVisit } from "./starVisit";

export interface Star {
    radius: number;
    angleMilliradians: number;
    type: string;
}

export interface StarInfo {
    star: Star,
    visit: StarVisit
}