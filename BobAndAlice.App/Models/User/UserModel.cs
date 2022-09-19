using System;

namespace BobAndAlice.App.Models.User
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Cpf { get; set; }
        public string Name { get; set; }
    }
}
