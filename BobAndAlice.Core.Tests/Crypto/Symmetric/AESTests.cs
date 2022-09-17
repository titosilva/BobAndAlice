using System;
using System.Collections.Generic;
using BobAndAlice.Core.Crypto.Symmetric;
using BobAndAlice.Core.Maths;
using Xunit;

namespace BobAndAlice.Core.Tests.Crypto.Symmetric
{
    public class AESTests
    {
        #region Encrypt/Decrypt block

        [Fact]
        public void AESDecrypt__ShouldUndo__AESEncrypt()
        {
            var prng = new Prng();
            var aes = new AES(AES.AESSupportedKeySizes.Bits128, prng.Next(16).ToBinary());
            var randomBlock = prng.Next(16).ToBinary();
            Assert.Equal(randomBlock, aes.DecryptBlock(aes.EncryptBlock(randomBlock)));
        }
        #endregion
        
        #region SubBytes
        [Fact]
        public void SubByte__ShouldReturnByte__BasedOnSBoxTable()
        {
            var prng = new Prng();
            var aes = new AES(AES.AESSupportedKeySizes.Bits128, prng.Next(16).ToBinary());

            var testPairs = new List<(byte Input, byte ExpectedOutput)>()
            { (0x9a, 0xb8), (0x0f, 0x76),  (0xf8, 0x41), (0xbe, 0xae), };

            foreach (var pair in testPairs)
            {
                Assert.Equal(pair.ExpectedOutput, aes.SubByte(pair.Input));
            }
        }

        [Fact]
        public void InvSubByte__ShouldReturnByte__BasedOnInvSBoxTable()
        {
            var prng = new Prng();
            var aes = new AES(AES.AESSupportedKeySizes.Bits128, prng.Next(16).ToBinary());

            var testPairs = new List<(byte ExpectedOutput, byte Input)>()
                {(0x9a, 0xb8), (0x0f, 0x76), (0xf8, 0x41), (0xbe, 0xae),};

            foreach (var pair in testPairs)
            {
                Assert.Equal(pair.ExpectedOutput, aes.InvSubByte(pair.Input));
            }
        }
        #endregion

        #region ShiftRows
        [Fact]
        public void ShiftRow__ShouldReturnWord__ProperlyShifted()
        {
            var prng = new Prng();
            var aes = new AES(AES.AESSupportedKeySizes.Bits128, prng.Next(16).ToBinary());
            Assert.Equal((UInt32) 0x01020304, aes.ShiftRow(0x01020304, 0));
            Assert.Equal((UInt32) 0x02030401, aes.ShiftRow(0x01020304, 1));
            Assert.Equal((UInt32) 0x03040102, aes.ShiftRow(0x01020304, 2));
            Assert.Equal((UInt32) 0x04010203, aes.ShiftRow(0x01020304, 3)); 
            Assert.Equal((UInt32) 0x2F9392C0, aes.ShiftRow(0xC02F9392, 1));
            Assert.Equal((UInt32) 0xAFC7AB30, aes.ShiftRow(0xAB30AFC7, 2));
            Assert.Equal((UInt32) 0xA220CB2B, aes.ShiftRow(0x20CB2BA2, 3));
        }

        [Fact]
        public void InvShiftRow__ShouldUndo__ShiftRow()
        {
            var prng = new Prng();
            var aes = new AES(AES.AESSupportedKeySizes.Bits128, prng.Next(16).ToBinary());
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
        [Fact]
        public void InvMixColumns__ShouldUndo__MixColumns()
        {
            var prng = new Prng();
            var aes = new AES(AES.AESSupportedKeySizes.Bits128, prng.Next(16).ToBinary());
            
            for (int i = 0; i <= 10; i++)
            {
                var state = prng.Next(16).ToBinary();
                Assert.Equal(state.Content, aes.InvMixColumns(aes.MixColumns(state)).Content);
            }
        }
        #endregion
    }
}