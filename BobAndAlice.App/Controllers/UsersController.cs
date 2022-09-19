using BobAndAlice.App.Filters;
using BobAndAlice.App.Models.User;
using BobAndAlice.App.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BobAndAlice.App.Controllers
{
    [Route("/api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService userService;
        private readonly ModelConverter mc;

        public UsersController(UserService userService, ModelConverter mc)
        {
            this.userService = userService;
            this.mc = mc;
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
    }
}
