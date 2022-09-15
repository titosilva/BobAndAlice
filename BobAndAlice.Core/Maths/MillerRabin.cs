using System;
using System.Collections.Generic;
using System.Numerics;

namespace BobAndAlice.Core.Maths
{
    public class MillerRabin
    {
        public bool IsPrime(BigInteger number, int maxRounds = 20)
        {
            if (number.IsEven)
            {
                return false;
            }

            var oddExponent = findHighestOddDivisor(number - 1, out var divisionsBy2);
            var isComposite = false;

            for (int i = 0; i < maxRounds && !isComposite; i++)
            {
                var expBase = new Random().Next(2, number > 1000? 1000 : (int) number);
                var mod = BigInteger.ModPow(expBase, oddExponent, number);
                
                if (mod == 1 || mod == number - 1)
                {
                    continue;
                }

                isComposite = true;
                for (int j = 0; j < divisionsBy2 - 1; j++)
                {
                    mod = BigInteger.ModPow(mod, 2, number);
                    if (mod == number - 1)
                    {
                        isComposite = false;
                        break;
                    }
                }
            }

            return !isComposite;
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