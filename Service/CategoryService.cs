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
    public class CategoryService : BaseService, ICategoryService
    {
        public CategoryService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Result<CategoryDto> Create(CategoryDto model)
        {
            using (var scope = this._serviceProvider.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var IsAvailable = dbContext.Categories.Where(e => e.Name.Equals(model.Name)).FirstOrDefault();
                if (IsAvailable is null)
                {
                    dbContext.Categories.Add(model.Adapt<Category>());
                    dbContext.SaveChanges();
                    return Result<CategoryDto>.Success(model);
                }
                else
                {
                    return Result<CategoryDto>.Failure("This entry already exist!");
                }
            }
        }

        public Result<bool> Delete(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var model = dBContext.Categories.Where(b => b.Id == id).FirstOrDefault();

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

        public IReadOnlyList<CategoryDto> Get()
        {
            using (var scope = this._serviceProvider.CreateScope())
            using (var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var model = dbcontext.Categories.ToList();
                return model.Adapt<IReadOnlyList<CategoryDto>>();
            }
        }

        public CategoryDto Get(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                return dBContext.Categories.Where(x => x.Id == id).AsNoTracking().FirstOrDefault().Adapt<CategoryDto>();
            }
        }

        public Result<CategoryDto> Update(CategoryDto model)
        {
            throw new NotImplementedException();
        }
    }
}
