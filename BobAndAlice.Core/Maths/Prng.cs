using System;
using System.Numerics;

namespace BobAndAlice.Core.Maths
{
    public class Prng
    {
        private readonly Random _random = new Random(); 
        
        public BigInteger Next(int bytesSize)
        {
            var result = new BigInteger(NextNonZeroByte());
            
            for (int i = 1; i < bytesSize; i++)
            {
                result <<= 8;
                result |= NextByte();
            }

            return result;
        }

        public byte NextByte()
            => (byte) (_random.Next() & 0xff);

        public byte NextNonZeroByte()
        {
            byte newByte = 0;
            while (newByte == 0)
            {
                newByte = NextByte();
            }

            return newByte;
        }
    }
}