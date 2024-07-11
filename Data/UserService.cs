namespace CagedApi.Services;
using CagedApi.Models;
public interface IUserService
{
    User? Save(User user);
    User? GetUserByEmail(string email);
    User? CreateUserFromEmail(string email);
};
public class UserService(SystemDbContext dbContext) : IUserService
{
    public User? Update(User user)
    {
        var result = dbContext.Update(user);

        dbContext.SaveChanges();

        if (result.Entity is null) return default;

        return result.Entity;
    }
    public User? Save(User user)
    {
        var result = dbContext.Add(user);

        if (result.Entity is null) return default;

        dbContext.SaveChanges();

        return result.Entity;
    }
    public User? CreateUserFromEmail(string email)
    {
        var userToSave = new User { Email = email, Id = Guid.NewGuid().ToString() };

        var result = dbContext.Add(userToSave);

        if (result.Entity is null) return default;

        var saveChangesResult = dbContext.SaveChanges();

        if (saveChangesResult == 0) return default;

        return result.Entity;
    }
    public User? GetUserByEmail(string email)
    {
        try
        {
            var result = dbContext.Users.First(u => u.Email == email);

            return result;
        }
        catch { return default; }
    }
}

