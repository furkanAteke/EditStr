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
    public class EditService : BaseService, IEditService
    {
        public EditService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Result<EditDto> Create(EditDto model)
        {
            using (var scope = this._serviceProvider.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var IsAvailable = dbContext.Edits.Where(e => e.EditFull.Equals(model.EditFull)).FirstOrDefault();
                if (IsAvailable is null)
                {
                    dbContext.Edits.Add(model.Adapt<Edit>());
                    dbContext.SaveChanges();
                    return Result<EditDto>.Success(model);
                }
                else
                {
                    return Result<EditDto>.Failure("This entry already exist!");
                }
            }
        }

        public Result<bool> Delete(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                var model = dBContext.Edits.Where(b => b.Id == id).FirstOrDefault();

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

        public IReadOnlyList<EditDto> Get()
        {
            using (var scope = this._serviceProvider.CreateScope())
            using (var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                return dbcontext.Edits
                .Select(e => new EditDto
                {
                    Id = e.Id,
                    EditName = e.EditName,
                    EditFull = e.EditFull,
                    CategoryId = e.CategoryId,
                    Description = e.Description,
                     Category = new CategoryDto
                     {
                         Id = e.Category.Id,
                         Name = e.Category.Name,
                         Description = e.Category.Description
                     },
                })
                .ToList();
            }
        }

        public EditDto Get(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                return dBContext.Edits.Where(x => x.Id == id).AsNoTracking().FirstOrDefault().Adapt<EditDto>();
            }
        }

        public Result<EditDto> Update(EditDto model)
        {
            throw new NotImplementedException();
        }

    }
}
