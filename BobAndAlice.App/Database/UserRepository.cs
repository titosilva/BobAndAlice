using BobAndAlice.App.Entities;
using Microsoft.EntityFrameworkCore;
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

        public async Task<User> GetUserByCpfOrDefaultAsync(string cpf)
            => await Users.FirstOrDefaultAsync(u => u.Cpf == cpf);

        public async Task SaveChangesAsync()
            => await context.SaveChangesAsync();
    }
}
