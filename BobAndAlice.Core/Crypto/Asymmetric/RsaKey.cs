using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BobAndAlice.Core.Crypto.Asymmetric
{
    public class RsaKey
    {
        public RsaKey(BigInteger modulus, BigInteger value)
        {
            Modulus = modulus;
            Value = value;
        }

        public BigInteger Modulus { get; set; }
        public BigInteger Value { get; set; }
    }
}
