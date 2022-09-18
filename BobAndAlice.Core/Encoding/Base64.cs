﻿using System;
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
    }
}