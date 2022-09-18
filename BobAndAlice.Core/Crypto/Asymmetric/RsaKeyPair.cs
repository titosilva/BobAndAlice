using System.Numerics;

namespace BobAndAlice.Core.Crypto.Asymmetric
{
    public class RsaKeyPair
    {
        public (BigInteger, BigInteger) Primes { get; set; }
        public RsaKey PrivateKey { get; set; }
        public RsaKey PublicKey { get; set; }

        public BigInteger Modulus 
            => Primes.Item1 * Primes.Item2;
    }
}