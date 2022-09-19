using BobAndAlice.App.Models.File;
using System;

namespace BobAndAlice.App.Models.Signatures
{
    public class SignatureData
    {
        public Guid UserId { get; set; }
        public FileModel File { get; set; }
    }
}
