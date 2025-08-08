using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? Image { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public UserRole? UserRole { get; set; }
        public string Password { get; set; }
    }
}
