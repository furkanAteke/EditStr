using Core.DTO;

namespace EditStr.Models
{
    public class RoleViewModel
    {
        public UserRoleDto Response { get; set; }
        public IReadOnlyList<UserRoleDto> List { get; set; }
    }
}
