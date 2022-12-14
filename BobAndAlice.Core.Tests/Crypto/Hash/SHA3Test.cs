using BobAndAlice.Core.Crypto.Hash;
using BobAndAlice.Core.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BobAndAlice.Core.Tests.Crypto.Hash
{
    public class SHA3Test
    {
        [Fact]
        public void SHA3__ShouldReturnHashes__WithSpecifiedBitLength()
        {
            var random = new Random();
            var prng = new Prng();

            var sha3 = new SHA3(SHA3.SHA3SupportedBitSizes.Bits224);
            var result = sha3.Hash(prng.Next(random.Next(256, 1024)).ToBinary());
            Assert.Equal(224 >> 3, result.Length);

            sha3 = new SHA3(SHA3.SHA3SupportedBitSizes.Bits256);
            result = sha3.Hash(prng.Next(random.Next(256, 1024)).ToBinary());
            Assert.Equal(256 >> 3, result.Length);

            sha3 = new SHA3(SHA3.SHA3SupportedBitSizes.Bits384);
            result = sha3.Hash(prng.Next(random.Next(256, 1024)).ToBinary());
            Assert.Equal(384 >> 3, result.Length);

            sha3 = new SHA3(SHA3.SHA3SupportedBitSizes.Bits512);
            result = sha3.Hash(prng.Next(random.Next(256, 1024)).ToBinary());
            Assert.Equal(512 >> 3, result.Length);
        }
    }
}
