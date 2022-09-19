using BobAndAlice.Core.Crypto.Asymmetric;
using BobAndAlice.Core.Crypto.Signatures;
using BobAndAlice.Core.Maths;
using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace BobAndAlice.App.Entities
{
    public class UserSignature
    {
        public const int FileNameMaxLength = 300;

        public UserSignature() { }

        public UserSignature(User user, string fileName, Signature signature)
        {
            Id = Guid.NewGuid();
            UserId = user.Id;
            FileName = fileName;
            EncryptedData = signature.EncryptedMessage.ToByteArray();
            SignatureData = signature.SignedHashAndParameters.ToByteArray();
            PublicKeyModulus = signature.SignerPublicKey.Modulus.ToByteArray();
            PublicKey = signature.SignerPublicKey.Value.ToByteArray();
        }

        [Key]
        public Guid Id { get; set; }


        public Guid UserId { get; set; }
        public User User { get; set; }


        [Required, MaxLength(FileNameMaxLength)]
        public string FileName { get; private set; }

        [Required]
        public byte[] EncryptedData { get; private set; }

        [Required]
        public byte[] SignatureData { get; private set; }

        [Required]
        public byte[] PublicKeyModulus { get; private set; }

        [Required]
        public byte[] PublicKey { get; private set; }

        public Signature Extract()
            => new Signature()
            {
                EncryptedMessage = new Binary(EncryptedData),
                SignerPublicKey = new RsaKey(new BigInteger(PublicKeyModulus), new BigInteger(PublicKey)),
                SignedHashAndParameters = new Binary(SignatureData),
            };
    }
}
