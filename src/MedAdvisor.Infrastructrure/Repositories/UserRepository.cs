
using MedAdvisor.DataAccess.MySql.DataContext;
using MedAdvisor.Models;
using Microsoft.EntityFrameworkCore;

namespace MedAdvisor.DataAccess.MySql.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository( AppDbContext dbContext)
        {
          _db = dbContext;   
        }

        public async Task<User> AddUserAsync(User new_user)
        {
            await _db.Users.AddAsync(new_user);
            await _db.SaveChangesAsync();
            return new_user;
        }

        public async Task<User> FetchUserData(string email)
        {
            var user = await _db.Users
                .Include(u => u.Allergies)
                .Include(u=>u.Medicines)
                .Include(u=>u.Vaccines)
                .Include(u=>u.Diagnoses)
                .Include(u=>u.Documents)
                .FirstOrDefaultAsync(user => user.Email == email);
            return user;
        }


        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(
                           user => user.Email == email);
            return user;
        }

        public async Task<User> GetUserById(Guid id)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == id);
            return user;
        }
    }
}
