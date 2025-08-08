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
    public class UserRoleService : BaseService, IUserRoleService
    {
        public UserRoleService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Result<UserRoleDto> Create(UserRoleDto model)
        {
            using (var scope = this._serviceProvider.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var IsAvailable = dbContext.UserRoles.Where(e => e.Name.Equals(model.Name)).FirstOrDefault();
                if (IsAvailable is null)
                {
                    dbContext.UserRoles.Add(model.Adapt<UserRole>());
                    dbContext.SaveChanges();
                    return Result<UserRoleDto>.Success(model);
                }
                else
                {
                    return Result<UserRoleDto>.Failure("This entry already exist!");
                }
            }
        }

        public Result<bool> Delete(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var model = dBContext.UserRoles.Where(b => b.Id == id).FirstOrDefault();

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

        public IReadOnlyList<UserRoleDto> Get()
        {
            using (var scope = this._serviceProvider.CreateScope())
            using (var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var model = dbcontext.UserRoles.ToList();
                return model.Adapt<IReadOnlyList<UserRoleDto>>();
            }
        }

        public UserRoleDto Get(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                return dBContext.UserRoles.Where(x => x.Id == id).AsNoTracking().FirstOrDefault().Adapt<UserRoleDto>();
            }
        }

        public Result<UserRoleDto> Update(UserRoleDto model)
        {
            try
            {
                using (var scope = this._serviceProvider.CreateScope())
                using (var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                {
                    var modeldb = dbcontext.UserRoles.Where(b => b.Id == model.Id).AsNoTracking().FirstOrDefault();

                    if (modeldb is not null)
                    {
                        var IsSameEntry = dbcontext.UserRoles.Where(b => b.Id != model.Id && b.Name.Equals(model.Name)).FirstOrDefault();

                        if (IsSameEntry is null)
                        {
                            modeldb = model.Adapt<UserRole>();

                            dbcontext.UserRoles.Update(modeldb);
                            dbcontext.SaveChanges();
                            return Result<UserRoleDto>.Success(model);
                        }
                        else
                        {
                            return Result<UserRoleDto>.Failure("This entry already exist!");
                        }
                    }
                    else
                    {
                        return Result<UserRoleDto>.Failure("Not Found!");
                    }
                }
            }
            catch (Exception x)
            {
                return Result<UserRoleDto>.Failure("Error:" + x.Message);
                throw;
            }
        }
    }
}
