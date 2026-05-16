export function polarDistance(
    r1: number, a1: number,
    r2: number, a2: number
): number {
    const angleDiff = a1 - a2;
    // d² = r1² + r2² - 2*r1*r2*cos(Δ angle)
    return Math.sqrt(r1 * r1 + r2 * r2 - 2 * r1 * r2 * Math.cos(angleDiff));
}