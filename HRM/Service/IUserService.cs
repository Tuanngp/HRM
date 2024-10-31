using HRM.Repositories;
using User = HRM.Models.User;

namespace HRM.Service;

public interface IUserService : IBaseRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
}