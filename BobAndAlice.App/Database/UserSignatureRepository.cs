using BobAndAlice.App.Entities;
using BobAndAlice.App.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BobAndAlice.App.Database
{
    public class UserSignatureRepository
    {
        private readonly AppDbContext context;

        public UserSignatureRepository(AppDbContext context)
        {
            this.context = context;
        }

        public IQueryable<UserSignature> UserSignatures
            => context.UserSignatures.Include(u => u.User);

        public async Task<UserSignature> GetUserSignatureOrDefaultAsync(Guid signatureId)
            => await UserSignatures.FirstOrDefaultAsync(us => us.Id == signatureId);

        public async Task<UserSignature> GetRequiredUserSignatureAsync(Guid signatureId)
            => await GetUserSignatureOrDefaultAsync(signatureId) ?? throw new AppException("Assinatura não encontrada");

        public void Add(UserSignature signature) 
            => context.UserSignatures.Add(signature);

        public async Task SaveChangesAsync()
            => await context.SaveChangesAsync();
    }
}
