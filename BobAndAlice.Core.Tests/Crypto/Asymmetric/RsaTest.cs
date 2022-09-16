using BobAndAlice.Core.Crypto.Asymmetric;
using BobAndAlice.Core.Maths;
using Xunit;

namespace BobAndAlice.Core.Tests.Crypto.Asymmetric
{
    public class RsaTest
    {
        [Fact]
        public void GenerateKeys__ShouldReturnRsaKeyPair__WithTwoPrimes()
        {
            var keyPair = RsaKeyGen.GenerateKeys();
            var primalityTest = new MillerRabin();
            Assert.True(primalityTest.IsPrime(keyPair.Primes.Item1));
            Assert.True(primalityTest.IsPrime(keyPair.Primes.Item2));
        }

        [Fact]
        public void GenerateKeys__ShouldReturnRsaKeyPair__WithTwoMultiplicativeInversesAndSameModulus()
        {
            var keyPair = RsaKeyGen.GenerateKeys();

            var modulus = keyPair.PrivateKey.Modulus;
            Assert.Equal(modulus, keyPair.PublicKey.Modulus);

            var verification = (keyPair.PrivateKey.Value * keyPair.PublicKey.Value) % modulus;
            Assert.Equal(1, verification);
        }
    }
}