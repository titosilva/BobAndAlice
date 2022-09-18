using BobAndAlice.App.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BobAndAlice.App.Database
{
    public class UserKeyRepository
    {
        private readonly AppDbContext context;

        public UserKeyRepository(AppDbContext context)
        {
            this.context = context;
        }

        public IQueryable<UserKey> UserKeys
            => context.UserKeys.Include(uk => uk.User);

        public void AddUserKey(UserKey userKey)
            => context.UserKeys.Add(userKey);

        public async Task SaveChangesAsync()
            => await context.SaveChangesAsync();
    }
}
