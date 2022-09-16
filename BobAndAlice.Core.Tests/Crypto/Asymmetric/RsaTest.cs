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

            var modulus = keyPair.Primes.Item1 * keyPair.Primes.Item2;
            Assert.Equal(modulus, keyPair.PublicKey.Modulus);
            Assert.Equal(modulus, keyPair.PrivateKey.Modulus);

            var phi = (keyPair.Primes.Item1 - 1) * (keyPair.Primes.Item2 - 1);
            var verification = (keyPair.PrivateKey.Value * keyPair.PublicKey.Value) % phi;
            Assert.Equal(1, verification);
        }

        [Fact]
        public void TrapdoorPrivateKey__ShouldUndo__TrapdoorPublicKey()
        {
            var keyPair = RsaKeyGen.GenerateKeys();

            var msg = new Prng().Next(64);
            var rsa = new RsaOAEP();

            var cipher = rsa.Trapdoor(msg, keyPair.PublicKey);
            Assert.Equal(msg, rsa.Trapdoor(cipher, keyPair.PrivateKey));
        }
        
        [Fact]
        public void TrapdoorPublicKey__ShouldUndo__TrapdoorPrivateKey()
        {
            var keyPair = RsaKeyGen.GenerateKeys();

            var msg = new Prng().Next(64);
            var rsa = new RsaOAEP();

            var cipher = rsa.Trapdoor(msg, keyPair.PrivateKey);
            Assert.Equal(msg, rsa.Trapdoor(cipher, keyPair.PublicKey));
        }
    }
}