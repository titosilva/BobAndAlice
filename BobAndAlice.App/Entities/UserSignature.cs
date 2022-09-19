using System;
using System.ComponentModel.DataAnnotations;

namespace BobAndAlice.App.Entities
{
    public class UserSignature
    {
        public const int FileNameMaxLength = 300;

        public UserSignature() { }

        public UserSignature(User user, string fileName, byte[] encryptedData, byte[] signatureData, byte[] publicKey)
        {
            Id = Guid.NewGuid();
            User = user;
            FileName = fileName;
            EncryptedData = encryptedData;
            SignatureData = signatureData;
            PublicKey = publicKey;
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
        public byte[] PublicKey { get; private set; }
    }
}
