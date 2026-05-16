import { Star } from "./star";

export interface GalaxyState {
    stars: Star[];
    maxRadius: number;
    minRadius: number;
}