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

        public Binary(byte[] bin)
        {
            Content = new List<byte>(bin);
        }

        public Binary(List<byte> content)
        {
            Content = new List<byte>(content);
        }

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

        public Binary GetReversed()
        {
            var copy = new Binary(this);
            copy.Content.Reverse();
            return copy;
        }

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

        #region Operators
        public static Binary operator ^(Binary bin1, Binary bin2)
            => new Binary(bin1.Content.Select((value, index) => (byte) (value ^ bin2.Content[index])).ToList());
        #endregion
    }
}
