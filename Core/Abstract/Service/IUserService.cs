using Core.DTO;
using Core.Helpers;

namespace Core.Abstract.Service
{
    public interface IUserService : IBaseService<UserDto>
    {
        public Result<UserDto> CanLogin(UserLoginDto user);
        public Result<UserDto> GetUserForEdit(int id);
        public Result<UserDto> CanLoginForAdmin(UserDto user);
    }
}
