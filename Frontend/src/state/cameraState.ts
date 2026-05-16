import { DEFAULT_SCALE } from "../config/constants";
import { Camera } from "../types/camera";

export const camera : Camera = {
    rotation: 0,
    //left top corner of camera, not center
    offsetX: -window.innerWidth / 2,
    offsetY: -window.innerHeight / 2,
    scale: DEFAULT_SCALE,
    brightness: 0.8,
};