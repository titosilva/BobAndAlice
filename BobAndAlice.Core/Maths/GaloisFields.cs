namespace BobAndAlice.Core.Maths
{
    public class GaloisFields
    {
        // Source: https://en.wikipedia.org/wiki/Rijndael_MixColumns
        public static byte Multiply256(byte a, byte b)
        {
            if (b == 0x01)
            {
                return a;
            } else if (a == 0x01)
            {
                return b;
            }
            
            byte p = 0;

            for (int counter = 0; counter < 8; counter++) {
                if ((b & 1) != 0) {
                    p ^= a;
                }
                
                a <<= 1;
                if ((a & 0x80) != 0) {
                    a ^= 0x1B; /* x^8 + x^4 + x^3 + x + 1 */
                }
                b >>= 1;
            }

            return p;
        }
    }
}