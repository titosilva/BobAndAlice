using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BobAndAlice.Core.Maths;

namespace BobAndAlice.Core.Crypto.Symmetric
{
    public class AES
    {
        #region Block sizes supported
        public enum AESSupportedBlockSizes
        {
            Bits128,
            Bits192,
            Bits256,
        }

        public int ToBitSize(AESSupportedBlockSizes blockSize)
            => blockSize switch
            {
                AESSupportedBlockSizes.Bits128 => 128,
                AESSupportedBlockSizes.Bits192 => 192,
                AESSupportedBlockSizes.Bits256 => 256,
                _ => throw new ArgumentException("Not supported AES block size"),
            };

        public int ToByteSize(AESSupportedBlockSizes blockSize)
            => ToBitSize(blockSize) >> 3;
        
        public int ToRounds(AESSupportedBlockSizes blockSize)
            => blockSize switch
            { // Number of rounds needed + initial round
                AESSupportedBlockSizes.Bits128 => 11, 
                AESSupportedBlockSizes.Bits192 => 13,
                AESSupportedBlockSizes.Bits256 => 15,
                _ => throw new ArgumentException("Not supported AES block size"),
            };
        #endregion

        private AESSupportedBlockSizes blockSize { get; set; }

        private int blockBytesSize
            => ToByteSize(blockSize);

        private int rounds
            => ToRounds(blockSize);

        private int keyWordsLength // Block bytes / 4 = block words = key length
            => blockBytesSize >> 2;

        private List<UInt32> keyScheduleWords { get; set; } = new List<UInt32>();
        private Binary key { get; set; }
        
        public Binary EncryptBlock(Binary value) {
            if (value.Length != blockBytesSize) {
                throw new ArgumentException($"AES block size on selected mode must {blockBytesSize} bytes ({blockBytesSize << 3} bits)");
            }

            genKeySchedule();

            var state = value;
            state = applyInitialRound(state);
            for (int i = 0; i < rounds - 2; i++) {
                state = applyNormalRound(state, i + 1);
            }

            return applyFinalRound(state);
        }
        
        private Binary applyInitialRound(Binary state) {
            return addRoundKey(state, 0);
        }
        
        private Binary applyNormalRound(Binary state, int round) {
            var result = state;

            result = subBytes(result);
            result = shiftRows(result);
            result = mixColumns(result);
            result = addRoundKey(result, round);

            return result;
        }

        private Binary applyFinalRound(Binary state) {
            var result = state;

            result = subBytes(result);
            result = shiftRows(result);
            result = addRoundKey(result, rounds - 1);

            return result;
        }
        
        private Binary subBytes(Binary state) {
            var bytes = state.Content;
            var resultBytes = new List<byte>();

            foreach (var b in bytes)
            {
                var lowerNibble = (b & 0x0f);
                var upperNibble = (b & 0xf0) >> 4;
                resultBytes.Add(Constants.AES.SBoxTable[upperNibble, lowerNibble]);
            }

            return new Binary(resultBytes);
        }
        
        private Binary shiftRows(Binary state) {
            var words = state.ToWords();
            var resultWords = new List<UInt32>();
    
            for (int i = 3; i >= 0; i--) {
                var word = words[i];
        
                if (i == 0) {
                    // The first row is not shifted
                    resultWords.Add(word);
                } else if (i == 1) {
                    // The second row is shifted one place to the left
                    resultWords.Add((word << 8) | (word >> 24));
                } else if (i == 2) {
                    // The third row is shifted two places to the left
                    resultWords.Add((word << 16) | (word >> 16));
                } else if (i == 3) {
                    // The last row is shifted three places to the left
                    resultWords.Add((word << 24) | (word >> 8));
                }
            }

            return new Binary(resultWords);
        }
        
        
    // Source of reference for this implementation:
    // https://en.wikipedia.org/wiki/Rijndael_MixColumns
        Binary mixColumns(Binary state) {
            var stateBytes = state.Content;
            var resultColumns = new List<List<byte>>();
            var resultBytes = new List<byte>();

            for (int col = 0; col < 4; col++) {
                // State column
                var r = new List<byte>();
                // Computed columns
                var a = new List<byte>();
                var b = new List<byte>();

                for (int row = 3; row >= 0; row--) {
                    r.Add(stateBytes[col + 4 * row]);
                }

                for (int c = 0; c < 4; c++) {
                    var h = (r[c] >> 7) & 1;
                    b[c] = (byte) (r[c] << 1);
                    b[c] ^= (byte) (h * 0x1B);
                }

                r[0] = (byte) (b[0] ^ a[3] ^ a[2] ^ b[1] ^ a[1]); /* 2 * a0 + a3 + a2 + 3 * a1 */
                r[1] = (byte) (b[1] ^ a[0] ^ a[3] ^ b[2] ^ a[2]); /* 2 * a1 + a0 + a3 + 3 * a2 */
                r[2] = (byte) (b[2] ^ a[1] ^ a[0] ^ b[3] ^ a[3]); /* 2 * a2 + a1 + a0 + 3 * a3 */
                r[3] = (byte) (b[3] ^ a[2] ^ a[1] ^ b[0] ^ a[0]); /* 2 * a3 + a2 + a1 + 3 * a0 */

                resultColumns.Insert(0, r);
            }

            for (int row = 0; row < 4; row++) {
                for (int col = 0; col < 4; col++) {
                    resultBytes.Insert(0, resultColumns[col][row]);
                }
            }

            return new Binary(resultBytes);
        }

        private Binary addRoundKey(Binary state, int round)
        {
            var roundKey = new Binary(keyScheduleWords[round]);
            return state ^ roundKey;
        }
        
        // Source of reference for this implementation:
        // https://en.wikipedia.org/wiki/AES_key_schedule
        void genKeySchedule() {
            this.keyScheduleWords.Clear();

            // Each round needs a derived key composed of 4 32-bit word
            // So, for instance, we need a key schedule of 11 * 4 = 44 32-bits words for derived keys on AES-128
            for (int i = 0; i < keyWordsLength * rounds; i++) {
                UInt32 nextWord = 0;

                // For the first 4 words, we just copy from the key
                if (i < keyWordsLength) {
                    nextWord = key.ToWords()[i];
                } else {
                    // For the other words, we must do some computation
                    UInt32 previousPeriodWord = keyScheduleWords[i - keyWordsLength];
                    UInt32 previousWord = keyScheduleWords[i - 1];
                    nextWord = previousPeriodWord;

                    if (i % keyWordsLength == 0) {
                        nextWord ^= subWord(rotWord(previousWord));
                        nextWord ^= Constants.AES.RoundConstants[i / keyWordsLength];
                    } else if (keyWordsLength > 6 && i % keyWordsLength == 4) {
                        nextWord ^= subWord(previousWord);
                    } else {
                        nextWord ^= previousWord;
                    }
                }

                keyScheduleWords.Add(nextWord);
            }
        }
        
        private UInt32 rotWord(UInt32 word) {
            return (word << 8) | (word >> 24);
        }
        
        private UInt32 subWord(UInt32 word) {
            return (uint) (
                (subByte((byte) ((word & 0xff000000) >> 24)) << 24) |
                (subByte((byte) ((word & 0x00ff0000) >> 16)) << 16) |
                (subByte((byte) ((word & 0x0000ff00) >> 8)) << 8) |
                (subByte((byte) (word & 0x000000ff))));
        }
        
        byte subByte(byte b) {
            var lowerNibble = (b & 0x0f);
            var upperNibble = (b & 0xf0) >> 4;
            return Constants.AES.SBoxTable[upperNibble, lowerNibble];
        }
    }
}
