using System.Threading.Tasks;
using Access.Auth.Service.Domain.UserManagement;

namespace Access.Auth.Service.Infra.Store
{
    public interface IUserStore
    {
        Task<User> FindUserByUsernameAsync(string username);
        Task<User> FindUserByNicknameAsync(string nickname);
        Task<User> FindUserByUsernameOrNicknameAsync(string name);
        Task<User> FindUserByIdAsync(string id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
