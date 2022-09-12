using System.Numerics;
using BobAndAlice.Core.Maths;
using Xunit;

namespace BobAndAlice.Core.Tests.Maths
{
    public class PrngTest
    {
        [Fact]
        public void PrngNext__ShouldReturnABigInteger__WithGivenSize()
        {
            var random = new Prng();
            for (var length = 1; length < 20; length++)
            {
                var randomNumber = random.Next(length);
                
                Assert.True(randomNumber > BigInteger.Pow(2, (length - 1) * 8));
                Assert.True(randomNumber < BigInteger.Pow(2, length * 8));
            }
        }
    }
}