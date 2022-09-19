using BobAndAlice.Core.Crypto.Asymmetric;
using BobAndAlice.Core.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobAndAlice.Core.Crypto.Signatures
{
    public class Signature
    {
        public string OriginalFileName { get; set; }
        public Binary EncryptedMessage { get; set; }
        public Binary SignedHashAndParameters { get; set; }
        public RsaKey SignerPublicKey { get; set; }
    }

    public class SignatureHashAndParameters
    {
        public Binary Hash { get; set; }
        public Binary AesEncryptionKey { get; set; }
        public byte AesPaddingSize { get; set; }
    }
}
