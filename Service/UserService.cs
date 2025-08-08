using Core.Abstract.Service;
using Core.DTO;
using Core.Helpers;
using Data.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Service.DBContexts;

namespace Service
{
    public class UserService : BaseService, IUserService
    {
        public UserService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Result<UserDto> CanLogin(UserLoginDto user)
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var model = dBContext.Users.Where(e => e.Email.Equals(user.Email)).Include(u => u.UserRole).FirstOrDefault();
                return Result<UserDto>.Success(model.Adapt<UserDto>());
            }
        }

        public Result<UserDto> GetUserForEdit(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var user = dBContext.Users.Where(x => x.Id == id).Include(u => u.UserRole).AsNoTracking().FirstOrDefault();
                return Result<UserDto>.Success(user.Adapt<UserDto>());
            }
        }

        public Result<UserDto> CanLoginForAdmin(UserDto user)
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var model = dBContext.Users.Where(e => e.Email.Equals(user.Email)).Include(u => u.UserRole).FirstOrDefault();
                return Result<UserDto>.Success(model.Adapt<UserDto>());
            }
        }

        public Result<UserDto> Create(UserDto model)
        {
            using (var scope = this._serviceProvider.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var IsAvailable = dbContext.Users.Where(e => e.Email.Equals(model.Email)).FirstOrDefault();
                if (IsAvailable is null)
                {
                    dbContext.Users.Add(model.Adapt<User>());
                    dbContext.SaveChanges();
                    return Result<UserDto>.Success(model);
                }
                else
                {
                    return Result<UserDto>.Failure("This email address is used by another user!");
                }
            }
        }

        public Result<bool> Delete(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var model = dBContext.Users.Where(b => b.Id == id).FirstOrDefault();

                if (model is not null)
                {
                    dBContext.Remove(model);
                    dBContext.SaveChanges();
                    return Result<bool>.Success(true);
                }
                else
                {
                    return Result<bool>.Failure("Not Found!");
                }
            }
        }

        public IReadOnlyList<UserDto> Get()
        {
            using (var scope = this._serviceProvider.CreateScope())
            using (var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var model = dbcontext.Users.Include(u => u.UserRole).ToList();
                return model.Adapt<IReadOnlyList<UserDto>>();
            }
        }

        public UserDto Get(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                return dBContext.Users.Where(x => x.Id == id).AsNoTracking().FirstOrDefault().Adapt<UserDto>();
            }
        }

        public Result<UserDto> Update(UserDto model)
        {
            try
            {
                using (var scope = this._serviceProvider.CreateScope())
                using (var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                {
                    var modeldb = dbcontext.Users.Where(b => b.Id == model.Id).AsNoTracking().FirstOrDefault();

                    if (modeldb is not null)
                    {
                        var IsSameEntry = dbcontext.Users.Where(b => b.Id != model.Id && b.FullName.Equals(model.FullName)).FirstOrDefault();

                        if (IsSameEntry is null)
                        {
                            modeldb = model.Adapt<User>();

                            dbcontext.Users.Update(modeldb);
                            dbcontext.SaveChanges();
                            return Result<UserDto>.Success(model);
                        }
                        else
                        {
                            return Result<UserDto>.Failure("This entry already exist!");
                        }
                    }
                    else
                    {
                        return Result<UserDto>.Failure("Not Found!");
                    }
                }
            }
            catch (Exception x)
            {
                return Result<UserDto>.Failure("Error:" + x.Message);
                throw;
            }
        }
    }
}
