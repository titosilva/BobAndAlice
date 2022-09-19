using BobAndAlice.App.Configuration;
using BobAndAlice.App.Exceptions;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BobAndAlice.App.Services
{
    public class FileService
    {
        private readonly IOptions<AppConfiguration> appConfig;

        public FileService(IOptions<AppConfiguration> appConfig)
        {
            this.appConfig = appConfig;
        }

        public byte[] ReadFile(Guid fileId)
        {
            var filePath = Path.Join(appConfig.Value.UploadsBasePath, fileId.ToString());

            if (!File.Exists(filePath))
            {
                throw new AppException("O arquivo indicado não existe");
            }

            return File.ReadAllBytes(filePath);
        }
    }
}
