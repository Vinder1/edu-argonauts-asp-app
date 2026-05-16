export interface BattleMember {
    id: string;
    battleId: string;
    health: number;
    maxHealth: number;
    power: number;
    isAI: boolean;
    name?: string;
    move: string;
    targetId?: string;
}

export interface BattleStatus {
    members: BattleMember[];
}