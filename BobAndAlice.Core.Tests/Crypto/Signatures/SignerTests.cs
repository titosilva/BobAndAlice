using BobAndAlice.Core.Crypto.Asymmetric;
using BobAndAlice.Core.Crypto.Signatures;
using BobAndAlice.Core.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BobAndAlice.Core.Tests.Crypto.Signatures
{
    public class SignerTests
    {
        [Fact]
        public void SignerVerifySignature__ShouldReturnTrue__ForSignerSignGeneratedSignature()
        {
            var keyPair = RsaKeyGen.GenerateKeys();
            var prng = new Prng();
            var signer = new Signer(Signer.SignerModes.Hash256Encryption128, keyPair.PublicKey, keyPair.PrivateKey);

            for (int i = 0; i < 10; i++)
            {
                var randomMessageSize = Math.Abs((int)(prng.NextByte()) + 20);
                var randomMessageToSign = prng.Next(randomMessageSize).ToBinary();

                Assert.True(signer.VerifySignature(signer.Sign(randomMessageToSign), out _));
            }
        }

        [Fact]
        public void SignerVerifySignature__ShouldProvidedCorrectDecryptedMessage__ForSignerSignGeneratedSignature()
        {
            var keyPair = RsaKeyGen.GenerateKeys();
            var prng = new Prng();
            var signer = new Signer(Signer.SignerModes.Hash256Encryption128, keyPair.PublicKey, keyPair.PrivateKey);

            for (int i = 0; i < 10; i++)
            {
                var randomMessageSize = Math.Abs(prng.NextByte() + 20);
                var randomMessageToSign = prng.Next(randomMessageSize).ToBinary();

                signer.VerifySignature(signer.Sign(randomMessageToSign), out var recoveredMessage);
                Assert.Equal(randomMessageToSign.Content, recoveredMessage.Content);
            }
        }

        [Fact]
        public void SignerVerifySignature__ShouldReturnFalse__ForModifiedMessage()
        {
            var keyPair = RsaKeyGen.GenerateKeys();
            var prng = new Prng();
            var signer = new Signer(Signer.SignerModes.Hash256Encryption128, keyPair.PublicKey, keyPair.PrivateKey);

            for (int i = 0; i < 10; i++)
            {
                var randomMessageSize = Math.Abs(prng.NextByte() + 20);
                var randomMessageToSign = prng.Next(randomMessageSize).ToBinary();

                var signature = signer.Sign(randomMessageToSign);
                var byteToModify = randomMessageSize >> 1;
                signature.EncryptedMessage.Content[byteToModify] = (byte)~signature.EncryptedMessage.Content[byteToModify];

                Assert.False(signer.VerifySignature(signature, out _));
            }
        }

        [Fact]
        public void SignerVerifySignature__ShouldReturnFalse__ForModifiedSignature()
        {
            var keyPair = RsaKeyGen.GenerateKeys();
            var prng = new Prng();
            var signer = new Signer(Signer.SignerModes.Hash256Encryption128, keyPair.PublicKey, keyPair.PrivateKey);

            for (int i = 0; i < 10; i++)
            {
                var randomMessageSize = Math.Abs(prng.NextByte() + 20);
                var randomMessageToSign = prng.Next(randomMessageSize).ToBinary();

                var signature = signer.Sign(randomMessageToSign);
                signature.SignedHashAndParameters.Content[0] = (byte)~signature.SignedHashAndParameters.Content[0];

                Assert.False(signer.VerifySignature(signature, out _));
            }
        }
    }
}
