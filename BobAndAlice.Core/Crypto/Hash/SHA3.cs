using BobAndAlice.Core.Maths;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobAndAlice.Core.Crypto.Hash
{

    public class SHA3 // Wrapper around BouncyCastle's SHA3
    {
        #region Supported bit sizes
        public enum SHA3SupportedBitSizes
        {
            Bits224,
            Bits256,
            Bits384,
            Bits512,
        }

        public static int ToInt(SHA3SupportedBitSizes bitSize)
            => bitSize switch
            {
                SHA3SupportedBitSizes.Bits224 => 224,
                SHA3SupportedBitSizes.Bits256 => 256,
                SHA3SupportedBitSizes.Bits384 => 384,
                SHA3SupportedBitSizes.Bits512 => 512,
                _ => throw new ArgumentException("Not supported SHA-3 bit size"),
            };
        #endregion

        private readonly Sha3Digest digest;
        private int bitsSize;

        public SHA3(SHA3SupportedBitSizes bitSize)
        {
            bitsSize = ToInt(bitSize);
            digest = new Sha3Digest(bitsSize);
        }

        public Binary Hash(Binary data)
        {
            digest.Reset();
            digest.BlockUpdate(data.Content.ToArray(), 0, data.Length);

            // Bytes size = bits size / 8, but will sum 1 to make sure there is enough space
            var resultBytesSize = (bitsSize >> 3);
            var result = new byte[resultBytesSize];

            digest.DoFinal(result, 0);
            return new Binary(result);
        }
    }
}
