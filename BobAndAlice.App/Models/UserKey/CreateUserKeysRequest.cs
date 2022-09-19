using System;

namespace BobAndAlice.App.Models.UserKey
{
    public class CreateUserKeysRequest
    {
        public Guid UserId { get; set; }
        public string Prime1Base64 { get; set; }
        public string Prime2Base64 { get; set; }
    }
}
