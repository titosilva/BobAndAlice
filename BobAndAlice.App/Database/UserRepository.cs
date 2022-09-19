using BobAndAlice.App.Entities;
using BobAndAlice.App.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BobAndAlice.App.Database
{
    public class UserRepository
    {
        private readonly AppDbContext context;

        public UserRepository(AppDbContext context)
        {
            this.context = context;
        }

        public IQueryable<User> Users
            => context.Users.Include(u => u.Keys).AsNoTracking();

        public void AddUser(User user)
            => context.Users.Add(user);

        public void UpdateUser(User user)
            => context.Users.Update(user);

        public async Task<User> GetUserOrDefaultAsync(Guid userId)
            => await Users.FirstOrDefaultAsync(u => u.Id == userId);

        public async Task<User> GetRequiredUserAsync(Guid userId)
            => (await GetUserOrDefaultAsync(userId)) ?? throw new AppException("Usuário não encontrado");

        public async Task<User> GetUserByCpfOrDefaultAsync(string cpf)
            => await Users.FirstOrDefaultAsync(u => u.Cpf == cpf);


        public async Task SaveChangesAsync()
            => await context.SaveChangesAsync();
    }
}
