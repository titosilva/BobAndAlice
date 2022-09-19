using BobAndAlice.App.Configuration;
using BobAndAlice.App.Exceptions;
using BobAndAlice.App.Models.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BobAndAlice.App.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IOptions<AppConfiguration> appConfig;

        public FilesController(IOptions<AppConfiguration> appConfig)
        {
            this.appConfig = appConfig;
        }

        [HttpPost("upload")]
        public async Task<FileModel> Upload()
        {
            var file = Request.Form.Files.First();
            var filename = file.FileName;
            using var streamToRead = file.OpenReadStream();
                
            Directory.CreateDirectory(appConfig.Value.UploadsBasePath);
            var fileGuid = Guid.NewGuid();
            using var streamToWrite = System.IO.File.Open(Path.Join(appConfig.Value.UploadsBasePath, fileGuid.ToString()), FileMode.Create);

            streamToRead.CopyTo(streamToWrite);

            return new FileModel()
            {
                Id = fileGuid,
                FileName = filename,
            };
        }
    }
}
