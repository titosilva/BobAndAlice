import { FileModel } from "./files";

export interface SignatureModel {
    id: string;
    fileName: string;
    
    failure: string;
    encryptedDataBase64: string;
    signatureParametersBase64: string;
    publicKeyModulusBase64: string;
    publicKeyBase64: string;
    decryptedDataBase64: string;
    dataHashBase64: string;
    encryptionKeyBase64: string;
    encryptionPaddingSize: number;
}

export interface SignatureData {
    userId: string;
    file: FileModel;
}
