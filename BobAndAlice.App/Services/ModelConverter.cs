using BobAndAlice.App.Entities;
using BobAndAlice.App.Models.User;
using BobAndAlice.App.Models.UserKey;
using BobAndAlice.Core.Encoding;

namespace BobAndAlice.App.Services
{
    public class ModelConverter
    {
        public UserModel ToModel(User user)
            => new UserModel()
            {
                Id = user.Id,
                Cpf = user.Cpf,
                Name = user.Name,
            };

        public UserKeyModel ToModel(UserKey key)
            => new UserKeyModel()
            {
                ModulusBase64 = Base64.FromByteArray(key.Modulus),
                PrivateKeyBase64 = Base64.FromByteArray(key.PrivateKey),
                PublicKeyBase64 = Base64.FromByteArray(key.PublicKey),
            };
    }
}
