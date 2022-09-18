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

        public int ToBitSize(AESSupportedKeySizes keySize)
            => keySize switch
            {
                AESSupportedKeySizes.Bits128 => 128,
                AESSupportedKeySizes.Bits192 => 192,
                AESSupportedKeySizes.Bits256 => 256,
                _ => throw new ArgumentException("Not supported AES block size"),
            };

        public int ToByteSize(AESSupportedKeySizes keySize)
            => ToBitSize(keySize) >> 3;
        
        public int ToRounds(AESSupportedKeySizes keySize)
            => keySize switch
            { // Number of rounds needed + initial round
                AESSupportedKeySizes.Bits128 => 11, 
                AESSupportedKeySizes.Bits192 => 13,
                AESSupportedKeySizes.Bits256 => 15,
                _ => throw new ArgumentException("Not supported AES block size"),
            };
        #endregion

        public AES(AESSupportedKeySizes keySize, Binary key)
        {
            if (key.Length != ToByteSize(keySize))
            {
                throw new ArgumentException($"Size of block selected must be same size of key");
            }

            this.key = key;
            this.keySize = keySize;
            genKeySchedule();
        }

        private AESSupportedKeySizes keySize { get; set; }

        private int keyBytesSize
            => ToByteSize(keySize);

        public int Rounds
            => ToRounds(keySize);

        private int keyWordsLength // Block bytes / 4 = block words = key length
            => keyBytesSize >> 2;

        private List<UInt32> keyScheduleWords { get; set; } = new List<UInt32>();
        private Binary key { get; set; }

        #region Encrypt/Decrypt block
        public Binary EncryptBlock(Binary value) {
            if (value.Length != 16) {
                throw new ArgumentException($"AES block size on selected mode must 16 bytes (128 bits)");
            }

            var state = value;
            state = ApplyInitialRound(state);
            for (int i = 0; i < Rounds - 2; i++) {
                state = ApplyNormalRound(state, i + 1);
            }
            state = ApplyFinalRound(state);

            return state;
        }

        public Binary DecryptBlock(Binary value)
        {
            if (value.Length != 16) {
                throw new ArgumentException($"AES block size on selected mode must 16 bytes (128 bits)");
            }

            var state = value;
            state = ApplyInvFinalRound(state);
            for (int i = Rounds - 3; i >= 0; i--) {
                state = ApplyInvNormalRound(state, i + 1);
            }
            state = ApplyInvInitialRound(state);

            return state; 
        }
        #endregion

        #region Rounds
        public Binary ApplyInitialRound(Binary state) {
            return AddRoundKey(state, 0);
        }

        public Binary ApplyInvInitialRound(Binary state)
        {
            return AddRoundKey(state, 0);
        }
        
        public Binary ApplyNormalRound(Binary state, int round) {
            var result = state;

            result = SubBytes(result);
            result = ShiftRows(result);
            result = MixColumns(result);
            result = AddRoundKey(result, round);

            return result;
        }

        public Binary ApplyInvNormalRound(Binary state, int round)
        {
            var result = state;

            result = AddRoundKey(result, round);
            result = InvMixColumns(result);
            result = InvShiftRows(result);
            result = InvSubBytes(result);

            return result;   
        }

        public Binary ApplyFinalRound(Binary state) {
            var result = state;

            result = SubBytes(result);
            result = ShiftRows(result);
            result = AddRoundKey(result, Rounds - 1);

            return result;
        }

        public Binary ApplyInvFinalRound(Binary state)
        {
            var result = state;

            result = AddRoundKey(result, Rounds - 1);
            result = InvShiftRows(result);
            result = InvSubBytes(result);
            
            return result;
        }
        #endregion
        
        #region SubBytes
        public byte SubByte(byte b) {
            var lowerNibble = (b & 0x0f);
            var upperNibble = (b & 0xf0) >> 4;
            return Constants.AES.SBoxTable[upperNibble, lowerNibble];
        }

        public byte InvSubByte(byte b)
        {
            var lowerNibble = (b & 0x0f);
            var upperNibble = (b & 0xf0) >> 4;
            return Constants.AES.InvSBoxTable[upperNibble, lowerNibble];
        }
        
        public Binary SubBytes(Binary state)
        {
            return new Binary(state.Content.Select(SubByte).ToList());
        }

        public Binary InvSubBytes(Binary state)
        {
            return new Binary(state.Content.Select(InvSubByte).ToList());
        }
        #endregion

        #region ShiftRows
        public UInt32 ShiftRow(UInt32 row, int shift)
        {
            return (row << (shift * 8)) | (row >> ((4 - shift) * 8));
        }
        
        public Binary ShiftRows(Binary state)
        {
            return new Binary(state.ToWords().Select(ShiftRow).ToList());
        }
        
        public UInt32 InvShiftRow(UInt32 row, int shift)
        {
            return (row >> (shift * 8)) | (row << ((4 - shift) * 8));
        }

        public Binary InvShiftRows(Binary state)
        {
            return new Binary(state.ToWords().Select(InvShiftRow).ToList());
        }
        #endregion

        #region MixColumns
        public byte MultiplyColumnGF(List<byte> column, byte[] coefficients)
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
        public Binary MixColumns(Binary state)
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
                
                resultBytes[0 + colIdx] = MultiplyColumnGF(col, new byte[] {0x02, 0x03, 0x01, 0x01});
                resultBytes[4 + colIdx] = MultiplyColumnGF(col, new byte[] {0x01, 0x02, 0x03, 0x01});
                resultBytes[8 + colIdx] = MultiplyColumnGF(col, new byte[] {0x01, 0x01, 0x02, 0x03});
                resultBytes[12 + colIdx] = MultiplyColumnGF(col, new byte[] {0x03, 0x01, 0x01, 0x02});
            }

            return new Binary(resultBytes);
        }
        
        public Binary InvMixColumns(Binary state)
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
                
                resultBytes[0 + colIdx] = MultiplyColumnGF(col, new byte[] {0x0e, 0x0b, 0x0d, 0x09});
                resultBytes[4 + colIdx] = MultiplyColumnGF(col, new byte[] {0x09, 0x0e, 0x0b, 0x0d});
                resultBytes[8 + colIdx] = MultiplyColumnGF(col, new byte[] {0x0d, 0x09, 0x0e, 0x0b});
                resultBytes[12 + colIdx] = MultiplyColumnGF(col, new byte[] {0x0b, 0x0d, 0x09, 0x0e});
            }

            return new Binary(resultBytes);
        }
        #endregion

        #region AddRoundKey
        public Binary AddRoundKey(Binary state, int round)
        {
            var roundKey = new Binary(new List<uint>()
            {
                keyScheduleWords[round * 4],
                keyScheduleWords[round * 4 + 1],
                keyScheduleWords[round * 4 + 2],
                keyScheduleWords[round * 4 + 3]
            });
            return state ^ roundKey;
        }
        #endregion
        
        #region Key schedule generation
        // Source of reference for this implementation:
        // https://en.wikipedia.org/wiki/AES_key_schedule
        void genKeySchedule() {
            keyScheduleWords.Clear();

            // Each round needs a derived key composed of 4 32-bit word
            // So, for instance, we need a key schedule of 11 * 4 = 44 32-bits words for derived keys on AES-128
            for (int i = 0; i < keyWordsLength * Rounds; i++) {
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
                        nextWord ^= Constants.AES.RoundConstants[i / keyWordsLength - 1];
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
                (SubByte((byte) ((word & 0xff000000) >> 24)) << 24) |
                (SubByte((byte) ((word & 0x00ff0000) >> 16)) << 16) |
                (SubByte((byte) ((word & 0x0000ff00) >> 8)) << 8) |
                (SubByte((byte) (word & 0x000000ff))));
        }
        
        #endregion
    }
}
