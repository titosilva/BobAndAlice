using BobAndAlice.Core.Crypto.Asymmetric;
using BobAndAlice.Core.Crypto.Hash;
using BobAndAlice.Core.Crypto.Symmetric;
using BobAndAlice.Core.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobAndAlice.Core.Crypto.Signatures
{
    public class Signer
    {
        #region Modes
        public enum SignerModes
        {
            Hash256Encryption128,
            Hash256Encryption192,
            Hash224Encryption256,
        }

        public static AES.AESSupportedKeySizes GetAesKeySize(SignerModes mode)
            => mode switch
            {
                SignerModes.Hash256Encryption128 => AES.AESSupportedKeySizes.Bits128,
                SignerModes.Hash256Encryption192 => AES.AESSupportedKeySizes.Bits192,
                SignerModes.Hash224Encryption256 => AES.AESSupportedKeySizes.Bits256,
                _ => throw new ArgumentException("Not supported signer mode"),
            };

        public static SHA3.SHA3SupportedBitSizes GetHashSize(SignerModes mode)
            => mode switch
            {
                SignerModes.Hash256Encryption128 => SHA3.SHA3SupportedBitSizes.Bits256,
                SignerModes.Hash256Encryption192 => SHA3.SHA3SupportedBitSizes.Bits256,
                SignerModes.Hash224Encryption256 => SHA3.SHA3SupportedBitSizes.Bits224,
                _ => throw new ArgumentException("Not supported signer mode"),
            };
        #endregion

        private readonly AES aes;
        private readonly Prng prng;
        private readonly SHA3 messageDigest;
        private readonly RsaOAEP rsa;

        public SignerModes Mode { get; private set; }
        public AES.AESSupportedKeySizes AesKeySize => GetAesKeySize(Mode);
        public SHA3.SHA3SupportedBitSizes HashSize => GetHashSize(Mode);

        // Use 512 bits for signature (hash + aes key + aes padding)
        public int RsaPaddingSize
            => Mode switch
            {
                SignerModes.Hash256Encryption128 => 120 / 8, // 256 (hash) + 128 (aes key) + 8 (aes padding size) + 120 = 512 bits,
                SignerModes.Hash256Encryption192 => 56 / 8, // 256 (hash) + 192 (aes key) + 8 (aes padding size) + 56 = 512 bits,
                SignerModes.Hash224Encryption256 => 24 / 8, // 224 (hash) + 256 (aes key) + 8 (aes padding size) + 24 = 512 bits,
                _ => throw new ArgumentException("Not supported signer mode"),
            };

        public RsaKey SignaturePublicKey { get; private set; }
        public RsaKey SignaturePrivateKey { get; private set; }

        public Signer(SignerModes mode, RsaKey signaturePublicKey, RsaKey signaturePrivateKey)
        {
            Mode = mode;
            SignaturePublicKey = signaturePublicKey;
            SignaturePrivateKey = signaturePrivateKey;

            aes = new AES(AesKeySize);
            prng = new Prng();
            messageDigest = new SHA3(HashSize);

            // Use 512 bits for signature (hash + aes key + aes padding)
            rsa = new RsaOAEP(SHA3.SHA3SupportedBitSizes.Bits512, RsaPaddingSize);
        }

        public Signer(SignerModes mode)
        {
            Mode = mode;

            aes = new AES(AesKeySize);
            prng = new Prng();
            messageDigest = new SHA3(HashSize);

            // Use 512 bits for signature (hash + aes key + aes padding)
            rsa = new RsaOAEP(SHA3.SHA3SupportedBitSizes.Bits512, RsaPaddingSize);
        }

        public Signature Sign(Binary message)
            => SignaturePrivateKey != null && SignaturePublicKey != null? new Signature()
            {
                EncryptedMessage = aesEncryptWithRandomKey(message, out var aesKey, out var paddingSize),
                SignedHashAndParameters = sign(generateDataToSign(message, aesKey, paddingSize)),
                SignerPublicKey = SignaturePublicKey,
            } : throw new Exception("Keys are not set");

        public bool VerifySignature(Signature signature, out Binary decryptedMessage)
        {
            decryptedMessage = null;
            if (!TryDecryptSignature(signature.SignedHashAndParameters, signature.SignerPublicKey, out var decryptedSignature))
            {
                return false;
            }

            var (providedMessageHash, aesKey, aesPaddingSize) = ReadDecryptedSignature(decryptedSignature);

            decryptedMessage = AesDecrypt(signature.EncryptedMessage, aesKey, aesPaddingSize);
            var decryptedMessageHash = HashMessage(decryptedMessage);

            return providedMessageHash == decryptedMessageHash;
        }

        #region AES Encryption / Decryption
        private Binary aesEncryptWithRandomKey(Binary message, out Binary key, out byte paddingSize)
        {
            var paddedMessage = padMessage(message, out paddingSize);
            key = prng.Next(AES.ToByteSize(AesKeySize)).ToBinary();
            return aes.Encrypt(paddedMessage, key);
        }

        public Binary AesDecrypt(Binary encryptedMessage, Binary key, byte paddingSize)
        {
            var paddedMessage = aes.Decrypt(encryptedMessage, key);
            return removePadding(paddedMessage, paddingSize);
        }
        #endregion

        #region Hash and parameters
        public Binary HashMessage(Binary message)
            => messageDigest.Hash(message);

        private Binary generateDataToSign(Binary message, Binary aesKey, byte aesPaddingSize)
        {
            var messageHash = HashMessage(message);
            var paddingSizeBin = new Binary(aesPaddingSize);
            return new Binary(messageHash, aesKey, paddingSizeBin);
        } 

        public (Binary MessageHash, Binary AesKey, byte AesPaddingSize) ReadDecryptedSignature(Binary decryptedSignature)
            => decryptedSignature.Length == AES.ToByteSize(AesKeySize) + SHA3.ToBytesSize(HashSize) + 1? (
                new Binary(decryptedSignature.Content.Take(SHA3.ToBytesSize(HashSize)).ToList()),
                new Binary(decryptedSignature.Content.Skip(SHA3.ToBytesSize(HashSize)).Take(AES.ToByteSize(AesKeySize)).ToList()),
                decryptedSignature.Content.Last()
            ) : throw new ArgumentException("The provided value does not have the expected size");

        private Binary sign(Binary dataToSign)
            => rsa.Encrypt(dataToSign, SignaturePrivateKey);

        public bool TryDecryptSignature(Binary signedData, RsaKey publicKey, out Binary decryptedSignature) {
            try
            {
                decryptedSignature = rsa.Decrypt(signedData, publicKey);
                return true;
            }
            catch (Exception)
            {
                decryptedSignature = null;
                return false;
            }
        }
        #endregion

        #region AES message padding
        private Binary padMessage(Binary message, out byte paddingSize)
        {
            var messageCopy = new Binary(message);

            paddingSize = 0;
            while (messageCopy.Length % 16 != 0)
            {
                messageCopy.Content.Add(0);
                paddingSize++;
            }

            return messageCopy;
        }

        private Binary removePadding(Binary paddedMessage, byte paddingSize)
            => new Binary(paddedMessage.Content.SkipLast(paddingSize).ToList());
        #endregion
    }
}
