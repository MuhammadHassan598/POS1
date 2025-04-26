
using Microsoft.EntityFrameworkCore;
using POS1.Data;
using POS1.Utilities;
using POS1.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using POS1.Services;
using POS1.Components.Pages.Products;
using Microsoft.AspNetCore.Identity;
namespace POS1.Services
{
    public class UserServices
    {


        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public UserServices(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public List<User> GetAllUsers()
        {
            using var _context = _contextFactory.CreateDbContext();
            return _context.Users.ToList();
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            await using var _context = _contextFactory.CreateDbContext();
            return await _context.Users.Include(u=>u.Tenant).FirstOrDefaultAsync(u => u.Username == username);
        }
        public List<User> GetUsersByTenantId(int tenantId)
        {
           using var _context = _contextFactory.CreateDbContext();
            return _context.Users.Where(u => u.TenantID == tenantId).ToList();
        }

        public async Task<User?> GetUserByUserIdAsync(int userId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users.FindAsync(userId);
        }
        public List<User> GetUsersByRole(int tenantId, UserRole role)
        {
           using var _context = _contextFactory.CreateDbContext();
            return _context.Users
                           .Where(u => u.TenantID == tenantId && u.RoleIs == role)
                           .ToList();
        }
        public async Task<bool> RegisterUserAsync(User newUser, string userName)
        {
            await using var _context = _contextFactory.CreateDbContext();
            var user = await GetUserByUsernameAsync(userName);

          var users=   GetUsersByTenantId(user.TenantID);
            // Check if the username or email already exists
            if (users.Any(u => u.Username == newUser.Username || u.Email == newUser.Email))
            {
                return false;
           }

          


           newUser.TenantID= user.TenantID;
            // Hash the password
            newUser.HashedPassword = Hash.HashPassword(newUser.HashedPassword);

          
            _context.Users.Add(newUser);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                Console.WriteLine($"Error saving changes: {ex.Message}");
            }

            return true;
        }




        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                await using var _context = _contextFactory.CreateDbContext();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    // User not found
                    return false;
                }
                //CCLNKmSlnV9g7R0TxZBLOQ ==:NWpHIvpELMIa67MRhnzLoeL7pfsAbOhdETSHl9ithBY =
               // string hashedpassword = Hash.HashPassword("12345");
                // Verify the password

                bool isValidPassword = Hash.VerifyPassword(password, user.HashedPassword);

                return isValidPassword;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }

}
    // Hash a password
    //string hashedpassword = passwordhasher.hashpassword("my_password");

    

