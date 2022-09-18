using System;
using System.Collections.Generic;
using BobAndAlice.Core.Crypto.Symmetric;
using BobAndAlice.Core.Maths;
using Xunit;

namespace BobAndAlice.Core.Tests.Crypto.Symmetric
{
    public class AESTests
    {
        #region Encrypt/Decrypt
        [Theory]
        [InlineData(AES.AESSupportedKeySizes.Bits128)]
        [InlineData(AES.AESSupportedKeySizes.Bits192)]
        [InlineData(AES.AESSupportedKeySizes.Bits256)]
        public void Decrypt__ShouldUndo__Encrypt(AES.AESSupportedKeySizes keySize)
        {
            var prng = new Prng();
            var key = prng.Next(AES.ToByteSize(keySize)).ToBinary();
            var aes = new AES(keySize);

            for (int i = 0; i < 10; i++)
            {
                var blocks = new List<Binary>();
                for (int j = 0; j < (prng.NextByte() >> 2) + 10; j++)
                {
                    blocks.Add(prng.Next(16).ToBinary());
                }
                var data = new Binary(blocks.ToArray());

                Assert.Equal(data.Content, aes.Decrypt(aes.Encrypt(data, key), key).Content);
            }
        }

        #endregion

        #region Encrypt/Decrypt block

        [Theory]
        [InlineData(AES.AESSupportedKeySizes.Bits128)]
        [InlineData(AES.AESSupportedKeySizes.Bits192)]
        [InlineData(AES.AESSupportedKeySizes.Bits256)]
        public void AESDecrypt__ShouldUndo__AESEncrypt(AES.AESSupportedKeySizes keySize)
        {
            var prng = new Prng();
            var key = prng.Next(AES.ToByteSize(keySize)).ToBinary();
            var aes = new AES(keySize);

            for (int i = 0; i < 10; i++)
            {
                var randomBlock = prng.Next(16).ToBinary();
                Assert.Equal(randomBlock.Content, aes.DecryptBlock(aes.EncryptBlock(randomBlock, key), key).Content);
            }
        }

        #endregion

        #region Rounds
        [Theory]
        [InlineData(AES.AESSupportedKeySizes.Bits128)]
        [InlineData(AES.AESSupportedKeySizes.Bits192)]
        [InlineData(AES.AESSupportedKeySizes.Bits256)]
        public void ApplyInvInitialRound__ShouldUndo__ApplyInitialRound(AES.AESSupportedKeySizes keySize)
        {
            var prng = new Prng();
            var key = prng.Next(AES.ToByteSize(keySize)).ToBinary();
            var aes = new AES(keySize);
            var keySchedule = aes.GenerateKeySchedule(key);

            for (int i = 0; i <= 10; i++)
            {
                var state = prng.Next(16).ToBinary();
                Assert.Equal(state.Content, aes.ApplyInvInitialRound(aes.ApplyInitialRound(state, keySchedule), keySchedule).Content);
            }
        }

        [Theory]
        [InlineData(AES.AESSupportedKeySizes.Bits128)]
        [InlineData(AES.AESSupportedKeySizes.Bits192)]
        [InlineData(AES.AESSupportedKeySizes.Bits256)]
        public void ApplyInvNormalRound__ShouldUndo__ApplyNormalRound(AES.AESSupportedKeySizes keySize)
        {
            var prng = new Prng();
            var key = prng.Next(AES.ToByteSize(keySize)).ToBinary();
            var aes = new AES(keySize);
            var keySchedule = aes.GenerateKeySchedule(key);

            for (int round = 0; round < aes.Rounds; round++)
            {
                var state = prng.Next(16).ToBinary();
                Assert.Equal(state.Content, aes.ApplyInvNormalRound(aes.ApplyNormalRound(state, round, keySchedule), round, keySchedule).Content);
            }
        }

        [Theory]
        [InlineData(AES.AESSupportedKeySizes.Bits128)]
        [InlineData(AES.AESSupportedKeySizes.Bits192)]
        [InlineData(AES.AESSupportedKeySizes.Bits256)]
        public void ApplyInvFinalRound__ShouldUndo__ApplyFinalRound(AES.AESSupportedKeySizes keySize)
        {
            var prng = new Prng();
            var key = prng.Next(AES.ToByteSize(keySize)).ToBinary();
            var aes = new AES(keySize);
            var keySchedule = aes.GenerateKeySchedule(key);

            for (int i = 0; i <= 10; i++)
            {
                var state = prng.Next(16).ToBinary();
                Assert.Equal(state.Content, aes.ApplyInvFinalRound(aes.ApplyFinalRound(state, keySchedule), keySchedule).Content);
            }
        }
        #endregion

        #region SubBytes
        [Theory]
        [InlineData(AES.AESSupportedKeySizes.Bits128)]
        [InlineData(AES.AESSupportedKeySizes.Bits192)]
        [InlineData(AES.AESSupportedKeySizes.Bits256)]
        public void SubByte__ShouldReturnByte__BasedOnSBoxTable(AES.AESSupportedKeySizes keySize)
        {
            var prng = new Prng();
            var aes = new AES(keySize);

            var testPairs = new List<(byte Input, byte ExpectedOutput)>()
            { (0x9a, 0xb8), (0x0f, 0x76),  (0xf8, 0x41), (0xbe, 0xae), };

            foreach (var pair in testPairs)
            {
                Assert.Equal(pair.ExpectedOutput, aes.SubByte(pair.Input));
            }
        }

        [Theory]
        [InlineData(AES.AESSupportedKeySizes.Bits128)]
        [InlineData(AES.AESSupportedKeySizes.Bits192)]
        [InlineData(AES.AESSupportedKeySizes.Bits256)]
        public void InvSubByte__ShouldReturnByte__BasedOnInvSBoxTable(AES.AESSupportedKeySizes keySize)
        {
            var aes = new AES(keySize);

            var testPairs = new List<(byte ExpectedOutput, byte Input)>()
                {(0x9a, 0xb8), (0x0f, 0x76), (0xf8, 0x41), (0xbe, 0xae),};

            foreach (var pair in testPairs)
            {
                Assert.Equal(pair.ExpectedOutput, aes.InvSubByte(pair.Input));
            }
        }
        #endregion

        #region ShiftRows
        [Theory]
        [InlineData(AES.AESSupportedKeySizes.Bits128)]
        [InlineData(AES.AESSupportedKeySizes.Bits192)]
        [InlineData(AES.AESSupportedKeySizes.Bits256)]
        public void ShiftRow__ShouldReturnWord__ProperlyShifted(AES.AESSupportedKeySizes keySize)
        {
            var aes = new AES(keySize);

            Assert.Equal((UInt32) 0x01020304, aes.ShiftRow(0x01020304, 0));
            Assert.Equal((UInt32) 0x02030401, aes.ShiftRow(0x01020304, 1));
            Assert.Equal((UInt32) 0x03040102, aes.ShiftRow(0x01020304, 2));
            Assert.Equal((UInt32) 0x04010203, aes.ShiftRow(0x01020304, 3)); 
            Assert.Equal((UInt32) 0x2F9392C0, aes.ShiftRow(0xC02F9392, 1));
            Assert.Equal((UInt32) 0xAFC7AB30, aes.ShiftRow(0xAB30AFC7, 2));
            Assert.Equal((UInt32) 0xA220CB2B, aes.ShiftRow(0x20CB2BA2, 3));
        }

        [Theory]
        [InlineData(AES.AESSupportedKeySizes.Bits128)]
        [InlineData(AES.AESSupportedKeySizes.Bits192)]
        [InlineData(AES.AESSupportedKeySizes.Bits256)]
        public void InvShiftRow__ShouldUndo__ShiftRow(AES.AESSupportedKeySizes keySize)
        {
            var aes = new AES(keySize);

            Assert.Equal((UInt32) 0x01020304, aes.InvShiftRow(aes.ShiftRow(0x01020304, 0), 0));
            Assert.Equal((UInt32) 0x01020304, aes.InvShiftRow(aes.ShiftRow(0x01020304, 1), 1));
            Assert.Equal((UInt32) 0x01020304, aes.InvShiftRow(aes.ShiftRow(0x01020304, 2), 2));
            Assert.Equal((UInt32) 0x01020304, aes.InvShiftRow(aes.ShiftRow(0x01020304, 3), 3)); 
            Assert.Equal((UInt32) 0xC02F9392, aes.InvShiftRow(aes.ShiftRow(0xC02F9392, 1), 1));
            Assert.Equal((UInt32) 0xAB30AFC7, aes.InvShiftRow(aes.ShiftRow(0xAB30AFC7, 2), 2));
            Assert.Equal((UInt32) 0x20CB2BA2, aes.InvShiftRow(aes.ShiftRow(0x20CB2BA2, 3), 3));
        }
        #endregion

        #region MixColumns
        [Theory]
        [InlineData(AES.AESSupportedKeySizes.Bits128)]
        [InlineData(AES.AESSupportedKeySizes.Bits192)]
        [InlineData(AES.AESSupportedKeySizes.Bits256)]
        public void InvMixColumns__ShouldUndo__MixColumns(AES.AESSupportedKeySizes keySize)
        {
            var prng = new Prng();
            var aes = new AES(keySize);

            for (int i = 0; i <= 10; i++)
            {
                var state = prng.Next(16).ToBinary();
                Assert.Equal(state.Content, aes.InvMixColumns(aes.MixColumns(state)).Content);
            }
        }
        #endregion
    }
}