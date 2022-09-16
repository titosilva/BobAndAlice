using BobAndAlice.Core.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BobAndAlice.Core.Tests.Maths
{
    public class BigIntegerExtensionsTests
    {
        [Fact]
        public void ModInverse__ShouldReturnMultiplicativeInverseOrNull__ForGivenNumberAndModulus()
        {
            // Tool for generating examples: https://planetcalc.com/3311/
            var number = new BigInteger(312314541);
            var modulus = new BigInteger(3434533);
            Assert.Equal(new BigInteger(2845548), number.ModInverse(modulus));

            number = new BigInteger(8);
            modulus = new BigInteger(23);
            Assert.Equal(new BigInteger(3), number.ModInverse(modulus));

            number = new BigInteger(453784123123);
            modulus = new BigInteger(235);
            Assert.Equal(new BigInteger(212), number.ModInverse(modulus));
        }

        [Fact]
        public void ModInverse__ShouldReturnMultiplicativeInverseOrNull__ForRandomNumberAndModulus()
        {
            var prng = new Prng();

            for (int i = 0; i < 10; i++)
            {
                var number = prng.Next(512);
                var modulus = prng.Next(1024);
                var inverse = number.ModInverse(modulus);
                var verification = (number * inverse) % modulus;

                if (verification.HasValue)
                {
                    Assert.Equal(1, verification);
                }
            }
        }

        [Fact]
        public void ToBinary__ShouldReturnABinary__WithSameBytesAsNumber()
        {
            var number = new Prng().Next(1024);
            var binary = number.ToBinary();

            var byteCount = 0;
            foreach (var b in binary.Content)
            {
                var expectedByte = (byte)((number >> (byteCount * 8)) & 0xff);
                Assert.Equal(expectedByte, b);
                byteCount++;
            }
        }

        [Fact]
        public void ToBinaryAndToBigIntegerAreInverses()
        {
            for (int i = 0; i < 10; i++)
            {
                var number = new Prng().Next(1024);
                var binary = number.ToBinary();
                Assert.Equal(number, binary.ToBigInteger());
            }
        }
    }
}
