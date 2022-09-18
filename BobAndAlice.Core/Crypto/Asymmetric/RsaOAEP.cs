using BobAndAlice.Core.Crypto.Hash;
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
        private readonly Prng prng;
        private readonly SHA3 G;
        private readonly SHA3 H;

        public SHA3.SHA3SupportedBitSizes RandomValueSize { get; private set; }
        public SHA3.SHA3SupportedBitSizes PaddedMessageSize { get; private set; }
        public int PaddingBytesSize { get; private set; }

        public int MessageBytesSize
            => SHA3.ToBytesSize(PaddedMessageSize) - PaddingBytesSize;

        private int PaddedMessageSizeBytes
            => SHA3.ToBytesSize(PaddedMessageSize);

        private int RandomValueSizeBytes
            => SHA3.ToBytesSize(RandomValueSize);

        public RsaOAEP(SHA3.SHA3SupportedBitSizes paddedMessageSize = SHA3.SHA3SupportedBitSizes.Bits512, int paddingBytesSize = 4, SHA3.SHA3SupportedBitSizes randomValueSize = SHA3.SHA3SupportedBitSizes.Bits512)
        {
            prng = new Prng();
            G = new SHA3(paddedMessageSize);
            H = new SHA3(randomValueSize);

            PaddedMessageSize = paddedMessageSize;
            PaddingBytesSize = paddingBytesSize;
            RandomValueSize = randomValueSize;
        }


        public Binary Encrypt(Binary message, RsaKey key)
        {
            var messageToTrapdoor = prepareMessage(message);
            return Trapdoor(messageToTrapdoor.ToBigInteger(), key).ToBinary();
        }

        public Binary Decrypt(Binary message, RsaKey key)
        {
            var untrapdooredMessage = Trapdoor(message.ToBigInteger(), key).ToBinary();
            return recoverMessage(untrapdooredMessage);
        }

        public BigInteger Trapdoor(BigInteger message, RsaKey key)
            => BigInteger.ModPow(message, key.Value, key.Modulus);

        private Binary prepareMessage(Binary message)
        {
            if (message.Length + PaddingBytesSize != PaddedMessageSizeBytes)
            {
                throw new ArgumentException($"Message must be {PaddedMessageSizeBytes - PaddingBytesSize} bytes long to keep up with specified parameters");
            }

            // Pad the message
            var paddedMessage = new Binary(message);
            paddedMessage.PadRight(PaddingBytesSize);

            // Generate the random value "r"
            var r = prng.Next(RandomValueSizeBytes).ToBinary();

            // Feistel rounds
            var rHash = G.Hash(r);
            var s = paddedMessage ^ rHash;
            var t = r ^ H.Hash(s);

            return new Binary(s, t);
        }

        private Binary recoverMessage(Binary untrapdooredMessage)
        {
            // Split the message into s and t
            var s = new Binary(untrapdooredMessage.Content.Take(PaddedMessageSizeBytes).ToList());
            var t = new Binary(untrapdooredMessage.Content.Skip(PaddedMessageSizeBytes).ToList());

            // Undo Feistel rounds
            var r = H.Hash(s) ^ t;
            var paddedMessage = G.Hash(r) ^ s;

            // Verify padding bytes
            if (paddedMessage.Content.Take(PaddingBytesSize).Any(b => b != 0))
            {
                throw new Exception("The padding has bytes that are not 0");
            }

            // Return the original message without the padding bytes
            return new Binary(paddedMessage.Content.Skip(PaddingBytesSize).ToList());
        }
    }
}
