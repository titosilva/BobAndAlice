using System.Numerics;

namespace BobAndAlice.Core.Crypto.Asymmetric
{
    public class RsaKeyPair
    {
        public BigInteger PrivateKey { get; set; }
        public BigInteger PublicKey { get; set; }
    }
}