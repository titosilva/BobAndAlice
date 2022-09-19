using System;

namespace BobAndAlice.App.Models.Signatures
{
    public class SignatureModel
    {
        public Guid Id { get; set; }

        public string Failure { get; set; }

        public string EncryptedDataBase64 { get; set; }
        public string SignatureParametersBase64 { get; set; }
        public string PublicKeyModulusBase64 { get; set; }
        public string PublicKeyBase64 { get; set; }

        public string DecryptedDataBase64 { get; set; }
        public string DataHashBase64 { get; set; }
        public string EncryptioKeyBase64 { get; set; }
        public int EncryptionPaddingSize { get; set; }
    }
}
