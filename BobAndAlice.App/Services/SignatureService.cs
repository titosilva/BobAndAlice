using BobAndAlice.App.Configuration;
using BobAndAlice.App.Database;
using BobAndAlice.App.Entities;
using BobAndAlice.App.Exceptions;
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
                FileName = userSignature.FileName,
            };

            return VerifySignatureAndFill(userSignature.Extract(), result);
        }

        public SignatureModel VerifySignatureAndFill(Signature signature, SignatureModel model)
        {
            model.EncryptedDataBase64 = Base64.FromByteArray(signature.EncryptedMessage.ToByteArray());
            model.PublicKeyModulusBase64 = Base64.FromBigInteger(signature.SignerPublicKey.Modulus);
            model.PublicKeyBase64 = Base64.FromBigInteger(signature.SignerPublicKey.Value);
            model.SignatureParametersBase64 = Base64.FromByteArray(signature.SignedHashAndParameters.ToByteArray());

            var signer = new Signer(appConfig.Value.SignerMode);

            if (!signer.TryDecryptSignature(signature.SignedHashAndParameters, signature.SignerPublicKey, out var decryptedSignature))
            {
                model.Failure = "Não foi possível decriptar os campos da assinatura RSA";
                return model;
            }

            (Binary MessageHash, Binary AesKey, byte AesPaddingSize) signatureFields;
            try
            {
                signatureFields = signer.ReadDecryptedSignature(decryptedSignature);
            } catch
            {
                model.Failure = "Os campos da assinatura não são válidos";
                return model;
            }

            Binary decryptedMessage;
            try
            {
                decryptedMessage = signer.AesDecrypt(new Binary(signature.EncryptedMessage), signatureFields.AesKey, signatureFields.AesPaddingSize);
            }
            catch
            {
                model.Failure = "Não foi possível decriptar a mensagem cifrada com AES para verificação da assinatura";
                return model;
            }

            var decryptedMessageBase64 = Base64.FromByteArray(decryptedMessage.ToByteArray());
            var messageHashBase64 = Base64.FromByteArray(signatureFields.MessageHash.ToByteArray());

            model.Failure = Base64.FromByteArray(signer.HashMessage(decryptedMessage).ToByteArray()) == messageHashBase64 ? null : "O hash da mensagem decriptada não é igual ao hash da assinatura";
            model.DataHashBase64 = messageHashBase64;
            model.DecryptedDataBase64 = decryptedMessageBase64;
            model.EncryptionKeyBase64 = Base64.FromByteArray(signatureFields.AesKey.ToByteArray());
            model.EncryptionPaddingSize = signatureFields.AesPaddingSize;

            return model;
        }

        public string ToJsonFileContent(Signature signature)
            => new SignatureFileModel()
            {
                EncryptedData = Base64.FromByteArray(signature.EncryptedMessage.ToByteArray()),
                Signature = Base64.FromByteArray(signature.SignedHashAndParameters.ToByteArray()),
                PublicKeyModulus = Base64.FromBigInteger(signature.SignerPublicKey.Modulus),
                PublicKeyValue = Base64.FromBigInteger(signature.SignerPublicKey.Value),
            }.Serialize();

        public Signature FromJsonFileContent(string s)
        {
            if (!SignatureFileModel.TryDeserialize(s, out var deserialized))
            {
                throw new AppException("Formato inválido de arquivo");
            }

            return new Signature()
            {
                EncryptedMessage = Base64.ToBinary(deserialized.EncryptedData),
                SignedHashAndParameters = Base64.ToBinary(deserialized.Signature),
                SignerPublicKey = new RsaKey(
                    Base64.ToBigInteger(deserialized.PublicKeyModulus),
                    Base64.ToBigInteger(deserialized.PublicKeyValue)
                )
            };
        }
    }
}
