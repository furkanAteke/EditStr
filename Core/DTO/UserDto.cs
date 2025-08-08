using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Image { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public UserRoleDto? UserRole { get; set; }
        public string? Password { get; set; }
        public string? PasswordAgain { get; set; }
    }
}
