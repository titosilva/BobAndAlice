using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace BobAndAlice.Core.Maths
{
    public class MillerRabin
    {
        public bool IsPrime(BigInteger number, int maxRounds = 20)
        {
            var oddExponent = findHighestOddDivisor(number - 1, out var divisionsBy2);
            var random = new Random();
            var maxRandomNumber = number > 1_000_000 ? 1_000_000 : (int)number;
            bool isComposite;

            for (int i = 0; i < maxRounds; i++)
            {
                var expBase = random.Next(2, maxRandomNumber);
                var mod = BigInteger.ModPow(expBase, oddExponent, number);

                if (mod == 1 || mod == number - 1)
                {
                    // The number might be prime
                    continue;
                }

                isComposite = true;
                for (int j = 1; j < divisionsBy2; j++)
                {
                    mod = (mod * mod) % number;

                    // If this condition never happens, then the number is composite
                    if (mod == number - 1)
                    {
                        isComposite = false;
                        break;
                    }
                }

                if (isComposite)
                {
                    // The number is composite
                    return false;
                }
            }

            return true;
        }

        private BigInteger findHighestOddDivisor(BigInteger number, out int divisionsBy2)
        {
            var result = BigInteger.Abs(number);
            divisionsBy2 = 0;

            while (result.IsEven)
            {
                result >>= 1;
                divisionsBy2 += 1;
            }

            return result;
        }
    }
}