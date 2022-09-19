using BobAndAlice.App.Database;
using BobAndAlice.App.Entities;
using BobAndAlice.App.Exceptions;
using BobAndAlice.App.Models.User;
using System.Threading.Tasks;

namespace BobAndAlice.App.Services
{
    public class UserService
    {
        private readonly UserRepository userRepository;

        public UserService(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<User> CreateUserAsync(string cpf, string name, string password)
        {
            if (cpf.Length > 11)
            {
                throw new AppException($"CPF deve ter até 11 dígitos");
            }

            if (name.Length > User.NameMaxLength)
            {
                throw new AppException($"Nome deve ter até {User.NameMaxLength} caracteres");
            }

            var alreadyExistingUser = await userRepository.GetUserByCpfOrDefaultAsync(cpf);

            if (alreadyExistingUser != null)
            {
                throw new AppException($"Usuário com CPF {alreadyExistingUser.Cpf} já existe");
            }

            var user = new User(cpf, name, password);
            userRepository.AddUser(user);
            await userRepository.SaveChangesAsync();

            return user;
        }

        public async Task<User> LoginAsync(string cpf, string password)
        {
            var user = await userRepository.GetUserByCpfOrDefaultAsync(cpf);

            if (user == null)
            {
                throw new AppException($"Usuário com CPF {cpf} não encontrado");
            }

            return user.VerifyPassword(password) ? user : throw new AppException($"Senha incorreta");
        }
    }
}
