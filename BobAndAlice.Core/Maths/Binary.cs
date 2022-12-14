using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BobAndAlice.Core.Maths
{
    public class Binary
    {
        #region Constructors
        public Binary()
        {
            Content = new List<byte>();
        }

        // If multiple binaries are received, concatenate them
        public Binary(params Binary[] bins)
        {
            Content = new List<byte>();

            for (int idx = 0; idx < bins.Length; idx++)
            {
                var bin = bins[idx];
                Content.AddRange(new List<byte>(bin.Content));
            }
        }

        public Binary(params byte[] bin)
        {
            Content = new List<byte>(bin);
        }

        public Binary(List<byte> content)
        {
            Content = new List<byte>(content);
        }

        public Binary(List<UInt32> words)
        {
            Content = new List<byte>();

            foreach (var word in words)
            {
                Content.Add((byte)(word & 0xff));
                Content.Add((byte)((word >> 8) & 0xff));
                Content.Add((byte)((word >> 16) & 0xff));
                Content.Add((byte)((word >> 24) & 0xff));
            }
        }

        public Binary(UInt32 word)
        {
            Content.Add((byte)(word & 0xff));
            Content.Add((byte)((word >> 8) & 0xff));
            Content.Add((byte)((word >> 16) & 0xff));
            Content.Add((byte)((word >> 24) & 0xff));
        }
        #endregion

        public List<byte> Content { get; set; } = new List<byte>();

        public int Length
            => Content.Count;

        public void PadRight(int bytesSize, byte value = 0)
        {
            for (int i = 0; i < bytesSize; i++)
            {
                Content.Insert(0, value);
            }
        }

        public List<byte[]> Split(int blockSize)
        {
            var contentBytes = Content.ToArray();
            var result = new List<byte[]>();

            var blockIdx = 0;
            while (blockIdx * blockSize < contentBytes.Length)
            {
                var nextBlock = new byte[blockSize];

                for (int i = 0; i < blockSize; i++)
                {
                    nextBlock[i] = contentBytes[blockIdx * blockSize + i];
                }
                
                result.Add(nextBlock);
                blockIdx++;
            }

            return result;
        }

        public Binary GetReversed()
        {
            var copy = new Binary(this);
            copy.Content.Reverse();
            return copy;
        }

        #region Conversions
        public BigInteger ToBigInteger()
        {
            var reversed = GetReversed();
            var result = new BigInteger(reversed.Content.First());

            foreach (var b in reversed.Content.Skip(1))
            {
                result <<= 8;
                result |= b;
            }

            return result;
        }

        public byte[] ToByteArray()
            => Content.ToArray();

        public List<UInt32> ToWords()
        {
            var wordCount = Length % 4 == 0 ? (Length >> 2) : (Length >> 2) + 1;
            var result = new List<UInt32>();

            for (int i = 0; i < wordCount; i++)
            {
                var wordBytes = Content.Skip(i * 4).Take(4).ToList();
                UInt32 word = 0;

                for (int j = 0; j < 4 && wordBytes.Count > j; j++)
                {
                    word |= (uint)(wordBytes[j] << (j * 8));
                }

                result.Add(word);
            }

            return result;
        }
        #endregion

        #region Operators
        public static Binary operator ^(Binary bin1, Binary bin2)
            => new Binary(bin1.Content.Select((value, index) => (byte)(value ^ bin2.Content[index])).ToList());

        public static bool operator ==(Binary bin1, Binary bin2)
        {
            if (bin1.Length != bin2.Length)
            {
                return false;
            }

            for (int i = 0; i < bin1.Length; i++)
            {
                if (bin1.Content[i] != bin2.Content[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(Binary bin1, Binary bin2)
        {
            if (bin1.Length != bin2.Length)
            {
                return true;
            }

            for (int i = 0; i < bin1.Length; i++)
            {
                if (bin1.Content[i] != bin2.Content[i])
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
