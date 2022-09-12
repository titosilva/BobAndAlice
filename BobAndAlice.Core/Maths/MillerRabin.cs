using System.Numerics;

namespace BobAndAlice.Core.Maths
{
    public class MillerRabin
    {
        public bool IsPrime(BigInteger number, int maxRounds = 20)
        {
            var exponent = number - 1;
            
            for (int i = 0; i < maxRounds; i++)
            {
                var mod = BigInteger.ModPow(2, exponent, number);

                if (mod != 1)
                {
                    return mod == number - 1;
                }

                if ((exponent & 0x01) != 0)
                {
                    break;
                }
                
                exponent = exponent >> 1;
            }

            return true;
        }
    }
}