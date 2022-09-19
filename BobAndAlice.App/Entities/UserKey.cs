using BobAndAlice.Core.Crypto.Asymmetric;
using BobAndAlice.Core.Maths;
using System;
using System.ComponentModel.DataAnnotations;

namespace BobAndAlice.App.Entities
{
    public class UserKey
    {
        public UserKey() { }

        public UserKey(Guid userId, byte[] modulus, byte[] publicKey, byte[] privateKey)
        {
            Id = Guid.NewGuid();
            Modulus = modulus;
            PublicKey = publicKey;
            PrivateKey = privateKey;
            UserId = userId;
        }

        [Key]
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }
        public User User { get; private set; }

        public byte[] Modulus { get; private set; }
        public byte[] PublicKey { get; private set; }
        public byte[] PrivateKey { get; private set; }

        public RsaKey PublicKeyObject
            => new RsaKey(new Binary(Modulus).ToBigInteger(), new Binary(PublicKey).ToBigInteger());

        public RsaKey PrivateKeyObject
            => new RsaKey(new Binary(Modulus).ToBigInteger(), new Binary(PrivateKey).ToBigInteger());
    }
}
