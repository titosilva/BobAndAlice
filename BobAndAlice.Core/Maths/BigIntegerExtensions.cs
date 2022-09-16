using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BobAndAlice.Core.Maths
{
    public static class BigIntegerExtensions
    {
        // Source: https://en.wikipedia.org/wiki/Extended_Euclidean_algorithm
        public static BigInteger? ModInverse(this BigInteger number, BigInteger modulus)
        {
            var t = new BigInteger(0);
            var newt = new BigInteger(1);
            var r = modulus;
            var newr = number;

            while (newr != 0)
            {
                var quotient = r / newr;
                var temp = newt;

                newt = t - quotient* newt;
                t = temp;

                temp = newr;
                newr = r - quotient* newr;
                r = temp;
            }

            if (r > 1)
            {
                return null;
            }

            if (t < 0)
            {
                t = t + modulus;
            }

            return t;
        }

        public static Binary ToBinary(this BigInteger number)
        {
            var binary = new Binary();
            var numberCopy = number;

            while (numberCopy > 0)
            {
                binary.Content.Add((byte)(numberCopy & 0xff));
                numberCopy >>= 8;
            }

            return binary;
        }
    }
}
