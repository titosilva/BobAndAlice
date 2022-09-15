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
                result = prng.Next(keyBytesSize);

                // Never go on with even numbers
                if (result.IsEven)
                {
                    // If an even number is generated, turn it into an odd number
                    result += 1;
                }

                counter++;
            } while (!primalityTest.IsPrime(result));

            return result;
        }
        
        #endregion
    }
}