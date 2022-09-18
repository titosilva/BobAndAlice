using BobAndAlice.App.Entities;
using BobAndAlice.App.Models.User;

namespace BobAndAlice.App.Services
{
    public class ModelConverter
    {
        public UserModel ToModel(User user)
            => new UserModel()
            {
                Cpf = user.Cpf,
            };
    }
}
