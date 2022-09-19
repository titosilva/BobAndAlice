using BobAndAlice.App.Database;
using BobAndAlice.App.Models;
using BobAndAlice.App.Models.Signatures;
using BobAndAlice.App.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BobAndAlice.App.Controllers
{
    [Route("api/signatures")]
    [ApiController]
    public class SignaturesController : ControllerBase
    {
        private readonly UserRepository userRepository;
        private readonly UserSignatureRepository userSignatureRepository;
        private readonly SignatureService signatureService;

        public SignaturesController(UserRepository userRepository, SignatureService signatureService, UserSignatureRepository userSignatureRepository)
        {
            this.userRepository = userRepository;
            this.signatureService = signatureService;
            this.userSignatureRepository = userSignatureRepository;
        }

        [HttpPost]
        public async Task<SignatureModel> CreateSignature([FromBody] SignatureData data)
        {
            var user = await userRepository.GetRequiredUserAsync(data.UserId);

            var userSignature = await signatureService.CreateSignatureAsync(user, data.File);

            return signatureService.VerifySignature(userSignature);
        }

        [HttpGet("{id}")]
        public async Task<SignatureModel> GetSignature([FromRoute] Guid id)
        {
            var signature = await userSignatureRepository.GetRequiredUserSignatureAsync(id);
            return signatureService.VerifySignature(signature);
        }
    }
}
