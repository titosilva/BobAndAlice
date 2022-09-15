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
            var keyPair = Rsa.GenerateKeys();
            var primalityTest = new MillerRabin();
            Assert.True(primalityTest.IsPrime(keyPair.PrivateKey));
            Assert.True(primalityTest.IsPrime(keyPair.PublicKey));
        }
    }
}