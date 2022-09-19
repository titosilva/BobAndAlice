using BobAndAlice.App.Database;
using BobAndAlice.App.Models;
using BobAndAlice.App.Models.Signatures;
using BobAndAlice.App.Services;
using BobAndAlice.Core.Encoding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
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

        [HttpGet("{id}/original-file")]
        public async Task<IActionResult> DownloadOriginalFile([FromRoute] Guid id, [FromQuery] string fileName/* For naming the downloaded file */)
        {
            var signature = await userSignatureRepository.GetRequiredUserSignatureAsync(id);
            var signatureVerification = signatureService.VerifySignature(signature);
            var fileBytes = Base64.ToByteArray(signatureVerification.DecryptedDataBase64);

            var stream = new MemoryStream();
            stream.Write(fileBytes);
            stream.Position = 0;

            return File(stream, "application/octet-stream", fileName);
        }

        [HttpGet("{id}/file")]
        public async Task<IActionResult> DownloadFile([FromRoute] Guid id, [FromQuery] string fileName/* For naming the downloaded file */)
        {
            var userSignature = await userSignatureRepository.GetRequiredUserSignatureAsync(id);
            var fileContents = signatureService.ToJsonFileContent(userSignature.Extract());
            var fileBytes = Encoding.UTF8.GetBytes(fileContents);

            var stream = new MemoryStream();
            stream.Write(fileBytes);
            stream.Position = 0;

            return File(stream, "application/octet-stream", Path.ChangeExtension(fileName, ".json"));
        }

        [HttpGet("from-file")]
        public SignatureModel OpenAndVerifyFile([FromQuery] Guid fileId, [FromQuery] string fileName/* For naming the downloaded file */)
        {
            return signatureService.OpenAndVerifyFromFile(fileId, fileName);
        }

        [HttpGet("from-file/download")]
        public IActionResult OpenAndDecryptFile([FromQuery] Guid fileId, [FromQuery] string fileName/* For naming the downloaded file */)
        {
            var (fileBytes, originalFileName) = signatureService.OpenAndDecryptFromFile(fileId);

            var stream = new MemoryStream();
            stream.Write(fileBytes);
            stream.Position = 0;

            return File(stream, "application/octet-stream", originalFileName);
        }
    }
}
