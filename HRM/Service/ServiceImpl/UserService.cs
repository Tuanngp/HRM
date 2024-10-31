using HRM.Models;
using HRM.Repositories;
using HRM.Repositories.RepositoryImpl;
using Microsoft.EntityFrameworkCore;

namespace HRM.Service.ServiceImpl;

public class UserService() : BaseRepository<User>(new HrmContext()), IUserService
{
    private IUserRepository _userRepository = new UserRepository();
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _userRepository.GetQueryable().FirstOrDefaultAsync(u => u!.Username == username);
    }
}