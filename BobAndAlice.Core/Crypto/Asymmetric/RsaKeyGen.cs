using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using BobAndAlice.Core.Maths;

namespace BobAndAlice.Core.Crypto.Asymmetric
{
    public class RsaKeyGen
    {
        private readonly static Prng prng = new Prng();
        private readonly static ConcurrentDictionary<BigInteger, bool> usedPrimesFound = new ConcurrentDictionary<BigInteger, bool>();

        public static RsaKeyPair GenerateKeys(int primeBytesSize = 128)
        {
            var (p, q) = GeneratePrimes(primeBytesSize);

            return GenerateKeysFromPrimes(p, q, primeBytesSize);
        }

        public static (BigInteger P, BigInteger Q) GeneratePrimes(int primeBytesSize)
            => (generatePrime(primeBytesSize), generatePrime(primeBytesSize));

        public static RsaKeyPair GenerateKeysFromPrimes(BigInteger p, BigInteger q, int primeBytesSize)
        {
            var modulus = p * q;
            var modulusPhi = (p - 1) * (q - 1);

            BigInteger e;
            BigInteger? d;
            do
            {
                do
                {
                    e = prng.Next(primeBytesSize >> 1);
                } while (BigInteger.GreatestCommonDivisor(e, modulusPhi) != 1);

                d = e.ModInverse(modulusPhi);
            } while (!d.HasValue);


            return new RsaKeyPair()
            {
                Primes = (p, q),
                PublicKey = new RsaKey(modulus, e),
                PrivateKey = new RsaKey(modulus, d.Value),
            };
        }


        private static BigInteger generatePrime(int primeBytesSize)
        {

            if (tryGetAlreadyGeneratedPrime(out var prime))
            {
                return prime;
            }

            var primalityTest = new MillerRabin();
            var queue = new ConcurrentQueue<BigInteger>();

            while (true)
            {
                fillQueueWithPossiblePrimes(queue, prng, primeBytesSize);

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
    }
}