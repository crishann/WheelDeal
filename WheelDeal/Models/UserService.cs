using Microsoft.EntityFrameworkCore;
using WheelDeal.Models;
using System.Threading.Tasks; // Make sure this is included

namespace WheelDeal.Services // Assuming UserService is in a 'Services' folder
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Method to check if email exists
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        // Method to add a new user
        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        // Method to get a user by email
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // --- New: Method to get a user by ID ---
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id); // FindAsync is efficient for primary keys
        }

        // --- New: Method to update an existing user ---
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user); // Attaches the entity and marks it as modified
            await _context.SaveChangesAsync();
        }
    }
}