export interface UserContainer {
    player: UserDto;
    hasPlayer: boolean;
}

export interface UserDto {
    id: string;
    name: string;
    login: string;
    email: string;
    role: string;
}