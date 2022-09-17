using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobAndAlice.Core.Crypto.Symmetric
{
    public class AES
    {
        #region Block sizes supported
        public enum AESSupportedBlockSizes
        {
            Bits128,
            Bits192,
            Bits256,
        }

        public int ToBitSize(AESSupportedBlockSizes blockSize)
            => blockSize switch
            {
                AESSupportedBlockSizes.Bits128 => 128,
                AESSupportedBlockSizes.Bits192 => 192,
                AESSupportedBlockSizes.Bits256 => 256,
                _ => throw new ArgumentException("Not supported AES block size"),
            };

        public int ToByteSize(AESSupportedBlockSizes blockSize)
            => ToBitSize(blockSize) >> 3;
        #endregion
    }
}
