using BobAndAlice.App.Database;
using BobAndAlice.App.Models.UserKey;
using BobAndAlice.App.Services;
using BobAndAlice.Core.Encoding;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BobAndAlice.App.Controllers
{
    [Route("/api/keys")]
    [ApiController]
    public class UserKeysController : ControllerBase
    {
        private readonly UserKeyService userKeyService;
        private readonly UserRepository userRepository;
        private readonly ModelConverter mc;

        public UserKeysController(UserKeyService userKeyService, UserRepository userRepository, ModelConverter mc)
        {
            this.userKeyService = userKeyService;
            this.userRepository = userRepository;
            this.mc = mc;
        }

        [HttpPost]
        public async Task<UserKeyModel> CreateUserKeys(CreateUserKeysRequest request)
        {
            var user = await userRepository.GetRequiredUserAsync(request.UserId);
            var userKey = await userKeyService.CreateUserKeysAsync(user, request.Prime1Base64, request.Prime2Base64);
            return mc.ToModel(userKey);
        }

        [HttpGet("random-primes")]
        public GeneratePrimesResponse GeneratePrimes()
        {
            var (p, q) = userKeyService.GeneratePrimes();
            return new GeneratePrimesResponse()
            {
                Prime1Base64 = Base64.FromBigInteger(p),
                Prime2Base64 = Base64.FromBigInteger(q),
            };
        }
    }
}
