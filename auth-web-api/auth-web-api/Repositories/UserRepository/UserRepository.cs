using auth_web_api.Models.DatabaseObjects;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_web_api.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext databaseContext;
        public UserRepository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task<User> Create(User user)
        {
            user.Id = Guid.NewGuid();

            await databaseContext.Users.AddAsync(user);

            await databaseContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetByEmail(string email)
        {
            User userByEmail = await databaseContext.Users.FirstOrDefaultAsync(c => c.Email == email);

            return userByEmail;
        }

        public async Task<User> GetById(Guid userId)
        {
            User userById = await databaseContext.Users.FirstOrDefaultAsync(c => c.Id == userId);

            return userById;
        }

        public async Task<User> GetByLogin(string login)
        {
            User userByLogin = await databaseContext.Users.FirstOrDefaultAsync(c => c.Login == login);

            return userByLogin;
        }
    }
}
