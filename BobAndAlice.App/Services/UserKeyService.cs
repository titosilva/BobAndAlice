using BobAndAlice.App.Configuration;
using BobAndAlice.App.Database;
using BobAndAlice.App.Entities;
using BobAndAlice.Core.Crypto.Asymmetric;
using BobAndAlice.Core.Encoding;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace BobAndAlice.App.Services
{
    public class UserKeyService
    {
        private readonly IOptions<AppConfiguration> config;
        private readonly UserKeyRepository userKeyRepository;

        public UserKeyService(IOptions<AppConfiguration> config, UserKeyRepository userKeyRepository)
        {
            this.config = config;
            this.userKeyRepository = userKeyRepository;
        }

        public RsaKeyPair GenerateRsaKeyPair()
            => RsaKeyGen.GenerateKeys(config.Value.RsaPrimeSizeBytes);

        public async Task<UserKey> SaveUserKeysAsync(User user, string firstPrimeBase64, string secondPrimeBase64)
        {
            var p = Base64.ToBigInteger(firstPrimeBase64);
            var q = Base64.ToBigInteger(secondPrimeBase64);
            var keyPair = RsaKeyGen.GenerateKeysFromPrimes(p, q, config.Value.RsaPrimeSizeBytes);

            var userKey = new UserKey(user.Id, keyPair.Modulus.ToByteArray(), keyPair.PublicKey.Value.ToByteArray(), keyPair.PrivateKey.Value.ToByteArray());
            userKeyRepository.AddUserKey(userKey);
            await userKeyRepository.SaveChangesAsync();

            return userKey;
        }
    }
}
