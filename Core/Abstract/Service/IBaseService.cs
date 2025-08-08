using Core.Helpers;

namespace Core.Abstract.Service
{
    public interface IBaseService<T>
    {
        public IReadOnlyList<T> Get();
        public T Get(int id);
        public Result<T> Create(T model);
        public Result<T> Update(T model);
        public Result<bool> Delete(int id);
    }
}
