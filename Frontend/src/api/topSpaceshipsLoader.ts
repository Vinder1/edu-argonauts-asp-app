import { NamedSpaceship } from "../types/spaceship";
import { fetchWithAuth } from "./fetchWithAuth";

export async function loadTopSpaceships(): Promise<NamedSpaceship[]> {
    try {
        let response = await fetchWithAuth('/api/rating/top10');
        if (!response.ok) {
            return [];
        }
        return await response.json() as NamedSpaceship[];
    } catch (error: unknown) {
        console.error('Error loading top spaceships:', error);
        return [];
    }
}
