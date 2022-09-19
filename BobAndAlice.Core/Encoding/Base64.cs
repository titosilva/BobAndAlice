using BobAndAlice.Core.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BobAndAlice.Core.Encoding
{
    public static class Base64
    {
        public static BigInteger ToBigInteger(string b64)
            => new BigInteger(ToByteArray(b64));

        public static byte[] ToByteArray(string b64) 
            => Convert.FromBase64String(b64);

        public static Binary ToBinary(string b64)
            => new Binary(ToByteArray(b64));

        public static string FromBigInteger(BigInteger value)
            => Convert.ToBase64String(value.ToByteArray());

        public static string FromByteArray(byte[] value) 
            => Convert.ToBase64String(value);

        public static string FromBinary(Binary value)
            => FromByteArray(value.ToByteArray());

    }
}
