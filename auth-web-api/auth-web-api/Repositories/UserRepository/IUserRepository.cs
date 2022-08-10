using auth_web_api.Models.DatabaseObjects;
using System.Threading.Tasks;

namespace auth_web_api.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByLogin(string login);
        Task<User> Create(User user);
    }
}
