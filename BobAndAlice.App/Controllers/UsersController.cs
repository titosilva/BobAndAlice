using BobAndAlice.App.Database;
using BobAndAlice.App.Exceptions;
using BobAndAlice.App.Filters;
using BobAndAlice.App.Models.User;
using BobAndAlice.App.Models.UserKey;
using BobAndAlice.App.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BobAndAlice.App.Controllers
{
    [Route("/api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository userRepository;
        private readonly UserService userService;
        private readonly ModelConverter mc;

        public UsersController(UserService userService, ModelConverter mc, UserRepository userRepository)
        {
            this.userService = userService;
            this.mc = mc;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<UserModel> CreateUser([FromBody] UserData data)
        {
            var user = await userService.CreateUserAsync(data.Cpf, data.Name, data.Password);
            return mc.ToModel(user);
        }

        [HttpPost("login")]
        public async Task<UserModel> Login([FromBody] LoginRequest request)
        {
            var user = await userService.LoginAsync(request.Cpf, request.Password);
            return mc.ToModel(user);
        }

        [HttpGet("{userId}/keys")]
        public async Task<List<UserKeyModel>> GetUserKeys([FromRoute] Guid userId)
        {
            var user = await userRepository.GetUserOrDefaultAsync(userId);

            if (user == null)
            {
                throw new AppException($"Usuário não encontrado");
            }

            return user.Keys.ConvertAll(uk => mc.ToModel(uk));
        }
    }
}
