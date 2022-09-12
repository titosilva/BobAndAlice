using System.Numerics;
using BobAndAlice.Core.Maths;
using Xunit;

namespace BobAndAlice.Core.Tests.Maths
{
    public class MillerRabinTest
    {
        [Fact]
        public void MillerRabinTest__ShouldReturnTrue__ForSmallPrimes()
        {
            var test = new MillerRabin();

            Assert.True(test.IsPrime(7));
            Assert.True(test.IsPrime(7817));
            Assert.True(test.IsPrime(5483));
            Assert.True(test.IsPrime(5441));
            Assert.True(test.IsPrime(5641));
        }

        [Fact]
        public void MillerRabinTest__ShouldReturnFalse__ForSmallComposites()
        {
            var test = new MillerRabin();
            
            Assert.False(test.IsPrime(6));
            Assert.False(test.IsPrime(91));
            Assert.False(test.IsPrime(8962));
            Assert.False(test.IsPrime(654729075));
            Assert.False(test.IsPrime(30929603));
        }

        [Fact]
        public void MillerRabinTest__ShouldReturnTrue__ForLargePrimes()
        {
            var test = new MillerRabin();
            
            // 2 ^ 607 - 1
            var mersennePrime = (new BigInteger(1) << 607) - 1;
            Assert.True(test.IsPrime(mersennePrime));
            
            // 2 ^ 521 - 1
            mersennePrime = (new BigInteger(1) << 521) - 1;
            Assert.True(test.IsPrime(mersennePrime));
            
            // 2 ^ 1279 - 1
            mersennePrime = (new BigInteger(1) << 1279) - 1;
            Assert.True(test.IsPrime(mersennePrime));
        }
    }
}