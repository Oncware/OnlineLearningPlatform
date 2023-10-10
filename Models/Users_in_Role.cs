using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JustLearn1.Models
{
    public class Users_in_Role
    {

        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
    }
}
