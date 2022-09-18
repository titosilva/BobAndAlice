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
                Assert.Equal(state.Content, AES.ApplyInvInitialRound(AES.ApplyInitialRound(state, keySchedule), keySchedule).Content);
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
                Assert.Equal(state.Content, AES.ApplyInvNormalRound(AES.ApplyNormalRound(state, round, keySchedule), round, keySchedule).Content);
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
        [Fact]
        public void SubByte__ShouldReturnByte__BasedOnSBoxTable()
        {
            var testPairs = new List<(byte Input, byte ExpectedOutput)>()
            { (0x9a, 0xb8), (0x0f, 0x76),  (0xf8, 0x41), (0xbe, 0xae), };

            foreach (var (input, expectedOutput) in testPairs)
            {
                Assert.Equal(expectedOutput, AES.SubByte(input));
            }
        }

        [Fact]
        public void InvSubByte__ShouldReturnByte__BasedOnInvSBoxTable()
        {
            var testPairs = new List<(byte ExpectedOutput, byte Input)>()
                {(0x9a, 0xb8), (0x0f, 0x76), (0xf8, 0x41), (0xbe, 0xae),};

            foreach (var (expectedOutput, input) in testPairs)
            {
                Assert.Equal(expectedOutput, AES.InvSubByte(input));
            }
        }
        #endregion

        #region ShiftRows
        [Fact]
        public void ShiftRow__ShouldReturnWord__ProperlyShifted()
        {
            Assert.Equal((UInt32)0x01020304, AES.ShiftRow(0x01020304, 0));
            Assert.Equal((UInt32)0x02030401, AES.ShiftRow(0x01020304, 1));
            Assert.Equal((UInt32)0x03040102, AES.ShiftRow(0x01020304, 2));
            Assert.Equal((UInt32)0x04010203, AES.ShiftRow(0x01020304, 3));
            Assert.Equal((UInt32)0x2F9392C0, AES.ShiftRow(0xC02F9392, 1));
            Assert.Equal((UInt32)0xAFC7AB30, AES.ShiftRow(0xAB30AFC7, 2));
            Assert.Equal((UInt32)0xA220CB2B, AES.ShiftRow(0x20CB2BA2, 3));
        }

        [Fact]
        public void InvShiftRow__ShouldUndo__ShiftRow()
        {
            Assert.Equal((UInt32)0x01020304, AES.InvShiftRow(AES.ShiftRow(0x01020304, 0), 0));
            Assert.Equal((UInt32)0x01020304, AES.InvShiftRow(AES.ShiftRow(0x01020304, 1), 1));
            Assert.Equal((UInt32)0x01020304, AES.InvShiftRow(AES.ShiftRow(0x01020304, 2), 2));
            Assert.Equal((UInt32)0x01020304, AES.InvShiftRow(AES.ShiftRow(0x01020304, 3), 3));
            Assert.Equal((UInt32)0xC02F9392, AES.InvShiftRow(AES.ShiftRow(0xC02F9392, 1), 1));
            Assert.Equal((UInt32)0xAB30AFC7, AES.InvShiftRow(AES.ShiftRow(0xAB30AFC7, 2), 2));
            Assert.Equal((UInt32)0x20CB2BA2, AES.InvShiftRow(AES.ShiftRow(0x20CB2BA2, 3), 3));
        }
        #endregion

        #region MixColumns
        [Fact]
        public void InvMixColumns__ShouldUndo__MixColumns()
        {
            var prng = new Prng();

            for (int i = 0; i <= 10; i++)
            {
                var state = prng.Next(16).ToBinary();
                Assert.Equal(state.Content, AES.InvMixColumns(AES.MixColumns(state)).Content);
            }
        }
        #endregion
    }
}