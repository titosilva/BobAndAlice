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
        #region Key sizes supported
        public enum AESSupportedKeySizes
        {
            Bits128,
            Bits192,
            Bits256,
        }

        public static int ToBitSize(AESSupportedKeySizes keySize)
            => keySize switch
            {
                AESSupportedKeySizes.Bits128 => 128,
                AESSupportedKeySizes.Bits192 => 192,
                AESSupportedKeySizes.Bits256 => 256,
                _ => throw new ArgumentException("Not supported AES block size"),
            };

        public static int ToByteSize(AESSupportedKeySizes keySize)
            => ToBitSize(keySize) >> 3;

        public static int ToRounds(AESSupportedKeySizes keySize)
            => keySize switch
            { // Number of rounds needed + initial round
                AESSupportedKeySizes.Bits128 => 11,
                AESSupportedKeySizes.Bits192 => 13,
                AESSupportedKeySizes.Bits256 => 15,
                _ => throw new ArgumentException("Not supported AES block size"),
            };
        #endregion

        #region Properties
        public AESSupportedKeySizes KeySize { get; set; }

        public int Rounds
            => ToRounds(KeySize);

        public int KeyBytesSize
            => ToByteSize(KeySize);

        public int KeyWordsSize
            => KeyBytesSize >> 2;
        #endregion

        public AES(AESSupportedKeySizes keySize)
        {
            KeySize = keySize;
        }

        public Binary Encrypt(Binary bin, Binary key)
        {
            var toEncryptBlocks = bin.Split(16);
            if (toEncryptBlocks.Any(block => block.Length != 16))
            {
                // The user must pad their data themselves, so we don't affect theirs encodings
                throw new ArgumentException("Please, provide an input with a bytes length divisible by 16");
            }
            return new Binary(toEncryptBlocks.Select(block => EncryptBlock(new Binary(block), key)).ToArray());
        }

        public Binary Decrypt(Binary bin, Binary key)
        {
            var toDecryptBlocks = bin.Split(16);

            if (toDecryptBlocks.Any(block => block.Length != 16))
            {
                // The user must pad their data themselves, so we don't affect theirs encodings
                throw new ArgumentException("Please, provide an input with a bytes length divisible by 16");
            }

            return new Binary(toDecryptBlocks.Select(block => DecryptBlock(new Binary(block), key)).ToArray());
        }

        #region Encrypt/Decrypt block
        public Binary EncryptBlock(Binary value, Binary key)
        {
            if (key.Length != ToByteSize(KeySize))
            {
                throw new ArgumentException($"Size of block selected must be same size of key");
            }

            if (value.Length != 16)
            {
                throw new ArgumentException($"AES block size on selected mode must 16 bytes (128 bits)");
            }

            var keySchedule = GenerateKeySchedule(key);

            var state = value;
            state = ApplyInitialRound(state, keySchedule);
            for (int i = 0; i < Rounds - 2; i++)
            {
                state = ApplyNormalRound(state, i + 1, keySchedule);
            }
            state = ApplyFinalRound(state, keySchedule);

            return state;
        }

        public Binary DecryptBlock(Binary value, Binary key)
        {
            if (key.Length != ToByteSize(KeySize))
            {
                throw new ArgumentException($"Size of provided key must be {ToByteSize(KeySize)}");
            }

            if (value.Length != 16)
            {
                throw new ArgumentException($"AES block size on selected mode must 16 bytes (128 bits)");
            }

            var keySchedule = GenerateKeySchedule(key);

            var state = value;
            state = ApplyInvFinalRound(state, keySchedule);
            for (int i = Rounds - 3; i >= 0; i--)
            {
                state = ApplyInvNormalRound(state, i + 1, keySchedule);
            }
            state = ApplyInvInitialRound(state, keySchedule);

            return state;
        }
        #endregion

        #region Rounds
        public static Binary ApplyInitialRound(Binary state, List<uint> keySchedule)
        {
            return AddRoundKey(state, 0, keySchedule);
        }

        public static Binary ApplyInvInitialRound(Binary state, List<uint> keySchedule)
        {
            return AddRoundKey(state, 0, keySchedule);
        }

        public static Binary ApplyNormalRound(Binary state, int round, List<uint> keySchedule)
        {
            var result = state;

            result = SubBytes(result);
            result = ShiftRows(result);
            result = MixColumns(result);
            result = AddRoundKey(result, round, keySchedule);

            return result;
        }

        public static Binary ApplyInvNormalRound(Binary state, int round, List<uint> keySchedule)
        {
            var result = state;

            result = AddRoundKey(result, round, keySchedule);
            result = InvMixColumns(result);
            result = InvShiftRows(result);
            result = InvSubBytes(result);

            return result;
        }

        public Binary ApplyFinalRound(Binary state, List<uint> keySchedule)
        {
            var result = state;

            result = SubBytes(result);
            result = ShiftRows(result);
            result = AddRoundKey(result, Rounds - 1, keySchedule);

            return result;
        }

        public Binary ApplyInvFinalRound(Binary state, List<uint> keySchedule)
        {
            var result = state;

            result = AddRoundKey(result, Rounds - 1, keySchedule);
            result = InvShiftRows(result);
            result = InvSubBytes(result);

            return result;
        }
        #endregion

        #region SubBytes
        public static byte SubByte(byte b)
        {
            var lowerNibble = (b & 0x0f);
            var upperNibble = (b & 0xf0) >> 4;
            return Constants.AES.SBoxTable[upperNibble, lowerNibble];
        }

        public static byte InvSubByte(byte b)
        {
            var lowerNibble = (b & 0x0f);
            var upperNibble = (b & 0xf0) >> 4;
            return Constants.AES.InvSBoxTable[upperNibble, lowerNibble];
        }

        public static Binary SubBytes(Binary state)
        {
            return new Binary(state.Content.Select(SubByte).ToList());
        }

        public static Binary InvSubBytes(Binary state)
        {
            return new Binary(state.Content.Select(InvSubByte).ToList());
        }
        #endregion

        #region ShiftRows
        public static UInt32 ShiftRow(UInt32 row, int shift)
        {
            return (row << (shift * 8)) | (row >> ((4 - shift) * 8));
        }

        public static Binary ShiftRows(Binary state)
        {
            return new Binary(state.ToWords().Select(ShiftRow).ToList());
        }

        public static UInt32 InvShiftRow(UInt32 row, int shift)
        {
            return (row >> (shift * 8)) | (row << ((4 - shift) * 8));
        }

        public static Binary InvShiftRows(Binary state)
        {
            return new Binary(state.ToWords().Select(InvShiftRow).ToList());
        }
        #endregion

        #region MixColumns
        public static byte MultiplyColumnGF(List<byte> column, byte[] coefficients)
        {
            byte result = 0;

            for (int i = 0; i < coefficients.Length; i++)
            {
                result ^= GaloisFields.Multiply256(column[i], coefficients[i]);
            }

            return result;
        }

        // Source of reference for this implementation:
        // https://en.wikipedia.org/wiki/Rijndael_MixColumns
        public static Binary MixColumns(Binary state)
        {
            var stateBytes = new List<byte>(state.Content);
            var resultBytes = new List<byte>()
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
            };

            for (int colIdx = 0; colIdx < 4; colIdx++)
            {
                var col = stateBytes.Where((_, idx) => (idx % 4) == colIdx).ToList();

                resultBytes[0 + colIdx] = MultiplyColumnGF(col, new byte[] { 0x02, 0x03, 0x01, 0x01 });
                resultBytes[4 + colIdx] = MultiplyColumnGF(col, new byte[] { 0x01, 0x02, 0x03, 0x01 });
                resultBytes[8 + colIdx] = MultiplyColumnGF(col, new byte[] { 0x01, 0x01, 0x02, 0x03 });
                resultBytes[12 + colIdx] = MultiplyColumnGF(col, new byte[] { 0x03, 0x01, 0x01, 0x02 });
            }

            return new Binary(resultBytes);
        }

        public static Binary InvMixColumns(Binary state)
        {
            var stateBytes = new List<byte>(state.Content);
            var resultBytes = new List<byte>()
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
            };

            for (int colIdx = 0; colIdx < 4; colIdx++)
            {
                var col = stateBytes.Where((_, idx) => (idx % 4) == colIdx).ToList();

                resultBytes[0 + colIdx] = MultiplyColumnGF(col, new byte[] { 0x0e, 0x0b, 0x0d, 0x09 });
                resultBytes[4 + colIdx] = MultiplyColumnGF(col, new byte[] { 0x09, 0x0e, 0x0b, 0x0d });
                resultBytes[8 + colIdx] = MultiplyColumnGF(col, new byte[] { 0x0d, 0x09, 0x0e, 0x0b });
                resultBytes[12 + colIdx] = MultiplyColumnGF(col, new byte[] { 0x0b, 0x0d, 0x09, 0x0e });
            }

            return new Binary(resultBytes);
        }
        #endregion

        #region AddRoundKey
        public static Binary AddRoundKey(Binary state, int round, List<uint> keySchedule)
        {
            var roundKey = new Binary(new List<uint>()
            {
                keySchedule[round * 4],
                keySchedule[round * 4 + 1],
                keySchedule[round * 4 + 2],
                keySchedule[round * 4 + 3]
            });
            return state ^ roundKey;
        }
        #endregion

        #region Key schedule generation
        // Source of reference for this implementation:
        // https://en.wikipedia.org/wiki/AES_key_schedule
        public List<UInt32> GenerateKeySchedule(Binary key)
        {
            var keyScheduleWords = new List<UInt32>();

            // Each round needs a derived key composed of 4 32-bit word
            // So, for instance, we need a key schedule of 11 * 4 = 44 32-bits words for derived keys on AES-128
            for (int i = 0; i < 4 * Rounds; i++)
            {
                uint nextWord;

                // For the first 4 words, we just copy from the key
                if (i < KeyWordsSize)
                {
                    nextWord = key.ToWords()[i];
                }
                else
                {
                    // For the other words, we must do some computation
                    UInt32 previousPeriodWord = keyScheduleWords[i - KeyWordsSize];
                    UInt32 previousWord = keyScheduleWords[i - 1];
                    nextWord = previousPeriodWord;

                    if (i % KeyWordsSize == 0)
                    {
                        nextWord ^= SubWord(RotWord(previousWord));
                        nextWord ^= Constants.AES.RoundConstants[i / KeyWordsSize - 1];
                    }
                    else if (KeyWordsSize > 6 && i % KeyWordsSize == 4)
                    {
                        nextWord ^= SubWord(previousWord);
                    }
                    else
                    {
                        nextWord ^= previousWord;
                    }
                }

                keyScheduleWords.Add(nextWord);
            }

            return keyScheduleWords;
        }

        private static UInt32 RotWord(UInt32 word)
        {
            return (word << 8) | (word >> 24);
        }

        private static UInt32 SubWord(UInt32 word)
        {
            return (uint)(
                (SubByte((byte)((word & 0xff000000) >> 24)) << 24) |
                (SubByte((byte)((word & 0x00ff0000) >> 16)) << 16) |
                (SubByte((byte)((word & 0x0000ff00) >> 8)) << 8) |
                (SubByte((byte)(word & 0x000000ff))));
        }

        #endregion
    }
}
