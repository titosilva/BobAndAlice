using System.Collections.Concurrent;
using System.Numerics;
using BobAndAlice.Core.Maths;

namespace BobAndAlice.Core.Crypto.Asymmetric
{
    public class Rsa
    {
        public RsaKeyPair Keys { get; set; }

        #region Key Generation
        
        public static RsaKeyPair GenerateKeys(int keyBytesSize = 1025)
            => new RsaKeyPair()
            {
                PrivateKey = generateKey(keyBytesSize),
                PublicKey = generateKey(keyBytesSize),
            };

        private static BigInteger generateKey(int keyBytesSize = 128)
        {
            var prng = new Prng();
            var primalityTest = new MillerRabin();

            var counter = 0;
            BigInteger result;
            do
            {
                result = generatePossiblePrimeNumber(prng, keyBytesSize);
                counter++;
            } while (!primalityTest.IsPrime(result));

            return result;
        }

        private static BigInteger generatePossiblePrimeNumber(Prng prng, int bytesSize)
        {
            BigInteger result;

            do
            {
                result = prng.Next(bytesSize);

                foreach (var smallPrime in Constants.SmallPrimes100)
                {
                    if (result % smallPrime == 0)
                    {
                        result = 0;
                        break;
                    }
                }
            } while (result == 0);

            return result;
        }
        
        #endregion
    }
}