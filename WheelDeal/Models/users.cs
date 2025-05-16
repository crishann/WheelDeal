using System.ComponentModel.DataAnnotations;

namespace WheelDeal.Models
{
    public class User
    {
        public int Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "user";  // Default to "user"
    }

}
