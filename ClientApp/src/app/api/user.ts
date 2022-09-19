export interface UserModel {
    id: string;
    cpf: string;
    name: string;
}

export interface UserData {
    cpf: string;
    name: string;
    password: string;
}

export interface LoginRequest {
    cpf: string;
    password: string;
}