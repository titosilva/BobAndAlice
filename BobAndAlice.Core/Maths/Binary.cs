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
        public Binary()
        {
            Content = new List<byte>();
        }

        public Binary(Binary bin)
        {
            Content = new List<byte>(bin.Content);
        }

        public Binary(byte[] bin)
        {
            Content = new List<byte>(bin);
        }

        public List<byte> Content { get; set; } = new List<byte>();

        public int Length
            => Content.Count;

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
    }
}
