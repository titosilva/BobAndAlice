using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BobAndAlice.Core
{
    public class Constants
    {
        public static readonly List<int> SmallPrimes = new List<int>() {
            2,3,5,7,11,13,17,19,23,29
        };

        public static readonly Lazy<BigInteger> SmallPrimesProduct = new Lazy<BigInteger>(() =>
        {
            var result = new BigInteger(SmallPrimes.First());
            foreach (var p in SmallPrimes.Skip(1))
            {
                result *= p;
            }

            return result;
        });
    }
}
