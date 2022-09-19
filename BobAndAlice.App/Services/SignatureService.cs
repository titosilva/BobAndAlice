using BobAndAlice.App.Configuration;
using BobAndAlice.App.Database;
using BobAndAlice.App.Entities;
using BobAndAlice.App.Models.File;
using BobAndAlice.App.Models.Signatures;
using BobAndAlice.Core.Crypto.Asymmetric;
using BobAndAlice.Core.Crypto.Signatures;
using BobAndAlice.Core.Encoding;
using BobAndAlice.Core.Maths;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace BobAndAlice.App.Services
{
    public class SignatureService
    {
        private readonly FileService fileService;
        private readonly IOptions<AppConfiguration> appConfig;
        private readonly UserSignatureRepository userSignatureRepository;

        public SignatureService(FileService fileService, IOptions<AppConfiguration> appConfig, UserSignatureRepository userSignatureRepository)
        {
            this.fileService = fileService;
            this.appConfig = appConfig;
            this.userSignatureRepository = userSignatureRepository;
        }

        public async Task<UserSignature> CreateSignatureAsync(User user, FileModel file)
        {
            var fileContent = fileService.ReadFile(file.Id);
            var key = user.Keys.First();
            var signer = new Signer(appConfig.Value.SignerMode, key.PublicKeyObject, key.PrivateKeyObject);

            var signature = signer.Sign(new Binary(fileContent));

            var userSignature = new UserSignature(user, file.FileName, signature);
            userSignatureRepository.Add(userSignature);
            await userSignatureRepository.SaveChangesAsync();

            return userSignature;
        }

        public SignatureModel VerifySignature(UserSignature userSignature)
        {
            var result = new SignatureModel()
            {
                Id = userSignature.Id,
                EncryptedDataBase64 = Base64.FromByteArray(userSignature.EncryptedData),
                PublicKeyModulusBase64 = Base64.FromByteArray(userSignature.PublicKeyModulus),
                PublicKeyBase64 = Base64.FromByteArray(userSignature.PublicKey),
                SignatureParametersBase64 = Base64.FromByteArray(userSignature.SignatureData),
            };

            var signer = new Signer(appConfig.Value.SignerMode);

            var signedData = new Binary(userSignature.SignatureData);
            var publicKey = new RsaKey(new BigInteger(userSignature.PublicKeyModulus), new BigInteger(userSignature.PublicKey));

            if (!signer.TryDecryptSignature(signedData, publicKey, out var decryptedSignature))
            {
                result.Failure = "Não foi possível decriptar os campos da assinatura RSA";
                return result;
            }

            var signatureFields = signer.ReadDecryptedSignature(decryptedSignature);

            Binary decryptedMessage;
            try
            {
                decryptedMessage = signer.AesDecrypt(new Binary(userSignature.EncryptedData), signatureFields.AesKey, signatureFields.AesPaddingSize);
            } catch
            {
                result.Failure = "Não foi possível decriptar a mensagem cifrada com AES para verificação da assinatura";
                return result;
            }

            var decryptedMessageBase64 = Base64.FromByteArray(decryptedMessage.ToByteArray());
            var messageHashBase64 = Base64.FromByteArray(signatureFields.MessageHash.ToByteArray());

            result.Failure = Base64.FromByteArray(signer.HashMessage(decryptedMessage).ToByteArray()) == messageHashBase64 ? null : "O hash da mensagem decriptada não é igual ao hash da assinatura";
            result.DataHashBase64 = messageHashBase64;
            result.DecryptedDataBase64 = decryptedMessageBase64;
            result.EncryptioKeyBase64 = Base64.FromByteArray(signatureFields.AesKey.ToByteArray());
            result.EncryptionPaddingSize = signatureFields.AesPaddingSize;

            return result;
        }
    }
}
