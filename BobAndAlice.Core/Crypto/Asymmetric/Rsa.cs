using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using BobAndAlice.Core.Maths;

namespace BobAndAlice.Core.Crypto.Asymmetric
{
    public class Rsa
    {
        public RsaKeyPair Keys { get; set; }

        #region Key Generation
        private readonly static ConcurrentDictionary<BigInteger, bool> usedPrimesFound = new ConcurrentDictionary<BigInteger, bool>();

        public static RsaKeyPair GenerateKeys(int keyBytesSize = 128)
            => new RsaKeyPair()
            {
                PrivateKey = generateKey(keyBytesSize),
                PublicKey = generateKey(keyBytesSize),
            };

        private static BigInteger generateKey(int keyBytesSize)
        {

            if (tryGetAlreadyGeneratedPrime(out var prime))
            {
                return prime;
            }

            var prng = new Prng();
            var primalityTest = new MillerRabin();
            var queue = new ConcurrentQueue<BigInteger>();

            while (true)
            {
                fillQueueWithPossiblePrimes(queue, prng, keyBytesSize);

                Parallel.ForEach(queue, (x, i) =>
                {
                    if (primalityTest.IsPrime(x) && !usedPrimesFound.ContainsKey(x))
                    {
                        usedPrimesFound[x] = false;
                    };
                });

                if (tryGetAlreadyGeneratedPrime(out prime))
                {
                    return prime;
                }
            }
        }

        private static bool tryGetAlreadyGeneratedPrime(out BigInteger prime)
        {
            prime = 0;

            var foundPrime = usedPrimesFound.FirstOrDefault(kv => kv.Value == false).Key;
            if (foundPrime != default)
            {
                prime = foundPrime;
                usedPrimesFound[prime] = true;
                return true;
            }

            return false;
        }

        private static BigInteger generatePossiblePrimeNumber(Prng prng, int bytesSize)
        {
            BigInteger result;

            do
            {
                result = prng.Next(bytesSize);

                if (BigInteger.GreatestCommonDivisor(result, Constants.SmallPrimesProduct.Value) != 1)
                {
                    result = 0;
                }
            } while (result == 0);

            return result;
        }

        private static void fillQueueWithPossiblePrimes(ConcurrentQueue<BigInteger> queue, Prng prng, int bytesSize, int? quantity = null)
        {
            queue.Clear();
            var possiblePrime = generatePossiblePrimeNumber(prng, bytesSize);
            for (int i = 0; i < (quantity ?? Environment.ProcessorCount * 6); i++)
            {
                possiblePrime += Constants.SmallPrimesProduct.Value;
                queue.Enqueue(possiblePrime);
            }
        }
        
        #endregion
    }
}