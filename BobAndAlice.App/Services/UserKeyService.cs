using BobAndAlice.App.Configuration;
using BobAndAlice.App.Database;
using BobAndAlice.App.Entities;
using BobAndAlice.App.Exceptions;
using BobAndAlice.Core.Crypto.Asymmetric;
using BobAndAlice.Core.Encoding;
using BobAndAlice.Core.Maths;
using Microsoft.Extensions.Options;
using System.Numerics;
using System.Threading.Tasks;

namespace BobAndAlice.App.Services
{
    public class UserKeyService
    {
        private readonly IOptions<AppConfiguration> config;
        private readonly UserKeyRepository userKeyRepository;
        private readonly MillerRabin millerRabin;

        public UserKeyService(IOptions<AppConfiguration> config, UserKeyRepository userKeyRepository)
        {
            this.config = config;
            this.userKeyRepository = userKeyRepository;
            this.millerRabin = new MillerRabin();
        }

        public RsaKeyPair GenerateRsaKeyPair()
            => RsaKeyGen.GenerateKeys(config.Value.RsaPrimeSizeBytes);

        public (BigInteger Prime1, BigInteger Prime2) GeneratePrimes()
            => RsaKeyGen.GeneratePrimes(config.Value.RsaPrimeSizeBytes);

        public async Task<UserKey> CreateUserKeysAsync(User user, string firstPrimeBase64, string secondPrimeBase64)
        {
            BigInteger p, q;
            try
            {
                p = Base64.ToBigInteger(firstPrimeBase64);
                q = Base64.ToBigInteger(secondPrimeBase64);
            } catch
            {
                throw new AppException("Base64 inválido");
            }

            if (!millerRabin.IsPrime(p) || !millerRabin.IsPrime(q))
            {
                throw new AppException("Os números fornecidos devem ser primos");
            }

            var keyPair = RsaKeyGen.GenerateKeysFromPrimes(p, q, config.Value.RsaPrimeSizeBytes);

            var userKey = new UserKey(user.Id, keyPair.Modulus.ToByteArray(), keyPair.PublicKey.Value.ToByteArray(), keyPair.PrivateKey.Value.ToByteArray());
            userKeyRepository.AddUserKey(userKey);
            await userKeyRepository.SaveChangesAsync();

            return userKey;
        }
    }
}
