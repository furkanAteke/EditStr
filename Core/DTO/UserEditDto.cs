using Microsoft.AspNetCore.Http;

namespace Core.DTO
{
    public class UserEditDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}
