using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace BobAndAlice.App.Entities
{
    public class User
    {
        public const int NameMaxLength = 50;

        public User() { }

        public User(string cpf, string name, string password)
        {
            Id = Guid.NewGuid();
            Cpf = cpf;
            Name = name;

            SetPassword(password);
        }

        [Key]
        public Guid Id { get; private set; }

        [MaxLength(11)]
        public string Cpf { get; private set; }

        [Required, MaxLength(50)]
        public string Name { get; private set; }

        [MaxLength(256)]
        public byte[] PasswordHash { get; private set; }

        public List<UserKey> Keys { get; private set; }

        #region Methods
        public void SetPassword(string password)
            => PasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password.Trim()));

        public bool VerifyPassword(string providedPassword)
            => Convert.ToBase64String(PasswordHash) == Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(providedPassword.Trim())));
        #endregion
    }
}
