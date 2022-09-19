using BobAndAlice.Core.Crypto.Signatures;

namespace BobAndAlice.App.Configuration
{
    public class AppConfiguration
    {
        public int RsaPrimeSizeBytes { get; set; } = 129;

        public string UploadsBasePath { get; set; } = "C:\\Temp\\BobAndAlice";

        public Signer.SignerModes SignerMode { get; set; } = Signer.SignerModes.Hash256Encryption128;
    }
}
