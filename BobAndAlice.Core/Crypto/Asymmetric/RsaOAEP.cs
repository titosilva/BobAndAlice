using BobAndAlice.Core.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BobAndAlice.Core.Crypto.Asymmetric
{
    public class RsaOAEP
    {
        public RsaKey PrivateKey { get; set; }
        public RsaKey PublicKey { get; set; }

        public BigInteger Trapdoor(BigInteger message, RsaKey key)
            => BigInteger.ModPow(message, key.Value, key.Modulus);
    }
}
