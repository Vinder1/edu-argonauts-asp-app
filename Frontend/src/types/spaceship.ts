export interface SpaceshipContainer {
    spaceship: Spaceship;
    hasSpaceship: boolean;
}

export interface Spaceship {
    ownerId: string
    galaxyVersion: number,
    locatedRadius: number,
    locatedAngleMilliradians: number
}

export interface NamedSpaceship {
    id: string;
    name: string;
    battlePower: number;
}