
using Microsoft.EntityFrameworkCore;
using POS1.Data;
using POS1.Utilities;


namespace POS1.Services
{
    public class UserServices
    {

            private readonly ApplicationDbContext _context;
        
            public UserServices(ApplicationDbContext context)
            {
                _context = context;
            }

            public List<User> GetAllUsers()
            {
                return _context.Users.ToList();
            }

            public async Task<bool> LoginAsync(string username, string password)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    // User not found
                    return false;
                }

                // Verify the password
                bool isValidPassword = Hash.VerifyPassword(password, user.HashedPassword);

                return isValidPassword;
            }
        }
    }

    // Hash a password
    //string hashedpassword = passwordhasher.hashpassword("my_password");

    

