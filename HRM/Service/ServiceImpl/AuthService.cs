using System.IO;
using System.Text.Json;
using HRM.Models;
using HRM.Repositories;
using HRM.Repositories.RepositoryImpl;
using Microsoft.EntityFrameworkCore;

namespace HRM.Service.ServiceImpl;

public class AuthService() : BaseRepository<User>(new HrmContext()), IAuthService
{
    private readonly IUserRepository _userRepository = new UserRepository();

    public async Task<User?> LoginAsync(string username, string password)
    {
        User? user = await _userRepository
            .GetQueryable()
            .FirstOrDefaultAsync(u => u!.Username == username);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Tài khoản không tồn tại! Vui lòng thử lại.");
        }
        if (user.Password != (password))
        {
            throw new UnauthorizedAccessException("Mật khẩu không đúng! Vui lòng thử lại.");
        }
        user.LastLoginDate = DateTime.Now;
        await _userRepository.UpdateAsync(user);
        return user;
    }

    public Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        User? user = _userRepository.GetByIdAsync(userId).Result;
        if (user!.Password != (currentPassword))
        {
            throw new UnauthorizedAccessException("Mật khẩu không khớp! Vui lòng thử lại.");
        }
        _userRepository.UpdateAsync(user);
        return Task.FromResult(true);
    }

    public Task LogoutAsync()
    {
        UserSession.Instance.Clear();
        File.Delete("UserSession.json");
        return Task.CompletedTask;
    }


    public async Task SaveUserSessionAsync(User user, string lastPage)
    {
        var sessionData = new { Username = user.Username, Password = user.Password, Role = user.Role, LastPage = lastPage };
        var sessionJson = JsonSerializer.Serialize(sessionData);
        await File.WriteAllTextAsync("UserSession.json", sessionJson);
    }

}