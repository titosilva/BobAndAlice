export interface UserKeyModel {
    modulusBase64: string;
    publicKeyBase64: string;
    privateKeyBase64: string;
}

export interface CreateUserKeysRequest {
    userId: string;
    prime1Base64: string;
    prime2Base64: string;
}

export interface GeneratePrimesResponse {
    prime1Base64: string;
    prime2Base64: string;
}